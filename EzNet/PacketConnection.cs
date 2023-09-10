﻿using EzNet.Messaging;
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
		
		public PacketConnection() : this(new DefaultTcpConnection())
		{
		}

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
			PacketSerializerExtension.Serialize(ms, packet);
			byte[] bytes = ms.ToArray();
			Connection.Send(bytes);
		}

		public void Send(byte[] packetBytes) => Connection.Send(packetBytes);

		public async Task<TResponse> SendAsync<TResponse, T>(T packet, int timeoutMS = 2000)
			where T : BasePacket, new()
			where TResponse : BasePacket, new()
			=> await RequestHandler.SendAsync<T, TResponse>(packet, Send, timeoutMS);

		public void Receive(ArraySegment<byte> bytes) => MessageHandler.ReadPackets(bytes, this);
		
		public void Dispose()
		{
			Connection.Shutdown();
			MessageHandler.Dispose();
		}
	}
}
