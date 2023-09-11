using System;
using System.Threading.Tasks;

namespace EzNet.Messaging.Handling.Abstractions
{
	/// <summary>
	/// Handles decoding and encoding of packets
	/// </summary>
	public interface IMessageHandler : IDisposable
	{
		public IMessageContainer Container { get; }
		public IMessageStreamer Streamer { get; }
		/// <summary>
		/// Add a callback to handle a packet type
		/// </summary>
		/// <param name="callback"></param>
		/// <typeparam name="T"></typeparam>
		public void AddCallback<T>(Action<MessageNotification<T>> callback) where T : BasePacket, new();
		
		/// <summary>
		/// Remove a callback to handle a packet type
		/// </summary>
		/// <param name="callback"></param>
		/// <typeparam name="T"></typeparam>
		public void RemoveCallback<T>(Action<MessageNotification<T>> callback) where T : BasePacket, new();
		
		/// <summary>
		/// Register a function to handle a packet request with a packet response
		/// </summary>
		/// <param name="requestFunc"></param>
		/// <typeparam name="TRequest"></typeparam>
		/// <typeparam name="TResponse"></typeparam>
		public void RegisterRequest<TRequest, TResponse>(Func<TRequest, TResponse> requestFunc)
			where TRequest : BasePacket, new()
			where TResponse : BasePacket, new();
		
		/// <summary>
		/// Async send a message expecting a response
		/// </summary>
		/// <param name="request"></param>
		/// <param name="sendFunc"></param>
		/// <param name="timeoutMs"></param>
		/// <typeparam name="TResponse"></typeparam>
		/// <typeparam name="TRequest"></typeparam>
		/// <returns></returns>
		public Task<TResponse> SendAsync<TResponse, TRequest>(TRequest request, Func<BasePacket, bool> sendFunc, int timeoutMs = 2000)
			where TResponse : BasePacket, new()
			where TRequest : BasePacket, new();
	}
}
