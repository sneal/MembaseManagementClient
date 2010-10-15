using System;
using System.Collections.Generic;
using System.Linq;
using Membase.Management.Impl;

namespace Membase.Management
{
	/// <summary>
	/// Factory for creating IMembaseManagementClient instances.
	/// </summary>
	public static class MembaseManagementClientFactory
	{
		public static readonly Uri DefaultManagementUri = new Uri("http://localhost:8091");

		/// <summary>
		/// Creates a single client that does not support a pool. The endpoint is defaulted to
		/// the DefaultManagementUri on the localhost.
		/// </summary>
		/// <param name="adminUserName">The admin user name</param>
		/// <param name="adminPassword">The admin password</param>
		/// <returns>The client instance</returns>
		public static IMembaseManagementClient Create(string adminUserName, string adminPassword)
		{
			return CreateNonPooledClient(DefaultManagementUri, adminUserName, adminPassword);
		}

		/// <summary>
		/// Creates a single client that does not support a pool.
		/// </summary>
		/// <param name="adminUserName">The admin user name</param>
		/// <param name="adminPassword">The admin password</param>
		/// <param name="managementEndpoint">
		/// The management address, for example: http://localhost:8091
		/// </param>
		/// <returns>The client instance</returns>
		public static IMembaseManagementClient Create(
			string adminUserName,
			string adminPassword,
			Uri managementEndpoint)
		{
			return CreateNonPooledClient(managementEndpoint, adminUserName, adminPassword);
		}

		/// <summary>
		/// Creates a pooled client that does supports failover to a list of endpoints in a pool.
		/// </summary>
		/// <param name="adminUserName">The admin user name</param>
		/// <param name="adminPassword">The admin password</param>
		/// <param name="managementEndpoints">
		/// The Membase management addresses in priority order.
		/// </param>
		/// <returns>The pooled client instance</returns>
		public static IMembaseManagementClient Create(
			string adminUserName,
			string adminPassword,
			params Uri[] managementEndpoints)
		{
			return CreatePooledClient(managementEndpoints, adminUserName, adminPassword);
		}

		/// <summary>
		/// Creates a pooled client that does supports failover to a list of endpoints in a pool.
		/// </summary>
		/// <param name="adminUserName">The admin user name</param>
		/// <param name="adminPassword">The admin password</param>
		/// <param name="managementEndpoints">
		/// The Membase management addresses in priority order.
		/// </param>
		/// <returns>The pooled client instance</returns>
		public static IMembaseManagementClient Create(
			string adminUserName,
			string adminPassword,
			IEnumerable<Uri> managementEndpoints)
		{
			return CreatePooledClient(managementEndpoints, adminUserName, adminPassword);
		}

		private static IMembaseManagementClient CreatePooledClient(
			IEnumerable<Uri> managementEndpoints,
			string adminUserName,
			string adminPassword)
		{
			var clientPool = new ManagementClientPool(managementEndpoints.Select(
				endpoint => CreateNonPooledClient(endpoint, adminUserName, adminPassword)));

			return new PooledManagementClient(clientPool);
		}

		private static IMembaseManagementClient CreateNonPooledClient(
			Uri managementEndpoint,
			string adminUserName,
			string adminPassword)
		{
			// trim off any excess path data
			var uriBuilder = new UriBuilder
			{
			    Scheme = managementEndpoint.Scheme,
			    Host = managementEndpoint.Host,
			    Port = managementEndpoint.Port
			};

			var restClient = new RestClient(uriBuilder.Uri, adminUserName, adminPassword);
			return new ManagementClient(restClient);
		}
	}
}