using System;
using Membase.Management.Impl;
using Membase.Management.Resources;
using NUnit.Framework;

namespace Membase.Management.IntegrationTests
{
	[TestFixture]
	public class ManagementClientIntegrationTests
	{
		private const int BucketSize = 64;
		private const string BucketName = "TestBucket";

		private ManagementClient _client;
		private PooledManagementClient _pooledClient;

		[TestFixtureSetUp]
		public void OneTimeSetUp()
		{
			_client = (ManagementClient) MembaseManagementClientFactory.Create(
				TestConfiguration.AdminUserName,
				TestConfiguration.AdminPassword,
				TestConfiguration.MembaseManagementUri);

			_pooledClient = (PooledManagementClient) MembaseManagementClientFactory.Create(
				TestConfiguration.AdminUserName,
				TestConfiguration.AdminPassword,
				TestConfiguration.MembaseManagementUri,
				TestConfiguration.MembaseManagementUri);
		}

		[Test]
		public void GetDefaultPool_gets_the_default_pool()
		{
			ExecuteUsingEachClient(client =>
			{
				Pool defaultPool = client.GetDefaultPool();
				Assert.IsNotNull(defaultPool);
				Assert.AreEqual("default", defaultPool.Name);
			});
		}

		[Test]
		public void Can_create_get_then_delete_bucket()
		{
			var createRequest = BucketCreationDetails.PasswordProtectedMemcacheBucket(BucketName, BucketSize);
			ExecuteUsingEachClient(client =>
			{
			    client.CreateBucket(createRequest);
			    Bucket bucket = _client.GetBucket(BucketName);
			    Assert.AreEqual(BucketName, bucket.Name);
			    client.DeleteBucket(BucketName);
			});
		}

		[Test]
		public void BucketExists_safely_checks_for_bucket_existance()
		{
			var createRequest = BucketCreationDetails.PasswordProtectedMemcacheBucket(BucketName, BucketSize);
			ExecuteUsingEachClient(client =>
			{
				client.CreateBucket(createRequest);
			    Assert.IsTrue(client.BucketExists(BucketName));
			    client.DeleteBucket(BucketName);
			    Assert.IsFalse(client.BucketExists(BucketName));
			});
		}

		[Test]
		public void Creating_a_already_existing_bucket_is_idempotent()
		{
			var createRequest = BucketCreationDetails.PasswordProtectedMemcacheBucket(BucketName, BucketSize);
			ExecuteUsingEachClient(client =>
			{
				client.CreateBucket(createRequest);
				Assert.DoesNotThrow(() => client.CreateBucket(createRequest));
			});
		}

		[Test]
		[Ignore("Flushing is not currently implemented server side")]
		public void Flushing_a_bucket()
		{
			var createRequest = BucketCreationDetails.PasswordProtectedMemcacheBucket(BucketName, BucketSize);
			ExecuteUsingEachClient(client =>
			{
				client.CreateBucket(createRequest);
				Assert.DoesNotThrow(() => client.FlushBucket(BucketName));
			});
		}

		[Test]
		public void Getting_a_bucket_gets_the_bucket_details()
		{
			var createRequest = BucketCreationDetails.PasswordProtectedMemcacheBucket(BucketName, BucketSize);
			ExecuteUsingEachClient(client =>
			{
				client.CreateBucket(createRequest);
				Bucket bucket = client.GetBucket(BucketName);
				Assert.AreEqual(BucketName, bucket.Name);
			});
		}

		[Test]
		public void Getting_a_non_existant_bucket_throws_exception()
		{
			ExecuteUsingEachClient(client =>
				Assert.Throws<MembaseManagementRestException>(() => client.GetBucket(BucketName)));
		}

		[Test]
		public void Deleting_a_bucket_is_idempotent()
		{
			ExecuteUsingEachClient(client =>
				Assert.DoesNotThrow(() => _client.DeleteBucket(BucketName),
				"Deleting a non-existant bucket threw an exception"));
		}

		[Test]
		public void Creating_a_bucket_against_a_non_existant_server_throws_an_exception()
		{
			var createRequest = BucketCreationDetails.PasswordProtectedMemcacheBucket(BucketName, BucketSize);
			var restClient = new RestClient(new Uri("http://localhost:6666"));
			var client = new ManagementClient(restClient);
			Assert.Throws<MembaseManagementConnectionException>(
				() => client.CreateBucket(createRequest));
		}

		private void EnsureBucketDoesNotExist(string bucketName)
		{
			if (_client.BucketExists(bucketName))
			{
				_client.DeleteBucket(bucketName);
			}
		}

		private void ExecuteUsingEachClient(Action<IMembaseManagementClient> clientAction)
		{
			EnsureBucketDoesNotExist(BucketName);
			clientAction(_client);

			EnsureBucketDoesNotExist(BucketName);
			clientAction(_pooledClient);
		}

	}
}
