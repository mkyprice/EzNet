using EzNet.Messaging;
using EzNet.Messaging.Extensions;
using EzNet.Messaging.Handling;
using EzNet.Messaging.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace EzNet.Tcp
{
	public class TcpServer : IDisposable
	{
		public readonly MessageHandler MessageHandler = new MessageHandler();
		private readonly Socket Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		private readonly RequestHandler _requestHandler;
		private readonly List<PacketConnection> _connections = new List<PacketConnection>();

		public TcpServer()
		{
			_requestHandler = new RequestHandler(MessageHandler);
		}

		public void Listen(EndPoint endPoint)
		{
			Listener.Bind(endPoint);
			Listener.Listen(100);

			Listener.BeginAccept(OnConnection, Listener);
		}

		public void RegisterMessageHandler<TPacket>(Action<TPacket, PacketConnection> callback) 
			where TPacket : BasePacket, new()
		{
			MessageHandler.AddCallback<TPacket>((n) =>
			{
				callback?.Invoke(n.Message, (PacketConnection)n.Source);
			});
		}
		
		public void RegisterResponseHandler<TPacket, TResponse>(Func<TPacket, TResponse> callback) 
			where TPacket : BasePacket, new()
			where TResponse : BasePacket, new()
		{
			_requestHandler.RegisterRequest(callback);
		}

		public void Broadcast<T>(T packet)
			where T : BasePacket
		{
			using (MemoryStream ms = new MemoryStream())
			{
				PacketExtension.Serialize(ms, packet);
				byte[] bytes = ms.ToArray();
				foreach (PacketConnection connection in _connections)
				{
					connection.Send(bytes);
				}
			}
		}

		private void OnConnection(IAsyncResult result)
		{
			Socket socket = ((Socket)result.AsyncState).EndAccept(result);
			PacketConnection connection = new PacketConnection(socket, MessageHandler, _requestHandler);
			_connections.Add(connection);
			
			Listener.BeginAccept(OnConnection, Listener);
		}

		private void Shutdown()
		{
			if (Listener.Connected == false)
			{
				return;
			}
			Listener.Shutdown(SocketShutdown.Both);
		}
		
		public void Dispose()
		{
			foreach (var connection in _connections)
			{
				connection.Dispose();
			}
			_connections.Clear();
			Shutdown();
		}
	}
}
