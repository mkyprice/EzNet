using EzNet.Messaging.Handling.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace EzNet.Messaging.Handling
{
	internal class MessageContainer : IMessageContainer
	{
		private readonly ConcurrentDictionary<Type, IMessageQueue> _messageHandlers = new ConcurrentDictionary<Type, IMessageQueue>();

		public void RegisterMessageType<T>() where T : BasePacket, new() => GetOrCreateMessageHandler<T>();
		public MessageQueue<T> GetOrCreateMessageHandler<T>()
			where T : BasePacket, new()
		{
			Type type = typeof(T);
			if (_messageHandlers.TryGetValue(type, out IMessageQueue? handler))
			{
				return ((MessageQueue<T>)handler);
			}
			handler = new MessageQueue<T>();
			_messageHandlers[type] = handler;
			return (MessageQueue<T>)handler;
		}


		public bool TryGetValue(Type type, out IMessageQueue value) => _messageHandlers.TryGetValue(type, out value);
		
		public IEnumerable<IMessageQueue> GetMessageQueues()
		{
			return _messageHandlers.Values;
		}
		
		public int Count<T>() where T : BasePacket, new() => GetOrCreateMessageHandler<T>().Count;
	}
}
