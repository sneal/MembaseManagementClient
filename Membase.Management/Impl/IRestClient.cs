using System;
using System.Collections.Generic;

namespace Membase.Management.Impl
{
	/// <summary>
	/// Generic interface for interacting with remote REST based services.
	/// </summary>
	public interface IRestClient
	{
		/// <summary>
		/// Perform a GET on the remote service.
		/// </summary>
		/// <param name="resourcePath">The relative resource path</param>
		/// <returns>The returned resource</returns>
		/// <exception cref="MembaseManagementConnectionException">
		/// Thrown when unable to connect to the remote Membase managment server.
		/// </exception>
		RestResponse Get(string resourcePath);

		/// <summary>
		/// Perform a POST to the remote service using the specified data parameters.
		/// </summary>
		/// <param name="resourceUri">The relative resource path</param>
		/// <param name="parameters">The form body values</param>
		/// <exception cref="MembaseManagementConnectionException">
		/// Thrown when unable to connect to the remote Membase managment server.
		/// </exception>
		RestResponse Post(string resourceUri, IDictionary<string, string> parameters);

		/// <summary>
		/// Perform a DELETE on the remote service uri.
		/// </summary>
		/// <param name="endpoint">The relative resource path to delete</param>
		/// <exception cref="MembaseManagementConnectionException">
		/// Thrown when unable to connect to the remote Membase managment server.
		/// </exception>
		RestResponse Delete(string endpoint);

		/// <summary>
		/// The REST base uri endpoint
		/// </summary>
		Uri Endpoint { get; }
	}
}
