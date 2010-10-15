using System;
using Membase.Management.Impl;
using Membase.Management.Resources;

namespace Membase.Management
{
	/// <summary>
	/// Represents a thread safe client instance.
	/// </summary>
	public interface IMembaseManagementClient
	{
		/// <summary>
		/// Gets the default pool information. Good for checking server connectivity.
		/// </summary>
		/// <exception cref="MembaseManagementException">
		/// A connection error occurred or the server version is not compatible.
		/// </exception>
		Pool GetDefaultPool();

		/// <summary>
		/// Get the specified bucket by name if it exists in the default pool.
		/// </summary>
		/// <param name="bucketName">The name of the bucket</param>
		/// <returns>The bucket details</returns>
		/// <exception cref="MembaseManagementException">
		/// A connection error occurred or the bucket does not exist.
		/// </exception>
		Bucket GetBucket(string bucketName);

		/// <summary>
		/// Deletes the specified bucket.
		/// </summary>
		/// <param name="bucketName"></param>
		/// <exception cref="MembaseManagementRestException">
		/// The bucket does not exist.
		/// </exception>
		/// <exception cref="MembaseManagementException">
		/// An error occurred.
		/// </exception>
		void DeleteBucket(string bucketName);
		
		/// <summary>
		/// Checks to see if the specified bucket exists in the default pool.
		/// </summary>
		/// <param name="bucketName">The bucket name</param>
		/// <returns>True if the bucket exists, otherwise false</returns>
		/// <exception cref="MembaseManagementException">
		/// An error occurred.
		/// </exception>
		bool BucketExists(string bucketName);

		/// <summary>
		/// Create a new memcache or membase bucket in the default pool.
		/// </summary>
		/// <param name="bucketCreationDetails">The bucket creation info</param>
		/// <exception cref="MembaseManagementException">
		/// An error occurred.
		/// </exception>
		void CreateBucket(BucketCreationDetails bucketCreationDetails);

		/// <summary>
		/// Flushes all values from the memcache or membase bucket.
		/// </summary>
		/// <param name="bucketName">The name of the bucket to flush</param>
		void FlushBucket(string bucketName);

		/// <summary>
		/// The Membase management endpoint Uri.
		/// </summary>
		Uri Endpoint { get; }
	}
}