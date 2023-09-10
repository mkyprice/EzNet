using EzNet.Messaging;
using EzNet.Messaging.Extensions;
using EzNet.Messaging.Handling;
using EzNet.Messaging.Handling.Abstractions;
using EzNet.Transports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace EzNet
{
	public class Server : IDisposable
	{
		public readonly IMessageHandler MessageHandler = new MessageHandler();
		private readonly HashSet<Connection> _connections = new HashSet<Connection>();
		private readonly ITransportServer _server;

		public Server(ITransportServer server)
		{
			_server = server;
			_server.OnNewConnection += OnNewConnection;
		}
		private void OnNewConnection(ITransportConnection obj)
		{
			_connections.Add(new Connection(obj, MessageHandler));
		}

		public void RegisterMessageHandler<TPacket>(Action<TPacket, Connection> callback) 
			where TPacket : BasePacket, new()
		{
			MessageHandler.AddCallback<TPacket>((n) =>
			{
				callback?.Invoke(n.Message, (Connection)n.Source);
			});
		}
		
		public void RegisterResponseHandler<TPacket, TResponse>(Func<TPacket, TResponse> callback) 
			where TPacket : BasePacket, new()
			where TResponse : BasePacket, new()
		{
			MessageHandler.RegisterRequest(callback);
		}

		public void Broadcast<T>(T packet)
			where T : BasePacket, new()
		{
			using (MemoryStream ms = new MemoryStream())
			{
				PacketExtension.Serialize(ms, packet);
				byte[] bytes = ms.ToArray();
				_server.Send(bytes);
			}
		}

		private void Shutdown()
		{
			foreach (Connection connection in _connections)
			{
				connection.Dispose();
			}
			_connections.Clear();
			_server.Shutdown();
		}
		
		public void Dispose()
		{
			Shutdown();
		}
	}
}
