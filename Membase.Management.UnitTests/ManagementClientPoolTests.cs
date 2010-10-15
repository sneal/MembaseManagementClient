using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Membase.Management;
using Membase.Management.Impl;
using NUnit.Framework;
using Rhino.Mocks;

namespace Membase.Management.UnitTests
{
	[TestFixture]
	public class ManagementClientPoolTests
	{
		private ManagementClientPool _pool;
		private List<IMembaseManagementClient> _clients;

		[Test]
		public void Ctor_throws_when_clients_list_is_null()
		{
			Assert.Throws<ArgumentNullException>(() => new ManagementClientPool(null));
		}

		[Test]
		public void Ctor_throws_when_clients_list_is_empty()
		{
			Assert.Throws<ArgumentException>(() => new ManagementClientPool(new IMembaseManagementClient[0]));
		}

		[Test]
		public void Count_returns_the_number_of_clients_in_the_pool()
		{
			CreateManagementClientPool(5);
			Assert.AreEqual(5, _pool.Count);
		}

		[Test]
		public void MaxFailuresPerClient_defaults_to_10()
		{
			CreateManagementClientPool();
			Assert.AreEqual(10, _pool.MaxFailuresPerClient);
		}

		[Test]
		public void HandleClientFailure_goes_to_the_next_client_with_multiple_threads()
		{
			const int numClients = 500;

			CreateManagementClientPool(numClients);
			Parallel.For(0, numClients, i =>
			{
				var currentClient = _pool.GetCurrentClient();
				_pool.HandleClientFailure(currentClient);
				var nextClient = _pool.GetCurrentClient();
				Assert.AreNotSame(nextClient, currentClient);
			});
		}

		#region Setup

		public void CreateManagementClientPool()
		{
			CreateManagementClientPool(1);
		}

		public void CreateManagementClientPool(int numClients)
		{
			_clients = new List<IMembaseManagementClient>();
			for (int i = 0; i < numClients; i++)
			{
				_clients.Add(MockRepository.GenerateStub<IMembaseManagementClient>());
			}
			_pool = new ManagementClientPool(_clients);

		}

		#endregion

	}
}
