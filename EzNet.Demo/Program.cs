
using EzNet.Demo;
using EzNet.Messaging;
using EzNet.Tcp;
using System.Diagnostics;
using System.Net;

class Program
{
	static async Task Main(string[] args)
	{
		var endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6969);
		
		using var server = new TcpServer();
		server.Listen(endpoint);

		using var client = new TcpClient();
		client.Connect(endpoint);

		Stopwatch sw = new Stopwatch();
		sw.Start();
		// client.RegisterMessageHandler<DemoPacket>((p, c) =>
		// {
		// 	Console.WriteLine(p.ToString());
		// });
		//
		// client.RegisterMessageHandler<TestPacket>((p, c) =>
		// {
		// 	Console.WriteLine(p.ToString());
		// });
		
		server.RegisterResponseHandler<DemoPacket, TestPacket>((packet) =>
		{
			// Console.WriteLine("Server received {0}", packet);
			return new TestPacket(packet.Text, 111);
		});
		
		// client.RegisterResponseHandler((TestPacket request) => new DemoPacket("Sup"));

		// client.Send(new DemoPacket("test"));

		List<Task<TestPacket>> requests = new List<Task<TestPacket>>();
		for (int i = 0; i < 100; i++)
		{
			requests.Add(client.SendAsync<TestPacket, DemoPacket>(new DemoPacket($"Yoooo{i}"), 2000));
			// Console.WriteLine("Send async result: {0}", packet);
		}
		
		var results = await Task.WhenAll(requests.ToArray());
		foreach (TestPacket result in results)
		{
			Console.WriteLine("Result: {0}", result);
		}

		// List<Task> tasks = new List<Task>();
		// tasks.Add(SendPacketsLoop(server));
		// // tasks.Add(ReadPacketsLoop(client));
		//
		// await Task.WhenAll(tasks.ToArray());
		
		sw.Stop();
		Console.WriteLine("Total time {0}", sw.Elapsed);
	}
}