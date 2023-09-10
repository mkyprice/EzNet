using System.Collections.Concurrent;

namespace EzNet.Messaging.Handling
{
	public interface IMessageTypeHandler
	{
		public void ReadPacket(Stream stream, PacketConnection source);
		public void Update();
	}
	
	public class MessageTypeHandler<T> : IMessageTypeHandler
		where T : BasePacket, new()
	{
		private Action<MessageNotification<T>> _callback;
		public int Count => _receivePacketQueue.Count;
		private readonly ConcurrentQueue<MessageNotification<T>> _receivePacketQueue = new ConcurrentQueue<MessageNotification<T>>();

		public void AddCallback(Action<MessageNotification<T>> callback)
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

		public void RemoveCallback(Action<MessageNotification<T>> callback)
		{
			if (_callback != null)
			{
				_callback -= callback;
			}
		}

		public void Update()
		{
			if (_callback != null && TryDequeue(out var message))
			{
				_callback.Invoke(message);
			}
		}

		private bool TryDequeue(out MessageNotification<T> packet) => _receivePacketQueue.TryDequeue(out packet);
		private bool TryPeek(out MessageNotification<T> packet) => _receivePacketQueue.TryPeek(out packet);

		private async Task<MessageNotification<T>> DequeueAsync()
		{
			MessageNotification<T> packet;
			while (TryDequeue(out packet) == false && Count > 0)
			{
				await Task.Yield();
			}
			return packet;
		}

		private async Task<MessageNotification<T>> PeekAsync()
		{
			MessageNotification<T> packet;
			while (TryPeek(out packet) == false && Count > 0)
			{
				await Task.Yield();
			}
			return packet;
		}
		
		public void ReadPacket(Stream stream, PacketConnection source)
		{
			T packet = new T();
			packet.Read(stream);
			// Log.Info("Read packet {0}", packet);
			_receivePacketQueue.Enqueue(new MessageNotification<T>(packet, source));
		}
	}
}
