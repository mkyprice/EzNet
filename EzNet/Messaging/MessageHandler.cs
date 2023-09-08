using EzNet.Logging;
namespace EzNet.Messaging
{
	public class MessageHandler : IDisposable
	{
		private readonly Dictionary<Type, IMessageTypeHandler> _messageHandlers = new Dictionary<Type, IMessageTypeHandler>();

		public MessageHandler()
		{
			PacketSerializerExtension.Init();
		}
		
		public MessageTypeHandler<T> RegisterMessageHandler<T>()
			where T : BasePacket, new()
		{
			Type type = typeof(T);
			if (_messageHandlers.TryGetValue(type, out IMessageTypeHandler? handler))
			{
				return ((MessageTypeHandler<T>)handler);
			}
			handler = new MessageTypeHandler<T>();
			_messageHandlers[type] = handler;
			return (MessageTypeHandler<T>)handler;
		}

		public void RegisterMessageCallback<T>(Action<T> callback)
			where T : BasePacket, new()
		{
			RegisterMessageHandler<T>().AddCallback(callback);
		}

		public int Count<T>() where T : BasePacket, new()
		{
			return RegisterMessageHandler<T>().Count;
		}

		public T Dequeue<T>() where T : BasePacket, new()
		{
			var handler = RegisterMessageHandler<T>();
			T packet;
			while (handler.TryDequeue(out packet) == false && handler.Count > 0)
			{
				
			}
			return packet;
		}

		public T Peek<T>() where T : BasePacket, new()
		{
			var handler = RegisterMessageHandler<T>();
			T packet;
			while (handler.TryPeek(out packet) == false && handler.Count > 0)
			{
				
			}
			return packet;
		}

		internal void ReadPackets(byte[] packetBytes, int length)
		{
			using var ms = new MemoryStream(packetBytes, 0, length);
			while (ms.Position < ms.Length)
			{
				Type type = PacketSerializerExtension.GetPacketType(ms);
				if (_messageHandlers.TryGetValue(type, out var handler))
				{
					handler.ReadPacket(ms);
				}
				else
				{
					Log.Warn("No message handler for packet type {0}", type);
				}
			}
		}
		public void Dispose()
		{
			
		}
	}
}
