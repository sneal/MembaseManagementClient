using System;
using System.Collections.Generic;
using System.Linq;

namespace Membase.Management.Impl
{
	/// <summary>
	/// Thread safe pool of ManagementClient instances.
	/// </summary>
	public class ManagementClientPool
	{
		// shared lock for updating the current client
		private static readonly object ClientLock = new object();

		private readonly List<ClientContainer> _clients;
		private int _currentClientIdx;

		/// <summary>
		/// Creates a pool of management clients.
		/// </summary>
		/// <param name="clients">List of clients to initialize the pool with</param>
		public ManagementClientPool(IEnumerable<IMembaseManagementClient> clients)
		{
			if (clients == null)
				throw new ArgumentNullException("clients");

			if (clients.Count() < 1)
				throw new ArgumentException("Expected at least one client");

			_clients = new List<ClientContainer>(clients.Select(c => new ClientContainer(c)));
			MaxFailuresPerClient = 10;
		}

		/// <summary>
		/// The number of clients in the pool.
		/// </summary>
		public int Count
		{
			get { return _clients.Count; }
		}

		/// <summary>
		/// The maximum number of failures per client in the pool.  The default is 10.
		/// </summary>
		public int MaxFailuresPerClient { get; set; }

		/// <summary>
		/// This should be called when a client call failed with a connection exception.
		/// This will then go to the next available client in the client list for 
		/// GetCurrentClient().
		/// </summary>
		/// <param name="failedClient">The client instance that failed.</param>
		public void HandleClientFailure(IMembaseManagementClient failedClient)
		{
			if (failedClient == null)
				throw new ArgumentNullException("failedClient");

			lock (ClientLock)
			{
				// see if another thread already handled this client's failure
				var currentContainer = GetCurrentClientContainer();
				if (failedClient != currentContainer.Client)
				{
					return;
				}

				// record that this client failed
				currentContainer.FailureCount++;

				// go to the next client in the list
				IncrementCurrentClientIndex();
			}
		}

		/// <summary>
		/// Gets the current client to use.
		/// </summary>
		public IMembaseManagementClient GetCurrentClient()
		{
			var container = GetCurrentClientContainer();
			return container.Client;
		}

		/// <summary>
		/// Gets the current client wrapped in a meta-data container.
		/// </summary>
		private ClientContainer GetCurrentClientContainer()
		{
			ClientContainer container = _clients[_currentClientIdx];
			if (container.FailureCount > MaxFailuresPerClient)
			{
				string msg = string.Format(
					"Failed to connect to the remote endpoint {0} more than {1} times. This usually means that " +
					"no Membase servers are running or all of the configured endpoint addresses are wrong.",
					container.Client.Endpoint,
					MaxFailuresPerClient);
				throw new MembaseManagementConnectionException(msg);
			}
			return container;
		}

		private void IncrementCurrentClientIndex()
		{
			// must atomically increment and check bounds because of reader threads
			var newIdx = _currentClientIdx;
			newIdx++;
			if (newIdx >= _clients.Count)
			{
				newIdx = 0;
			}
			_currentClientIdx = newIdx;
		}

		private class ClientContainer
		{
			public ClientContainer(IMembaseManagementClient client)
			{
				Client = client;
			}

			public int FailureCount { get; set; }
			public IMembaseManagementClient Client { get; private set; }
		}
	}
}
