using EzNet.Messaging;
using EzNet.Messaging.Extensions;
using EzNet.Messaging.Handling.Abstractions;
using EzNet.Transports;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EzNet
{
	public class Connection : Network
	{
		public Action<Connection> OnEndConnection;
		
		private readonly ITransportConnection _connection;
		private Action<ArraySegment<byte>, Connection> OnBytesReceived;
		
		
		internal Connection(ITransportConnection connection)
		{
			MessageHandler = EzNet.Messaging.Handling.MessageHandler.Build();
			_connection = connection;
			_connection.OnReceive += ReceiveRaw;
			_connection.OnDisconnect += OnDisconnect;
			MessageHandler.Streamer.RegisterByteHandler(ref OnBytesReceived);
		}

		internal Connection(ITransportConnection connection, IMessageHandler handler)
		{
			MessageHandler = handler;
			_connection = connection;
			_connection.OnReceive += ReceiveRaw;
			MessageHandler.Streamer.RegisterByteHandler(ref OnBytesReceived);
		}

		/// <summary>
		/// Send a packet async that expects a response
		/// </summary>
		/// <param name="packet"></param>
		/// <param name="timeoutMs"></param>
		/// <typeparam name="TResponse"></typeparam>
		/// <typeparam name="TRequest"></typeparam>
		/// <returns></returns>
		public async Task<TResponse> SendAsync<TResponse, TRequest>(TRequest packet, int timeoutMs = 2000)
			where TRequest : BasePacket, new()
			=> await MessageHandler.SendAsync<TResponse, TRequest>(packet, Send, timeoutMs);
		
		/// <summary>
		/// Send a packet though connection
		/// </summary>
		/// <param name="packet"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public bool Send<T>(T packet)
			where T : BasePacket
		{
			using (MemoryStream ms = new MemoryStream())
			{
				PacketExtension.Serialize(ms, packet);
				byte[] bytes = ms.ToArray();
				return _connection.Send(bytes);
			}
		}

		public override void Dispose()
		{
			_connection.Shutdown();
			MessageHandler.Dispose();
		}

		#region Callbacks
		
		private void ReceiveRaw(ArraySegment<byte> bytes, ITransportConnection connection) => OnBytesReceived?.Invoke(bytes, this);
		
		private void OnDisconnect(ITransportConnection obj) => OnEndConnection?.Invoke(this);

		#endregion
	}
}
