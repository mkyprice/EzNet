
using EzNet;
using EzNet.Demo;
using EzNet.Transports.Extensions;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

class Program
{
	static async Task Main(string[] args)
	{
		await ParallelTests();
	}

	private static async Task DemoTests()
	{
		EndPoint ep = SocketExtensions.GetEndPoint(9696, AddressFamily.InterNetwork);
		using var server = ConnectionFactory.BuildServer(ep, false);
		using var client = ConnectionFactory.BuildClient(ep, false);
		
		Stopwatch sw = new Stopwatch();
		sw.Start();
		
		server.RegisterResponseHandler<TestPacket, DemoPacket>(ServerResponse);
		server.RegisterResponseHandler<TestValueClass, TestPacket>(ServerResponse);
		
		var packet = await client.SendAsync<TestPacket, DemoPacket>(new DemoPacket($"DemoPacketReq"));
		Console.WriteLine("Received packet {0}", packet);
		
		sw.Stop();
		Console.WriteLine("Total time {0}", sw.Elapsed);
	}

	private static async Task ParallelTests()
	{
		EndPoint ep = SocketExtensions.GetEndPoint(9696, AddressFamily.InterNetwork);
		using var server = ConnectionFactory.BuildServer(ep, true);
		using var client = ConnectionFactory.BuildClient(ep, true);

		int count = 100;
		TestValueClass[] server_responses = new TestValueClass[count];
		for (int i = 0; i < server_responses.Length; i++) server_responses[i] = new TestValueClass();

		int responseIndex = 0;
		server.RegisterResponseHandler<TestValueClass, TestPacket>((r) =>
		{
			TestValueClass response = server_responses[responseIndex];
			Interlocked.Increment(ref responseIndex);
			throw new Exception("Oopsies");
			return response;
		});
		
		Stopwatch sw = new Stopwatch();
		sw.Start();
		
		List<Task<TestValueClass>> requests = new List<Task<TestValueClass>>();
		for (int i = 0; i < count; i++)
		{
			requests.Add(client.SendAsync<TestValueClass, TestPacket>(new TestPacket($"Yuuuh", i), -1));
		}
		
		var results = await Task.WhenAll(requests.ToArray());
		
		sw.Stop();
		int total_successes = 0;
		for (int i = 0; i < count; i++)
		{
			var result = results[i];
			var sent = server_responses[i];
			if (result?.Equals(sent) == true) total_successes++;
			else Console.WriteLine("Failed {0} vs {1}", result, sent);
		}
		Console.WriteLine("All responses {0}", total_successes == count ? "successfully validated" : "failed to validate");
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

	private static TestValueClass ClientResponse(DemoPacket request)
	{
		var test = new TestValueClass();
		// Console.WriteLine("Sending:  {0}", test);
		return test;
	}
}