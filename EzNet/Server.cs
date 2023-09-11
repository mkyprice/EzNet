using EzNet.Messaging;
using EzNet.Messaging.Extensions;
using EzNet.Messaging.Handling;
using EzNet.Messaging.Handling.Abstractions;
using EzNet.Transports;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace EzNet
{
	public class Server : Network
	{
		#region Events

		public Action<int> OnConnectionAdded;

		#endregion
		private readonly ConcurrentDictionary<int, Connection> _connections = new ConcurrentDictionary<int, Connection>();
		private readonly ITransportServer _server;

		public Server(ITransportServer server)
		{
			MessageHandler = Messaging.Handling.MessageHandler.Build();
			_server = server;
			_server.OnNewConnection += OnNewConnection;
		}
		
		public override void Send<T>(T packet) => Broadcast(packet);

		/// <summary>
		/// Try to get a connection by Id
		/// </summary>
		/// <param name="id"></param>
		/// <param name="connection"></param>
		/// <returns></returns>
		public bool TryGetConnection(int id, out Connection connection) => _connections.TryGetValue(id, out connection);


		/// <summary>
		/// Send packet to all connections
		/// </summary>
		/// <param name="packet"></param>
		/// <typeparam name="T"></typeparam>
		public void Broadcast<T>(T packet)
			where T : BasePacket
		{
			using (MemoryStream? ms = new MemoryStream())
			{
				PacketExtension.Serialize(ms, packet);
				byte[] bytes = ms.ToArray();
				_server.Send(bytes);
			}
		}
		
		/// <summary>
		/// Shutdown all connections and listener
		/// </summary>
		public void Shutdown()
		{
			foreach (Connection connection in _connections.Values)
			{
				connection.Dispose();
			}
			_connections.Clear();
			_server.Shutdown();
		}
		
		public override void Dispose()
		{
			Shutdown();
		}

		private void OnNewConnection(ITransportConnection obj)
		{
			var connection = new Connection(obj, MessageHandler);
			_connections[connection.Id] = connection;
			OnConnectionAdded?.Invoke(connection.Id);
		}
	}
}
