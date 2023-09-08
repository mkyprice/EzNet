using System.Net;

namespace EzRPC.ClientDemo;

[Synced]
class Program
{
	static async Task Main(string[] args)
	{
		RpcDemoInstance demo = new RpcDemoInstance();
		using RpcClient client = new RpcClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6969));

		float result = (float)client.CallAsync<Program>("TestAdd", 1, 2).Result;
		Console.WriteLine(result);
		var v = (TestValueClass) await client.CallAsync(demo, "DemoTest", new TestValueClass());
		Console.WriteLine(v.ToString());
	}

	static float TestAdd(float a, float b)
	{
		return a + b;
	}
}