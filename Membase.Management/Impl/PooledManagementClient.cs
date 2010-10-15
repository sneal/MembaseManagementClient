using System;
using log4net;
using Membase.Management.Resources;

namespace Membase.Management.Impl
{
	/// <summary>
	/// Pooled client that will retry a management invocation on the subsequent client if needed.
	/// </summary>
	public class PooledManagementClient : IMembaseManagementClient
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof (PooledManagementClient));
		private readonly ManagementClientPool _clientPool;

		public PooledManagementClient(ManagementClientPool clientPool)
		{
			if (clientPool == null)
				throw new ArgumentNullException("clientPool");

			_clientPool = clientPool;
		}

		public Pool GetDefaultPool()
		{
			return ExecuteWithRetry(c => c.GetDefaultPool());
		}

		public Bucket GetBucket(string bucketName)
		{
			return ExecuteWithRetry(c => c.GetBucket(bucketName));
		}

		public void DeleteBucket(string bucketName)
		{
			ExecuteWithRetry(c => c.DeleteBucket(bucketName));
		}

		public bool BucketExists(string bucketName)
		{
			return ExecuteWithRetry(c => c.BucketExists(bucketName));
		}

		public void CreateBucket(BucketCreationDetails bucketCreationDetails)
		{
			ExecuteWithRetry(c => c.CreateBucket(bucketCreationDetails));
		}

		public void FlushBucket(string bucketName)
		{
			ExecuteWithRetry(c => c.FlushBucket(bucketName));
		}

		public Uri Endpoint
		{
			get { return _clientPool.GetCurrentClient().Endpoint; }
		}

		private int MaxRetries
		{
			get { return _clientPool.Count; }
		}

		private void ExecuteWithRetry(Action<IMembaseManagementClient> clientAction)
		{
			for (int failCount = 0; failCount < MaxRetries; failCount++)
			{
				var managementClient = _clientPool.GetCurrentClient();
				try
				{
					clientAction(managementClient);
					return;
				}
				catch (MembaseManagementConnectionException ex)
				{
					HandleClientFailure(managementClient, ex);
				}
			}

			throw new MembaseManagementConnectionException(CreateFailureExceptionMessage());
		}


		private T ExecuteWithRetry<T>(Func<IMembaseManagementClient, T> clientAction)
		{
			for (int failCount = 0; failCount < MaxRetries; failCount++)
			{
				var managementClient = _clientPool.GetCurrentClient();
				try
				{
					return clientAction(managementClient);
				}
				catch (MembaseManagementConnectionException ex)
				{
					HandleClientFailure(managementClient, ex);
				}
			}

			throw new MembaseManagementConnectionException(CreateFailureExceptionMessage());
		}

		private void HandleClientFailure(
			IMembaseManagementClient managementClient,
			MembaseManagementConnectionException ex)
		{
			string msg = string.Format(
				"A Membase client failed making a call to endpoint {0}, retrying on the next available endpoint",
				managementClient.Endpoint);
			Log.Warn(msg, ex);

			_clientPool.HandleClientFailure(managementClient);
		}

		private string CreateFailureExceptionMessage()
		{
			return string.Format(
				"Failed to connect to a Membase server more than {0} times. This usually means that " +
				"no Membase servers are running or all of the configured endpoint addresses are wrong.",
				MaxRetries);
		}

	}
}
