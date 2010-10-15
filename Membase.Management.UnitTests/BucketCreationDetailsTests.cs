using NUnit.Framework;

namespace Membase.Management.UnitTests
{
	[TestFixture]
	public class BucketCreationDetailsTests
	{
		[Test]
		public void PasswordProtectedMemcacheBucket_uses_SASL()
		{
			var detail = BucketCreationDetails.PasswordProtectedMemcacheBucket("b", 64);
			Assert.AreEqual(AuthType.Sasl, detail.AuthType);
		}

		[Test]
		public void PasswordProtectedMemcacheBucket_proxy_port_is_zero()
		{
			var detail = BucketCreationDetails.PasswordProtectedMemcacheBucket("b", 64);
			Assert.AreEqual(0, detail.ProxyPort);
		}

		[Test]
		public void PasswordProtectedMemcacheBucket_bucket_name()
		{
			var detail = BucketCreationDetails.PasswordProtectedMemcacheBucket("bucketname", 64);
			Assert.AreEqual("bucketname", detail.BucketName);
		}

		[Test]
		public void PasswordProtectedMemcacheBucket_bucket_size()
		{
			var detail = BucketCreationDetails.PasswordProtectedMemcacheBucket("b", 64);
			Assert.AreEqual(64, detail.Size);
		}

		[Test]
		public void PasswordProtectedMemcacheBucket_bucket_type_is_memcache()
		{
			var detail = BucketCreationDetails.PasswordProtectedMemcacheBucket("b", 64);
			Assert.AreEqual(BucketType.Memcache, detail.BucketType);
		}
	}
}
