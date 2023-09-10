using EzNet.Logging;
using EzNet.Messaging.Extensions;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace EzNet.Messaging.Handling
{
	public class MessageHandler : IDisposable
	{
		private readonly ConcurrentDictionary<Type, IMessageTypeHandler> _messageHandlers = new ConcurrentDictionary<Type, IMessageTypeHandler>();

		private Task MessageHandlerTask;
		private bool IsDisposed = false;
		public MessageHandler()
		{
			PacketExtension.Init();
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

		internal void ReadPackets(ArraySegment<byte> bytes, PacketConnection source)
		{
			if (bytes.Array == null)
			{
				Log.Warn("Cannot read packets from null buffer");
				return;
			}
			
			using var ms = new MemoryStream(bytes.Array, bytes.Offset, bytes.Count);
			while (ms.Position < ms.Length)
			{
				if (PacketExtension.TryReadType(ms, out Type type) && 
				    _messageHandlers.TryGetValue(type, out var handler))
				{
					handler.ReadPacket(ms, source);
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
