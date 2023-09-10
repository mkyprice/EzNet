using EzNet.Messaging;
using EzNet.Messaging.Extensions;
using EzNet.Messaging.Handling;
using EzNet.Messaging.Requests;
using EzNet.Tcp;
using System.Net.Sockets;

namespace EzNet
{
	public class PacketConnection : IDisposable
	{
		protected readonly MessageHandler MessageHandler;
		protected readonly RequestHandler RequestHandler;
		protected readonly IConnection Connection;

		public PacketConnection(IConnection connection)
		{
			MessageHandler = new MessageHandler();
			RequestHandler = new RequestHandler(MessageHandler);
			Connection = connection;
			Connection.OnReceive += Receive;
		}

		public PacketConnection(Socket socket, MessageHandler handler, RequestHandler requestHandler)
		{
			MessageHandler = handler;
			RequestHandler = requestHandler;
			Connection = new DefaultTcpConnection(socket);
			Connection.OnReceive += Receive;
		}
		
		public void Send<T>(T packet)
			where T : BasePacket
		{
			using MemoryStream ms = new MemoryStream();
			PacketExtension.Serialize(ms, packet);
			byte[] bytes = ms.ToArray();
			Connection.Send(bytes);
		}

		public void Send(byte[] packetBytes) => Connection.Send(packetBytes);

		public async Task<TResponse> SendAsync<TResponse, TRequest>(TRequest packet, int timeoutMS = 2000)
			where TRequest : BasePacket, new()
			where TResponse : BasePacket, new()
			=> await RequestHandler.SendAsync<TResponse, TRequest>(packet, Send, timeoutMS);

		public void Receive(ArraySegment<byte> bytes) => MessageHandler.ReadPackets(bytes, this);
		
		public void Dispose()
		{
			Connection.Shutdown();
			MessageHandler.Dispose();
		}
	}
}
