namespace Membase.Management
{
	/// <summary>
	/// Bucket creation detail parameter object.
	/// </summary>
	public class BucketCreationDetails
	{
		private BucketCreationDetails() { }

		/// <summary>
		/// The name of the bucket
		/// </summary>
		public string BucketName { get; set; }

		/// <summary>
		/// The size of the bucket, must be at least 64MB
		/// </summary>
		public int Size { get; set; }

		/// <summary>
		/// The authentication type the bucket should use.
		/// </summary>
		public AuthType AuthType { get; set; }

		/// <summary>
		/// The type of bucket to create, either membase or memcache.
		/// </summary>
		public BucketType BucketType { get; set; }

		/// <summary>
		/// The SASL password. Only valid when proxy port is not set.
		/// </summary>
		public string SaslPassword { get; set; }

		/// <summary>
		/// The replica number.
		/// </summary>
		public int ReplicaNumber { get; set; }

		/// <summary>
		/// The proxy port, required when SASL is not used.
		/// </summary>
		public int ProxyPort { get; set; }

		/// <summary>
		/// Creates a memcache BucketCreationDetails instance that is SASL password protected.
		/// The password is the same as the bucket name.
		/// </summary>
		/// <param name="bucketName">The new bucket name</param>
		/// <param name="size">The size of the bucket in MB</param>
		/// <returns>The new instance</returns>
		public static BucketCreationDetails PasswordProtectedMemcacheBucket(
			string bucketName,
			int size)
		{
			return PasswordProtectedMemcacheBucket(bucketName, bucketName, size);
		}

		/// <summary>
		/// Creates a memcache BucketCreationDetails instance that is SASL password protected.
		/// </summary>
		/// <param name="bucketName">The new bucket name</param>
		/// <param name="password">The bucket password</param>
		/// <param name="size">The size of the bucket in MB</param>
		/// <returns>The new instance</returns>
		public static BucketCreationDetails PasswordProtectedMemcacheBucket(
			string bucketName,
			string password,
			int size)
		{
			return new BucketCreationDetails
			{
			    AuthType = AuthType.Sasl,
			    BucketName = bucketName,
			    BucketType = BucketType.Memcache,
			    SaslPassword = password,
			    Size = size
			};
		}

		/// <summary>
		/// Creates a membase BucketCreationDetails instance that is SASL password protected.
		/// </summary>
		/// <param name="bucketName">The new bucket name</param>
		/// <param name="password">The bucket password</param>
		/// <param name="size">The size of the bucket in MB</param>
		/// <returns>The new instance</returns>
		public static BucketCreationDetails PasswordProtectedMembaseBucket(
			string bucketName,
			string password,
			int size)
		{
			return new BucketCreationDetails
			{
				AuthType = AuthType.Sasl,
				BucketName = bucketName,
				BucketType = BucketType.Membase,
				SaslPassword = password,
				Size = size
			};
		}

		/// <summary>
		/// Creates a membase BucketCreationDetails instance that is unprotected on 
		/// a specific port.
		/// </summary>
		/// <param name="bucketName">The new bucket name</param>
		/// <param name="port">The bucket port</param>
		/// <param name="size">The size of the bucket in MB</param>
		/// <returns>The new instance</returns>
		public static BucketCreationDetails PortBasedMembaseBucket(
			string bucketName,
			int port,
			int size)
		{
			return new BucketCreationDetails
			{
				AuthType = AuthType.None,
				BucketName = bucketName,
				BucketType = BucketType.Membase,
				ProxyPort = port,
				Size = size
			};
		}

		/// <summary>
		/// Creates a memcache BucketCreationDetails instance that is unprotected on 
		/// a specific port.
		/// </summary>
		/// <param name="bucketName">The new bucket name</param>
		/// <param name="port">The bucket port</param>
		/// <param name="size">The size of the bucket in MB</param>
		/// <returns>The new instance</returns>
		public static BucketCreationDetails PortBasedMemcacheBucket(
			string bucketName,
			int port,
			int size)
		{
			return new BucketCreationDetails
			{
				AuthType = AuthType.None,
				BucketName = bucketName,
				BucketType = BucketType.Memcache,
				ProxyPort = port,
				Size = size
			};
		}
	}

	public enum AuthType
	{
		None,
		Sasl
	}

	public enum BucketType
	{
		Memcache,
		Membase
	}
}
