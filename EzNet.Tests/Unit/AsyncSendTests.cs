using EzNet.Tests.Models;
using EzNet.Transports.Extensions;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace EzNet.Tests.Unit;

[TestClass]
public class AsyncSendTests
{
	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	[DoNotParallelize]
	public async Task AwaitAsyncSend(bool reliable)
	{
		EndPoint ep = SocketExtensions.GetEndPoint(5010, AddressFamily.InterNetwork);
		using Server server = ConnectionFactory.BuildServer(ep, reliable);
		using Connection client = ConnectionFactory.BuildClient(ep, reliable);
		Assert.IsTrue(client.IsConnected);
		
		var sentResponse = new TestValues();
		TestValues Response(TestPacket packet)
		{
			return sentResponse;
		}
		
		server.RegisterResponseHandler<TestValues, TestPacket>(Response);

		Stopwatch sw = new Stopwatch();
		sw.Start();
		var receivedResponse = await client.SendAsync<TestValues, TestPacket>(new TestPacket("Sup", 213.3123));
		sw.Stop();
		Assert.AreEqual(sentResponse, receivedResponse);
		
		Console.WriteLine("Total time: {0}ms", sw.ElapsedMilliseconds);
	}
	
	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	[DoNotParallelize]
	public async Task ParallelAsyncSend(bool reliable)
	{
		EndPoint ep = SocketExtensions.GetEndPoint(5011, AddressFamily.InterNetwork);
		using Server server = ConnectionFactory.BuildServer(ep, reliable);
		using Connection client = ConnectionFactory.BuildClient(ep, reliable);
		Assert.IsTrue(client.IsConnected);
		
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
			requests.Add(client.SendAsync<TestValues, TestPacket>(new TestPacket($"Yuuuh", i), 5000));
		}
		TestValues[] results = await Task.WhenAll(requests.ToArray());
		
		sw.Stop();
		foreach (TestValues tv in results)
		{
			Assert.IsNotNull(tv);
			Assert.IsTrue(server_responses.Any(t => tv.Equals(t)));
		}
		Console.WriteLine("Total time: {0}ms", sw.ElapsedMilliseconds);
	}
}
