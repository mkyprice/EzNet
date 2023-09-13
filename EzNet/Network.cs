using EzNet.Messaging;
using EzNet.Messaging.Extensions;
using EzNet.Messaging.Handling.Abstractions;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EzNet
{
	public abstract class Network : IDisposable
	{
		public readonly int Id;
		private static volatile int _nextId = 1;
		protected IMessageHandler MessageHandler;

		protected Network()
		{
			Id = _nextId;
			Interlocked.Increment(ref _nextId);
		}
		
		/// <summary>
		/// Send a packet
		/// </summary>
		/// <param name="packet"></param>
		/// <typeparam name="T"></typeparam>
		public abstract bool Send<T>(T packet) where T : BasePacket;

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
			where TRequest : BasePacket, new()
			=> MessageHandler.RegisterRequest(callback);

		/// <summary>
		/// Dispose of the network
		/// </summary>
		public abstract void Dispose();
	}
}
