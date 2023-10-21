using EzNet.Messaging.Handling.Abstractions;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace EzNet.Messaging.Handling
{
	public class MessageQueue<T> : IMessageQueue
		where T : BasePacket, new()
	{
		private Action<T, Connection> _callback;
		public int Count => _receivePacketQueue.Count;
		private readonly ConcurrentQueue<MessageNotification<T>> _receivePacketQueue = new ConcurrentQueue<MessageNotification<T>>();

		public void AddCallback(Action<T, Connection> callback)
		{
			if (_callback == null)
			{
				_callback = callback;
			}
			else
			{
				_callback += callback;
			}
		}

		public void RemoveCallback(Action<T, Connection> callback)
		{
			if (_callback != null)
			{
				_callback -= callback;
			}
		}
		
		public void Enqueue(BasePacket packet, Connection source) => 
			_receivePacketQueue.Enqueue(new MessageNotification<T>((T)packet, source));
		
		public BasePacket CreatePacket() => new T();

		public void DequeueMessages()
		{
			if (_callback != null && _receivePacketQueue.TryDequeue(out var message))
			{
				_callback.Invoke(message.Message, message.Source);
			}
		}
	}
}
