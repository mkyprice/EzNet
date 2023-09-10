using System;
using System.Collections.Generic;

namespace EzNet.Messaging.Handling.Abstractions
{
	public interface IMessageContainer
	{
		/// <summary>
		/// Register a packet type.
		/// This is not required but may improve one time performance
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void RegisterMessageType<T>() where T : BasePacket, new();
		
		/// <summary>
		/// Try to get a message codec. If not found, it is created
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public MessageCodec<T> GetOrCreateMessageHandler<T>() where T : BasePacket, new();
		
		/// <summary>
		/// Get message codec by type
		/// </summary>
		/// <param name="type"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool TryGetValue(Type type, out IMessageCodec value);
		
		/// <summary>
		/// Get all current Message Codecs
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IMessageCodec> GetCodecs();
		
		/// <summary>
		/// Number of packets of given type that are queued
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public int Count<T>() where T : BasePacket, new();
	}
}
