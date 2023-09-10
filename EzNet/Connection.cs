using EzNet.Messaging;
using EzNet.Messaging.Extensions;
using EzNet.Messaging.Handling;
using EzNet.Messaging.Handling.Abstractions;
using EzNet.Transports;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EzNet
{
	public class Connection : IDisposable
	{
		public readonly int Id;
		protected readonly IMessageHandler MessageHandler;
		protected readonly ITransportConnection RawConnection;
		private Action<ArraySegment<byte>, Connection> OnBytesReceived;

		private static volatile int _nextId = 1;

		internal Connection(ITransportConnection rawConnection)
		{
			MessageHandler = EzNet.Messaging.Handling.MessageHandler.Build();
			RawConnection = rawConnection;
			RawConnection.OnReceive += ReceiveRaw;
			MessageHandler.Streamer.RegisterByteHandler(ref OnBytesReceived);
			Id = _nextId;
			Interlocked.Increment(ref _nextId);
		}

		internal Connection(ITransportConnection rawConnection, IMessageHandler handler)
		{
			MessageHandler = handler;
			RawConnection = rawConnection;
			RawConnection.OnReceive += ReceiveRaw;
			MessageHandler.Streamer.RegisterByteHandler(ref OnBytesReceived);
			Id = _nextId;
			Interlocked.Increment(ref _nextId);
		}
		
		/// <summary>
		/// Send a packet
		/// </summary>
		/// <param name="packet"></param>
		/// <typeparam name="T"></typeparam>
		public void Send<T>(T packet)
			where T : BasePacket
		{
			using (MemoryStream ms = new MemoryStream())
			{
				PacketExtension.Serialize(ms, packet);
				byte[] bytes = ms.ToArray();
				RawConnection.Send(bytes);
			}
		}

		/// <summary>
		/// Send a packet async that expects a response
		/// </summary>
		/// <param name="packet"></param>
		/// <param name="timeoutMS"></param>
		/// <typeparam name="TResponse"></typeparam>
		/// <typeparam name="TRequest"></typeparam>
		/// <returns></returns>
		public async Task<TResponse> SendAsync<TResponse, TRequest>(TRequest packet, int timeoutMS = -1)
			where TRequest : BasePacket, new()
			where TResponse : BasePacket, new()
			=> await MessageHandler.SendAsync<TResponse, TRequest>(packet, Send, timeoutMS);
		

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
		/// <typeparam name="TPacket"></typeparam>
		/// <typeparam name="TResponse"></typeparam>
		public void RegisterResponseHandler<TPacket, TResponse>(Func<TPacket, TResponse> callback) 
			where TPacket : BasePacket, new()
			where TResponse : BasePacket, new()
		{
			MessageHandler.RegisterRequest(callback);
		}
		
		public void Dispose()
		{
			RawConnection.Shutdown();
			MessageHandler.Dispose();
		}
		
		private void ReceiveRaw(ArraySegment<byte> bytes, ITransportConnection connection) => OnBytesReceived?.Invoke(bytes, this);
	}
}
