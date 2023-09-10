using EzNet.Logging;
using System.Collections.Concurrent;

namespace EzNet.Messaging
{
	public class MessageHandler : IDisposable
	{
		private readonly ConcurrentDictionary<Type, IMessageTypeHandler> _messageHandlers = new ConcurrentDictionary<Type, IMessageTypeHandler>();

		private Task MessageHandlerTask;
		private bool IsDisposed = false;
		public MessageHandler()
		{
			PacketSerializerExtension.Init();
			MessageHandlerTask = Task.Run(HandleMessageLoop);
		}

		private async Task HandleMessageLoop()
		{
			while (IsDisposed == false)
			{
				foreach (IMessageTypeHandler handler in _messageHandlers.Values)
				{
					handler.Update();
				}
				await Task.Delay(1);
			}
		}
		
		public MessageTypeHandler<T> GetOrCreateMessageHandler<T>()
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
		
		public void AddCallback<T>(Action<MessageNotification<T>> callback) where T : BasePacket, new()
		{
			GetOrCreateMessageHandler<T>().AddCallback(callback);
		}
		
		public void RemoveCallback<T>(Action<MessageNotification<T>> callback) where T : BasePacket, new()
		{
			GetOrCreateMessageHandler<T>().RemoveCallback(callback);
		}

		public int Count<T>() where T : BasePacket, new()
		{
			return GetOrCreateMessageHandler<T>().Count;
		}

		internal void ReadPackets(ArraySegment<byte> bytes, object args)
		{
			if (bytes.Array == null)
			{
				Log.Warn("Cannot read packets from null buffer");
				return;
			}
			
			using var ms = new MemoryStream(bytes.Array, bytes.Offset, bytes.Count);
			while (ms.Position < ms.Length)
			{
				if (PacketSerializerExtension.TryReadType(ms, out Type type) && 
				    _messageHandlers.TryGetValue(type, out var handler))
				{
					handler.ReadPacket(ms, args);
				}
				else
				{
					Log.Warn("No message handler for packet type {0}", type);
				}
			}
		}
		public void Dispose()
		{
			if (IsDisposed) return;
			//TODO: Dispose
			IsDisposed = true;
		}
	}
}
