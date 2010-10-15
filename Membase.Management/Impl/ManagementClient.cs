using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Membase.Management.Resources;

namespace Membase.Management.Impl
{
	/// <summary>
	/// Non-pooled base client for managing a Membase server instance.
	/// </summary>
	/// <remarks>
	/// This class is thread safe.
	/// </remarks>
	public class ManagementClient : IMembaseManagementClient
	{
		private readonly IRestClient _restClient;

		/// <summary>
		/// Creates a new management client for a specific URI.
		/// </summary>
		/// <param name="restClient">The REST gateway</param>
		public ManagementClient(IRestClient restClient)
		{
			if (restClient == null)
				throw new ArgumentNullException("restClient");

			_restClient = restClient;
		}

		public Pool GetDefaultPool()
		{
			RestResponse restResponse = _restClient.Get(ResourceUris.Pools);
			if (restResponse.IsError)
			{
				throw new MembaseManagementRestException(restResponse.Body, restResponse.StatusCode);
			}

			var allPools = DeserializeJson<Connection>(restResponse.Body);
			if (allPools == null || allPools.Pools == null || allPools.Pools.Count < 1)
			{
				throw new MembaseManagementException(
					"Invalid response from the Membase management server. No pools were found!");
			}

			// Membase currently only supports one pool
			return allPools.Pools[0];
		}

		public void CreateBucket(BucketCreationDetails bucketCreationDetails)
		{
			var parameters = ToBucketCreationParameters(bucketCreationDetails);

			RestResponse restResponse = _restClient.Post(ResourceUris.DefaultPoolBuckets, parameters);
			if (restResponse.IsError)
			{
				// if bucket already exists, then we're golden
				if (!restResponse.Body.Contains("Bucket with given name already exists"))
				{
					throw new MembaseManagementRestException(restResponse.Body, restResponse.StatusCode);
				}
			}
		}

		public Bucket GetBucket(string bucketName)
		{
			RestResponse restResponse = _restClient.Get(CreateBucketPath(bucketName));
			if (restResponse.IsError)
			{
				throw new MembaseManagementRestException(restResponse.Body, restResponse.StatusCode);
			}

			return DeserializeJson<Bucket>(restResponse.Body);
		}

		public void DeleteBucket(string bucketName)
		{
			RestResponse restResponse = _restClient.Delete(CreateBucketPath(bucketName));
			if (restResponse.IsError && restResponse.StatusCode != HttpStatusCode.NotFound)
			{
				throw new MembaseManagementRestException(restResponse.Body, restResponse.StatusCode);
			}
		}

		public bool BucketExists(string bucketName)
		{
			RestResponse restResponse = _restClient.Get(CreateBucketPath(bucketName));
			if (restResponse.StatusCode == HttpStatusCode.NotFound)
			{
				return false;
			}

			// assume all other rest errors are real errors
			if (restResponse.IsError)
			{
				throw new MembaseManagementRestException(restResponse.Body, restResponse.StatusCode);
			}

			return true;
		}

		public void FlushBucket(string bucketName)
		{
			string bucketPath = CreateBucketPath(bucketName) + "/controller/doFlush";
			RestResponse restResponse = _restClient.Post(bucketPath, new Dictionary<string, string>());
			if (restResponse.IsError)
			{
				throw new MembaseManagementRestException(restResponse.Body, restResponse.StatusCode);
			}
		}

		public Uri Endpoint
		{
			get { return _restClient.Endpoint; }
		}

		/// <summary>
		/// The REST client gateway instance.
		/// </summary>
		public IRestClient RestClient
		{
			get { return _restClient; }
		}

		private IDictionary<string, string> ToBucketCreationParameters(BucketCreationDetails bucketCreationDetails)
		{
			AssertBucketNameIsValid(bucketCreationDetails.BucketName);
			AssertBucketSizeIsValid(bucketCreationDetails.Size);

			var parameters = new Dictionary<string, string>
			{
			    {"name", bucketCreationDetails.BucketName},
			    {"ramQuotaMB", bucketCreationDetails.Size.ToString()},
				{"authType", bucketCreationDetails.AuthType.ToString().ToLowerInvariant()},
				{"replicaNumber", bucketCreationDetails.ReplicaNumber.ToString()}
			};

			// the default is membase
			if (bucketCreationDetails.BucketType == BucketType.Memcache)
			{
				parameters["bucketType"] = "memcached";
			}

			// using either SASL or proxy port
			if (bucketCreationDetails.ProxyPort > 0)
			{
				parameters["proxyPort"] = bucketCreationDetails.ProxyPort.ToString();
			}
			else
			{
				parameters["saslPassword"] = bucketCreationDetails.SaslPassword;
			}

			return parameters;
		}

		private static string CreateBucketPath(string bucketName)
		{
			AssertBucketNameIsValid(bucketName);
			return ResourceUris.DefaultPoolBuckets + "/" + bucketName;
		}

		private static T DeserializeJson<T>(string jsonBody)
		{
			try
			{
				return JsonConvert.DeserializeObject<T>(jsonBody);
			}
			catch (JsonReaderException ex)
			{
				throw new MembaseManagementException("Could not deserialize the Membase REST response", ex);
			}
		}

		private static void AssertBucketNameIsValid(string bucketName)
		{
			if (string.IsNullOrWhiteSpace(bucketName))
			{
				throw new ArgumentException("You must specify a bucket name");
			}
			if (bucketName.Contains(" ") || bucketName.Contains("\t") || bucketName.Contains("\r") || bucketName.Contains("\n"))
			{
				throw new ArgumentException("The bucket name cannot contain whitespace");
			}
			if (bucketName.StartsWith("_"))
			{
				throw new ArgumentException("Bucket names cannot start with a leading underscore");
			}
		}

		private static void AssertBucketSizeIsValid(int bucketSizeInMegaBytes)
		{
			if (bucketSizeInMegaBytes < 64)
			{
				throw new ArgumentException("The bucket size must be at least 64MB in size");
			}
			if (bucketSizeInMegaBytes > 1024)
			{
				throw new ArgumentException("The bucket size must be less than 1024MB in size");
			}			
		}
	}
}
