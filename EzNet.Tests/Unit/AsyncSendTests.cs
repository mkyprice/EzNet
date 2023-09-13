using EzNet.Tests.Models;
using EzNet.Transports.Extensions;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace EzNet.Tests.Unit;

[TestClass]
public class AsyncSendTests
{
	[TestMethod]
	public async Task AwaitAsyncSend()
	{
		EndPoint ep = SocketExtensions.GetEndPoint(9696, AddressFamily.InterNetwork);
		using var server = ConnectionFactory.BuildServer(ep, false);
		using var client = ConnectionFactory.BuildClient(ep, false);
		
		var sent_response = new TestValues();
		TestValues Response(TestPacket packet)
		{
			return sent_response;
		}
		
		server.RegisterResponseHandler<TestValues, TestPacket>(Response);
		
		var received_response = await client.SendAsync<TestValues, TestPacket>(new TestPacket("Sup", 213.3123));

		Assert.AreEqual(sent_response, received_response);
	}
	
	[TestMethod]
	public async Task ParallelAsyncSend()
	{
		EndPoint ep = SocketExtensions.GetEndPoint(9697, AddressFamily.InterNetwork);
		using var server = ConnectionFactory.BuildServer(ep, true);
		using var client = ConnectionFactory.BuildClient(ep, true);
		
		
		int count = 100;
		TestValues[] server_responses = new TestValues[count];
		for (int i = 0; i < server_responses.Length; i++) server_responses[i] = new TestValues();

		int responseIndex = 0;
		server.RegisterResponseHandler<TestValues, TestPacket>((r) =>
		{
			TestValues response = server_responses[responseIndex];
			Interlocked.Increment(ref responseIndex);
			return response;
		});
		
		Stopwatch sw = new Stopwatch();
		sw.Start();
		
		List<Task<TestValues>> requests = new List<Task<TestValues>>();
		for (int i = 0; i < count; i++)
		{
			requests.Add(client.SendAsync<TestValues, TestPacket>(new TestPacket($"Yuuuh", i)));
		}
		
		var results = await Task.WhenAll(requests.ToArray());
		
		sw.Stop();
		int total_successes = 0;
		for (int i = 0; i < count; i++)
		{
			var result = results[i];
			var sent = server_responses[i];
			Assert.AreEqual(result, sent);
		}
		Console.WriteLine("Total time: {0}ms", sw.ElapsedMilliseconds);
	}
}
