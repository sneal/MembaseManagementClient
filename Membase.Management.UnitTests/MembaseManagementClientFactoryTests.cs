using System;
using Membase.Management.Impl;
using NUnit.Framework;

namespace Membase.Management.UnitTests
{
	[TestFixture]
	public class MembaseManagementClientFactoryTests
	{
		private readonly Uri _membaseUri1 = new Uri("http://localhost:8090");
		private readonly Uri _membaseUri2 = new Uri("http://localhost:8091");

		private const string Admin = "Administrator";
		private const string Password = "password";

		[Test]
		public void Create_minimal_args()
		{
			var client = MembaseManagementClientFactory.Create(Admin, Password);

			Assert.IsNotNull(client);
			Assert.AreEqual("http://localhost:8091/", client.Endpoint.ToString());
			Assert.IsInstanceOf(typeof(ManagementClient), client);
		}

		[Test]
		public void Create_single_client()
		{
			var client = MembaseManagementClientFactory.Create(
				Admin, Password, _membaseUri1);

			Assert.IsNotNull(client);
			Assert.AreEqual(_membaseUri1, client.Endpoint);
			Assert.IsInstanceOf(typeof(ManagementClient), client);
		}

		[Test]
		public void Create_2_clients()
		{
			var client = MembaseManagementClientFactory.Create(
				Admin, Password, _membaseUri1, _membaseUri2);

			Assert.IsNotNull(client);
			Assert.AreEqual(_membaseUri1, client.Endpoint, "Did not default to first client endpoint");
			Assert.IsInstanceOf(typeof(PooledManagementClient), client);
		}

		[Test]
		public void Create_2_clients_enumerable()
		{
			var uris = new[] {_membaseUri1, _membaseUri2};

			var client = MembaseManagementClientFactory.Create(
				Admin, Password, uris);

			Assert.IsNotNull(client);
			Assert.AreEqual(_membaseUri1, client.Endpoint, "Did not default to first client endpoint");
			Assert.IsInstanceOf(typeof(PooledManagementClient), client);
		}


		[Test]
		public void Create_ignores_everything_past_the_port_in_the_Uri()
		{
			Uri managementUri = new Uri("http://localhost:8091/pools/default");
			var client = MembaseManagementClientFactory.Create(
				Admin, Password, managementUri);

			Assert.AreEqual("http://localhost:8091/", client.Endpoint.ToString());
		}

		[Test]
		public void Create_ignores_everything_past_the_port_in_the_Uri_mutliple()
		{
			Uri managementUri = new Uri("http://localhost:8091/pools/default");
			var client = MembaseManagementClientFactory.Create(
				Admin, Password, managementUri, managementUri);

			Assert.AreEqual("http://localhost:8091/", client.Endpoint.ToString());
		}
	}
}
