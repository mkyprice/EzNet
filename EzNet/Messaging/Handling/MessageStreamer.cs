using EzNet.Logging;
using EzNet.Messaging.Extensions;
using EzNet.Messaging.Handling.Abstractions;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace EzNet.Messaging.Handling
{
	public class MessageStreamer : IMessageStreamer
	{
		private readonly IMessageContainer _container;

		public MessageStreamer(IMessageContainer container)
		{
			_container = container;
		}
		
		public void RegisterByteHandler(ref Action<ArraySegment<byte>, Connection> callback)
		{
			callback += ReadMessage;
		}

		public void DeregisterByteHandler(ref Action<ArraySegment<byte>, Connection> callback)
		{
			callback -= ReadMessage;
		}

		private void ReadMessage(ArraySegment<byte> segment, Connection source)
		{
			if (segment.Array == null)
			{
				Log.Warn("Cannot read packets from null buffer");
				return;
			}
			
			using var ms = new MemoryStream(segment.Array, segment.Offset, segment.Count);
			while (ms.Position < ms.Length)
			{
				if (PacketExtension.TryReadType(ms, out Type type) && 
				    _container.TryGetValue(type, out var handler))
				{
					handler.ReadPacket(ms, source);
				}
				else
				{
					Log.Warn("No message handler for packet type {0}", type);
				}
			}
		}
	}
}
