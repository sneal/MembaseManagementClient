# Membase management client for .NET

## License: Apache License 2.0  

### Features

* Create, Delete, Get operations on buckets.
* Failover via a pool of Membase management REST endpoints.
* Operations should be idempotent and threadsafe.

### Limitations

* Only the default pool is currently supported.
* Flush does not work in Membase 1.6.0

### Basic Usage

	const int bucketSizeInMB = 128;
	const string bucketName = "Tenant1";

	// create a client for the localhost membase instance
	var managementClient = MembaseManagementClientFactory.Create("Administrator", "password");
	
	// create a (SASL) password protected memcache bucket, password == bucketName
	var createRequest = BucketCreationDetails.PasswordProtectedMemcacheBucket(bucketName, bucketSizeInMB);
	managementClient.CreateBucket(createRequest);
	
	// delete the bucket if it exists
	if (managementClient.BucketExists(bucketName))
	{
		managementClient.DeleteBucket(bucketName);
	}
	