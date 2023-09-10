
using EzNet;
using EzNet.Demo;
using System.Diagnostics;
using System.Net;

class Program
{
	static async Task Main(string[] args)
	{
		var endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9696);

		using var server = ConnectionFactory.BuildServer(endpoint, true);
		using var client = ConnectionFactory.BuildClient(endpoint, true);
		
		Stopwatch sw = new Stopwatch();
		sw.Start();
		
		server.RegisterResponseHandler<DemoPacket, TestPacket>(ServerResponse);
		server.RegisterResponseHandler<TestPacket, TestValueClass>(ServerResponse);
		
		
		var packet = await client.SendAsync<TestPacket, DemoPacket>(new DemoPacket($"Yoooo"));
		Console.WriteLine("Received: {0}", packet);
		
		List<Task<TestValueClass>> requests = new List<Task<TestValueClass>>();
		for (int i = 0; i < 100; i++)
		{
			requests.Add(client.SendAsync<TestValueClass, TestPacket>(new TestPacket($"Yuuuh", i)));
			// var packet = await client.SendAsync<TestPacket, DemoPacket>(new DemoPacket($"Yoooo{i}"));
			// Console.WriteLine("Send async result: {0}", packet);
		}
		
		var results = await Task.WhenAll(requests.ToArray());
		
		sw.Stop();
		foreach (TestValueClass result in results)
		{
			Console.WriteLine("Result: {0}", result);
		}
		Console.WriteLine("Total time {0}", sw.Elapsed);
	}

	private static TestPacket ServerResponse(DemoPacket request)
	{
		return new TestPacket(request.Text, Random.Shared.NextSingle());
	}

	private static TestValueClass ServerResponse(TestPacket request)
	{
		var test = new TestValueClass();
		// Console.WriteLine("Sending:  {0}", test);
		return test;
	}
}