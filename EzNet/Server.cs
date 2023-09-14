using EzNet.Messaging;
using EzNet.Messaging.Extensions;
using EzNet.Transports;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace EzNet
{
	public class Server : Network
	{
		#region Events

		public Action<int> OnConnectionAdded;
		public Action<int> OnConnectionRemoved;

		#endregion
		
		private readonly ConcurrentDictionary<int, Connection> _connections = new ConcurrentDictionary<int, Connection>();
		private readonly ITransportServer _server;

		private Server() { }
		internal Server(ITransportServer server)
		{
			MessageHandler = Messaging.Handling.MessageHandler.Build();
			_server = server;
			_server.OnNewConnection += OnNewConnection;
		}

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
		public bool Broadcast<T>(T packet)
			where T : BasePacket
		{
			using (MemoryStream? ms = new MemoryStream())
			{
				PacketExtension.Serialize(ms, packet);
				byte[] bytes = ms.ToArray();
				return _server.Send(bytes);
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

		#region Callbacks

		/// <summary>
		/// New connection callback
		/// </summary>
		/// <param name="obj"></param>
		private void OnNewConnection(ITransportConnection obj)
		{
			Connection connection = new Connection(obj, MessageHandler);
			connection.OnEndConnection += OnEndConnection;
			_connections[connection.Id] = connection;
			OnConnectionAdded?.Invoke(connection.Id);
		}

		/// <summary>
		/// On disconnect
		/// </summary>
		/// <param name="obj"></param>
		private void OnEndConnection(Connection obj) => OnConnectionRemoved?.Invoke(obj.Id);

		#endregion
	}
}
