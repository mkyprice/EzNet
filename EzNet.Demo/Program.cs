
using EzNet.Demo;
using EzNet.Messaging;
using EzNet.Tcp;
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
			Console.WriteLine("Server received {0}", packet);
			return new TestPacket(packet.Text, 111);
		});

		// client.Send(new DemoPacket("test"));
		for (int i = 0; i < 100; i++)
		{
			TestPacket packet = await client.SendAsync<TestPacket, DemoPacket>(new DemoPacket($"Yoooo{i}"));
			Console.WriteLine("Send async result: {0}", packet);
		}
		
		// List<Task> tasks = new List<Task>();
		// tasks.Add(SendPacketsLoop(server));
		// // tasks.Add(ReadPacketsLoop(client));
		//
		// await Task.WhenAll(tasks.ToArray());
	}
}