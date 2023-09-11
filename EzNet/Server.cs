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
	public class Server : IDisposable
	{
		#region Events

		public Action<int> OnConnectionAdded;

		#endregion
		private readonly IMessageHandler MessageHandler = Messaging.Handling.MessageHandler.Build();
		private readonly ConcurrentDictionary<int, Connection> _connections = new ConcurrentDictionary<int, Connection>();
		private readonly ITransportServer _server;

		public Server(ITransportServer server)
		{
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
		/// Register a function to handle a packet type
		/// </summary>
		/// <param name="callback"></param>
		/// <typeparam name="TPacket"></typeparam>
		public void RegisterMessageHandler<TPacket>(Action<TPacket, Connection> callback) 
			where TPacket : BasePacket, new()
		{
			MessageHandler.AddCallback<TPacket>((n) =>
			{
				callback?.Invoke(n.Message, (Connection)n.Source);
			});
		}
		
		/// <summary>
		/// Register a function to handle requests that require a response
		/// </summary>
		/// <param name="callback"></param>
		/// <typeparam name="TResponse"></typeparam>
		/// <typeparam name="TRequest"></typeparam>
		public void RegisterResponseHandler<TResponse, TRequest>(Func<TRequest, TResponse> callback) 
			where TResponse : BasePacket, new()
			where TRequest : BasePacket, new()
		{
			MessageHandler.RegisterRequest(callback);
		}

		/// <summary>
		/// Send packet to all connections
		/// </summary>
		/// <param name="packet"></param>
		/// <typeparam name="T"></typeparam>
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
		
		public void Dispose()
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
