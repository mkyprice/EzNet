
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
		
		client.RegisterMessageHandler<DemoPacket>((p, c) =>
		{
			Console.WriteLine(p.ToString());
		});

		List<Task> tasks = new List<Task>();
		tasks.Add(SendPacketsLoop(server));
		// tasks.Add(ReadPacketsLoop(client));

		await Task.WhenAll(tasks.ToArray());


		await Task.Delay(1000);
	}

	static async Task SendPacketsLoop(TcpServer server)
	{
		int i = 100;
		while (i > 0)
		{
			i--;
			server.Broadcast(new DemoPacket(i.ToString()));
			// await Task.Delay(1);
		}
		Console.WriteLine("Finished sending");
	}

	// static async Task ReadPacketsLoop(TcpClient client)
	// {
	// 	while (true)
	// 	{
	// 		BasePacket packet;
	// 		if (client.TryDequeue(out packet))
	// 		{
	// 			DemoPacket d = (DemoPacket)packet;
	// 			Console.WriteLine(d.ToString());
	// 		}
	//
	// 		if (Console.KeyAvailable)
	// 		{
	// 			var key = Console.ReadKey().Key;
	// 			if (key == ConsoleKey.Q) break;
	// 		}
	// 	}
	// }
}