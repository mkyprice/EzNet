using EzNet.Tests.Models;
using EzNet.Tests.Models.Rpc;
using EzNet.Transports.Extensions;
using EzRpc;
using System.Net;

namespace EzNet.Tests.Unit
{
	[TestClass]
	public class RpcSendTests
	{
		[TestMethod]
		public async Task BasicRpcTest()
		{
			EndPoint ep = SocketExtensions.GetEndPoint(8989);
			using RpcServer server = new RpcServer(ConnectionFactory.BuildServer(ep, true), null);
			using RpcClient client = new RpcClient(ConnectionFactory.BuildClient(ep, true), null);

			BasicRpc rpcClass = new BasicRpc();
			server.Bind(rpcClass);
			client.Bind<BasicRpc>();

			float result = await client.CallAsync<float>(typeof(BasicRpc), nameof(BasicRpc.Test), 1, 2);
			Assert.AreEqual(rpcClass.Test(1, 2), result);
		}
		
		
		[TestMethod]
		public async Task ServerToClientCall()
		{
			EndPoint ep = SocketExtensions.GetEndPoint(8990);
			using RpcServer server = new RpcServer(ConnectionFactory.BuildServer(ep, true), null);
			using RpcClient client = new RpcClient(ConnectionFactory.BuildClient(ep, true), null);

			BasicRpc rpcClass = new BasicRpc();
			server.Bind(rpcClass);
			client.Bind<BasicRpc>();

			TestValues sent = new TestValues();
			TestValues result = await client.CallAsync<TestValues>(typeof(BasicRpc), nameof(BasicRpc.TestAdvanced), sent);
			Assert.AreEqual(sent, result);
		}
		
		
		[TestMethod]
		public async Task ClientToServerCall()
		{
			EndPoint ep = SocketExtensions.GetEndPoint(8991);
			using RpcServer server = new RpcServer(ConnectionFactory.BuildServer(ep, true), null);
			using RpcClient client = new RpcClient(ConnectionFactory.BuildClient(ep, true), null);

			BasicRpc rpcClass = new BasicRpc();
			server.Bind<BasicRpc>();
			client.Bind(rpcClass);

			TestValues sent = new TestValues();
			TestValues[] result = await server.CallAsync<TestValues>(typeof(BasicRpc), nameof(BasicRpc.TestAdvanced), sent);
			foreach (TestValues received in result)
			{
				Assert.AreEqual(sent, received);
			}
		}
	}
}
