using System;
using System.Collections.Generic;
using System.Net;
using Membase.Management;
using Membase.Management.Impl;
using NUnit.Framework;
using Rhino.Mocks;

namespace Membase.Management.UnitTests
{
	[TestFixture]
	public class ManagementClientTests
	{
		private ManagementClient _client;
		private IRestClient _restClient;

		[SetUp]
		public void SetUp()
		{
			_restClient = MockRepository.GenerateStub<IRestClient>();
			_client = new ManagementClient(_restClient);
		}

		[Test]
		public void Ctor_throws_when_rest_client_is_null()
		{
			_restClient = null;
			Assert.Throws<ArgumentNullException>(() => new ManagementClient(_restClient));
		}

		[Test]
		public void GetDefaultPool_throws_when_rest_call_fails()
		{
			_restClient.Stub(c => c.Get(""))
				.IgnoreArguments()
				.Return(new RestResponse(HttpStatusCode.BadRequest, "err"));

			Assert.Throws<MembaseManagementRestException>(() => _client.GetDefaultPool());
		}

		[Test]
		public void GetDefaultPool_throws_when_rest_call_returns_empty_body()
		{
			_restClient.Stub(c => c.Get(""))
				.IgnoreArguments()
				.Return(new RestResponse(HttpStatusCode.OK, ""));

			Assert.Throws<MembaseManagementException>(() => _client.GetDefaultPool());
		}

		[Test]
		public void GetBucket_creates_the_correct_rest_uri()
		{
			const string expectedResourcePath = "pools/default/buckets/testbucket";

			// must return empty string instead of null because of JSON deserializer
			_restClient.Expect(o => o.Get(expectedResourcePath)).Return(new RestResponse(HttpStatusCode.OK, ""));
			_client.GetBucket("testbucket");
		}

		[Test]
		public void GetBucket_throws_when_rest_call_fails()
		{
			var errorResponse = new RestResponse(HttpStatusCode.BadRequest, "err");

			_restClient.Stub(o => o.Get(""))
				.IgnoreArguments()
				.Return(errorResponse);

			Assert.Throws<MembaseManagementRestException>(() => _client.GetBucket("testbucket"));
		}

		[Test]
		public void GetBucket_throws_when_json_deserialization_fails()
		{
			var garbageResponse = new RestResponse(HttpStatusCode.OK, "nfjk garbage [//");

			_restClient.Stub(o => o.Get(""))
				.IgnoreArguments()
				.Return(garbageResponse);

			Assert.Throws<MembaseManagementException>(() => _client.GetBucket("testbucket"));
		}

		[Test]
		public void DeleteBucket_creates_the_correct_rest_uri()
		{
			const string expectedResourcePath = "pools/default/buckets/testbucket";
			var restResponse = new RestResponse(HttpStatusCode.OK, "");

			_restClient.Expect(o => o.Delete(expectedResourcePath))
				.Return(restResponse);

			_client.DeleteBucket("testbucket");
		}

		[Test]
		public void DeleteBucket_throws_when_rest_call_fails()
		{
			var restError = new RestResponse(HttpStatusCode.BadRequest, "err");

			_restClient.Stub(c => c.Delete(""))
				.IgnoreArguments()
				.Return(restError);

			Assert.Throws<MembaseManagementRestException>(() => _client.DeleteBucket("testbucket"));
		}

		[Test]
		public void DeleteBucket_does_not_throw_when_bucket_does_not_exist()
		{
			var bucketNotFound = new RestResponse(HttpStatusCode.NotFound, "");

			_restClient.Stub(c => c.Delete(""))
				.IgnoreArguments()
				.Return(bucketNotFound);

			Assert.DoesNotThrow(() => _client.DeleteBucket("testbucket"));
		}

		[Test]
		public void CreateBucket_does_not_throw_when_bucket_already_exists()
		{
			var bucketAlreadyExistsResponse = new RestResponse(
				HttpStatusCode.BadRequest, "{\"errors\":{\"name\":\"Bucket with given name already exists\"},\"summaries\":{\"ramSummary\":{\"total\":536870912,\"otherBuckets\":104857600,\"nodesCount\":1,\"perNodeMegs\":100,\"thisAlloc\":104857600,\"thisUsed\":0,\"free\":327155712},\"hddSummary\":{\"total\":249620795392.0,\"otherData\":142283853373.0,\"otherBuckets\":0,\"thisUsed\":0,\"free\":107336942019.0}}}");

			_restClient.Stub(c => c.Post("", new Dictionary<string, string>()))
				.IgnoreArguments()
				.Return(bucketAlreadyExistsResponse);

			var createRequest = BucketCreationDetails.PasswordProtectedMemcacheBucket("testbucket", 64);
			Assert.DoesNotThrow(() => _client.CreateBucket(createRequest));
		}


		[Test]
		public void CreateBucket_throws_when_rest_call_returns_error()
		{
			var errResponse = new RestResponse(
				HttpStatusCode.BadRequest, "err");

			_restClient.Stub(c => c.Post("", new Dictionary<string, string>()))
				.IgnoreArguments()
				.Return(errResponse);

			var createRequest = BucketCreationDetails.PasswordProtectedMemcacheBucket("testbucket", 64);
			Assert.Throws<MembaseManagementRestException>(() => _client.CreateBucket(createRequest));
		}

		[Test]
		public void CreateBucket_creates_the_correct_rest_uri()
		{
			const string expectedResourcePath = "pools/default/buckets";

			var parameters = new Dictionary<string, string>
			{
			    {"name", "testbucket"},
			    {"ramQuotaMB", 64.ToString()},
				{"authType", "sasl"},
				{"replicaNumber", "0"},
				{"bucketType", "memcached"},
				{"saslPassword", "testbucket"}
			};

			var restResponse = new RestResponse(HttpStatusCode.OK, "");

			_restClient.Expect(c => c.Post(expectedResourcePath, parameters))
				.Return(restResponse);

			var createRequest = BucketCreationDetails.PasswordProtectedMemcacheBucket("testbucket", 64);
			_client.CreateBucket(createRequest);
		}

		[Test]
		public void CreateBucket_throws_when_name_contains_whitespace()
		{
			var createRequest = BucketCreationDetails.PasswordProtectedMemcacheBucket("test bucket", 64);
			Assert.Throws<ArgumentException>(() => _client.CreateBucket(createRequest));
		}

		[Test]
		public void CreateBucket_throws_when_name_starts_with_underscore()
		{
			var createRequest = BucketCreationDetails.PasswordProtectedMemcacheBucket("_testbucket", 64);
			Assert.Throws<ArgumentException>(() => _client.CreateBucket(createRequest));
		}

		[Test]
		public void CreateBucket_throws_when_name_is_empty_string()
		{
			var createRequest = BucketCreationDetails.PasswordProtectedMemcacheBucket("", 64);
			Assert.Throws<ArgumentException>(() => _client.CreateBucket(createRequest));
		}

		[Test]
		public void CreateBucket_throws_when_name_is_null()
		{
			var createRequest = BucketCreationDetails.PasswordProtectedMemcacheBucket(null, 64);
			Assert.Throws<ArgumentException>(() => _client.CreateBucket(createRequest));
		}

		[Test]
		public void CreateBucket_throws_when_size_is_less_than_64MB()
		{
			var createRequest = BucketCreationDetails.PasswordProtectedMemcacheBucket("bucketname", 63);
			Assert.Throws<ArgumentException>(() => _client.CreateBucket(createRequest));

			createRequest.Size = -3;
			Assert.Throws<ArgumentException>(() => _client.CreateBucket(createRequest));
		}

		[Test]
		public void CreateBucket_throws_when_size_is_greater_than_1GB()
		{
			var createRequest = BucketCreationDetails.PasswordProtectedMemcacheBucket("bucketname", 1025);
			Assert.Throws<ArgumentException>(() => _client.CreateBucket(createRequest));
		}

		[Test]
		public void DeleteBucket_throws_when_name_contains_whitespace()
		{
			Assert.Throws<ArgumentException>(() => _client.DeleteBucket("bucket name"));
		}

		[Test]
		public void DeleteBucket_throws_when_name_starts_with_underscore()
		{
			Assert.Throws<ArgumentException>(() => _client.DeleteBucket("_bucketname"));
		}

		[Test]
		public void BucketExists_throws_when_name_contains_whitespace()
		{
			Assert.Throws<ArgumentException>(() => _client.BucketExists("bucket name"));
		}

		[Test]
		public void BucketExists_throws_when_name_starts_with_underscore()
		{
			Assert.Throws<ArgumentException>(() => _client.BucketExists("_bucketname"));
		}

		[Test]
		public void BucketExists_throws_when_name_is_empty_string()
		{
			Assert.Throws<ArgumentException>(() => _client.BucketExists(""));
		}

		[Test]
		public void BucketExists_throws_when_name_is_null()
		{
			Assert.Throws<ArgumentException>(() => _client.BucketExists(null));
		}

		[Test]
		public void BucketExists_returns_true_when_bucket_exists()
		{
			var restResponse = new RestResponse(HttpStatusCode.OK, "");

			_restClient.Stub(r => r.Get(""))
				.IgnoreArguments()
				.Return(restResponse);

			Assert.IsTrue(_client.BucketExists("testbucket"));
		}

		[Test]
		public void BucketExists_returns_false_when_bucket_does_not_exist()
		{
			var restResponse = new RestResponse(HttpStatusCode.NotFound, "");

			_restClient.Stub(r => r.Get(""))
				.IgnoreArguments()
				.Return(restResponse);

			Assert.IsFalse(_client.BucketExists("testbucket"));
		}

		[Test]
		public void BucketExists_throws_when_rest_call_fails()
		{
			var restResponse = new RestResponse(HttpStatusCode.BadRequest, "");

			_restClient.Stub(r => r.Get(""))
				.IgnoreArguments()
				.Return(restResponse);

			Assert.Throws<MembaseManagementRestException>(() => _client.BucketExists("testbucket"));
		}
	}
}
