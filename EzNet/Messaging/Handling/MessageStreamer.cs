using EzNet.Logging;
using EzNet.Messaging.Extensions;
using EzNet.Messaging.Handling.Abstractions;
using EzNet.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
			=> callback += ReadMessage;

		public void DeregisterByteHandler(ref Action<ArraySegment<byte>, Connection> callback)
			=> callback -= ReadMessage;

		private void ReadMessage(ArraySegment<byte> segment, Connection source)
		{
			if (segment.Array == null)
			{
				Log.Warn("Cannot read packets from null buffer");
				return;
			}

			MemoryStream ms;
			if (_buffer != null)
			{
				_buffer.Enqueue(segment);
				if (_buffer.Length == _buffer.Capacity)
				{
					ms = new MemoryStream(_buffer.GetBuffer());
					Log.Info("USING BUFFER");
				}
				else
				{
					return;
				}
			}
			else
			{
				ms = new MemoryStream(segment.Array, segment.Offset, segment.Count);
			}
			while (ms.Position < ms.Length)
			{
				if (PacketExtension.TryReadType(ms, out Type type) && 
				    _container.TryGetValue(type, out IMessageCodec handler))
				{
					BasePacket packet = handler.CreatePacket();
					int length = packet.PeekLength(ms);
					
					if (ms.Length != length)
					{
						packet.Read(ms);
						handler.Enqueue(packet, source);
						_buffer?.Dispose();
						_buffer = null;
					}
					else
					{
						_buffer = new ByteBuffer(length);
						Log.Error("Failed to read packet bc lib dum. Got {0} wanted {1}", ms.Length, length);
					}
				}
				else
				{
					Log.Warn("No message handler for packet type {0}. Bytes: {1}", type, ms.Length);
					// TODO: Add message queue for unsubscribed messages
					break;
				}
			}
			ms.Dispose();
		}

		private ByteBuffer? _buffer;
	}
}
