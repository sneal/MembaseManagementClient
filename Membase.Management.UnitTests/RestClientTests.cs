using System;
using System.Collections.Generic;
using Membase.Management.Impl;
using NUnit.Framework;

namespace Membase.Management.UnitTests
{
	[TestFixture]
	public class RestClientTests
	{
		private Uri _endpoint;
		private RestClient _restClient;

		[SetUp]
		public void SetUp()
		{
			_endpoint = new Uri("http://localhost:8090");
			_restClient = new RestClient(_endpoint);
		}

		[Test]
		public void Ctor_throws_when_uri_is_null()
		{
			Assert.Throws<ArgumentNullException>(() => new RestClient(null));
		}

		[Test]
		public void Get_throws_when_resourcePath_is_null()
		{
			Assert.Throws<ArgumentNullException>(() => _restClient.Get(null));
		}

		[Test]
		public void Delete_throws_when_resourcePath_is_null()
		{
			Assert.Throws<ArgumentNullException>(() => _restClient.Delete(null));
		}

		[Test]
		public void Post_throws_when_resourcePath_is_null()
		{
			Assert.Throws<ArgumentNullException>(() => _restClient.Post(null, new Dictionary<string, string>()));
		}

		[Test]
		public void Endpoint_returns_the_management_uri()
		{
			Assert.AreEqual(_endpoint, _restClient.Endpoint);
		}
	}
}
