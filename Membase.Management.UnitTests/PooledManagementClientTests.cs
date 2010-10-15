using Membase.Management.Impl;
using Membase.Management.Resources;
using NUnit.Framework;
using Rhino.Mocks;

namespace Membase.Management.UnitTests
{
	[TestFixture]
	public class PooledManagementClientTests
	{
		private IMembaseManagementClient _client1;
		private IMembaseManagementClient _client2;

		private ManagementClientPool _clientPool;
		private PooledManagementClient _pooledClient;

		[SetUp]
		public void SetUp()
		{
			_client1 = MockRepository.GenerateStub<IMembaseManagementClient>();
			_client2 = MockRepository.GenerateStub<IMembaseManagementClient>();

			_clientPool = new ManagementClientPool(new[] { _client1, _client2 });
			_pooledClient = new PooledManagementClient(_clientPool);
		}

		[Test]
		public void Failed_call_goes_to_the_next_client_in_the_pool()
		{
			var expectedPool = new Pool();

			_client1.Stub(o => o.GetDefaultPool()).Throw(new MembaseManagementConnectionException());
			_client2.Stub(o => o.GetDefaultPool()).Return(expectedPool);

			Assert.That(_pooledClient.GetDefaultPool(), Is.EqualTo(expectedPool));
		}

		[Test]
		public void Each_client_in_the_pool_is_tried_only_once_per_call()
		{
			_client1.Stub(o => o.GetDefaultPool()).Throw(new MembaseManagementConnectionException());
			_client2.Stub(o => o.GetDefaultPool()).Throw(new MembaseManagementConnectionException());

			Assert.Throws<MembaseManagementConnectionException>(() => _pooledClient.GetDefaultPool());

			_client1.AssertWasCalled(o => o.GetDefaultPool(), options => options.Repeat.Once());
			_client2.AssertWasCalled(o => o.GetDefaultPool(), options => options.Repeat.Once());
		}
	}
}
