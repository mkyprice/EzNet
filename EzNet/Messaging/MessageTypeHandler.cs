using EzNet.Logging;
using System.Collections.Concurrent;

namespace EzNet.Messaging
{
	public interface IMessageTypeHandler
	{
		public void ReadPacket(Stream stream);
	}
	
	public class MessageTypeHandler<T> : IMessageTypeHandler
		where T : BasePacket, new()
	{
		public int Count => _receivePacketQueue.Count;
		private Action<T> _callback = null;
		private readonly ConcurrentQueue<T> _receivePacketQueue = new ConcurrentQueue<T>();

		public bool TryDequeue(out T packet) => _receivePacketQueue.TryDequeue(out packet);
		public bool TryPeek(out T packet) => _receivePacketQueue.TryPeek(out packet);

		public void AddCallback(Action<T> callback) => _callback += callback;
		public void RemoveCallback(Action<T> callback)
		{
			if (_callback != null) _callback -= callback;
		}
		
		public void ReadPacket(Stream stream)
		{
			T packet = new T();
			packet.Read(stream);
			Log.Info("Read packet {0}", packet);
			_receivePacketQueue.Enqueue(packet);
		}
	}
}
