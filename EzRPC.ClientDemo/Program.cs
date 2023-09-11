using System.Net;

namespace EzRPC.ClientDemo;

[Synced]
class Program
{
	static async Task Main(string[] args)
	{
		var tcpEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6969);
		var udpEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6970);
		// RpcDemoInstance demo = new RpcDemoInstance();
		using RpcServer server = new RpcServer();
		using RpcClient client = new RpcClient();
		server.Start(tcpEndpoint, udpEndpoint);
		client.Start(tcpEndpoint, udpEndpoint);

		object result = await client.CallAsync<Program>("TestAdd", 1, 2);
		Console.WriteLine("Received: {0}", result);
		// var v = (TestValueClass) await client.CallAsync(demo, "DemoTest", new TestValueClass());
		// Console.WriteLine(v.ToString());
	}

	static float TestAdd(float a, float b)
	{
		return a + b;
	}
}