using EzNet.Messaging.Handling.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace EzNet.Messaging.Handling
{
	internal class MessageContainer : IMessageContainer
	{
		private readonly ConcurrentDictionary<Type, IMessageCodec> _messageHandlers = new ConcurrentDictionary<Type, IMessageCodec>();

		public void RegisterMessageType<T>() where T : BasePacket, new() => GetOrCreateMessageHandler<T>();
		public MessageCodec<T> GetOrCreateMessageHandler<T>()
			where T : BasePacket, new()
		{
			Type type = typeof(T);
			if (_messageHandlers.TryGetValue(type, out IMessageCodec? handler))
			{
				return ((MessageCodec<T>)handler);
			}
			handler = new MessageCodec<T>();
			_messageHandlers[type] = handler;
			return (MessageCodec<T>)handler;
		}


		public bool TryGetValue(Type type, out IMessageCodec value) => _messageHandlers.TryGetValue(type, out value);
		
		public IEnumerable<IMessageCodec> GetCodecs()
		{
			return _messageHandlers.Values;
		}
		
		public int Count<T>() where T : BasePacket, new() => GetOrCreateMessageHandler<T>().Count;
	}
}
