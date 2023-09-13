using EzNet.Messaging;
using EzNet.Messaging.Handling.Abstractions;
using System;
using System.Threading;

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
		/// Register a function to handle a packet type
		/// </summary>
		/// <param name="callback"></param>
		/// <typeparam name="TPacket"></typeparam>
		public void RegisterMessageHandler<TPacket>(Action<TPacket, Connection> callback) 
			where TPacket : BasePacket, new()
			=> MessageHandler.AddCallback(callback);
		

		/// <summary>
		/// Remove a function that handled a packet type
		/// </summary>
		/// <param name="callback"></param>
		/// <typeparam name="TPacket"></typeparam>
		public void RemoveMessageHandler<TPacket>(Action<TPacket, Connection> callback) 
			where TPacket : BasePacket, new()
			=> MessageHandler.RemoveCallback(callback);
		
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
