using EzNet.Logging;
using EzNet.Messaging.Extensions;
using EzNet.Messaging.Handling.Abstractions;
using System;
using System.IO;

namespace EzNet.Messaging.Handling
{
	public class MessageStreamer : IMessageStreamer
	{
		private readonly IMessageContainer _container;
		private Func<byte[], bool> _send;

		public MessageStreamer(IMessageContainer container)
		{
			_container = container;
		}

		public bool WriteMessage(BasePacket packet)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				PacketExtension.Serialize(ms, packet);
				byte[] bytes = ms.ToArray();
				return _send(bytes);
			}
		}
		public void RegisterMessageReader(ref Action<ArraySegment<byte>, Connection> callback)
			=> callback += ReadMessage;

		public void DeregisterMessageReader(ref Action<ArraySegment<byte>, Connection> callback)
			=> callback -= ReadMessage;
		
		public void RegisterByteSender(Func<byte[], bool> send) => _send += send;
		public void DeregisterByteSender(Func<byte[], bool> send) => _send -= send;
		
		private void ReadMessage(ArraySegment<byte> segment, Connection source)
		{
			if (segment.Array == null)
			{
				Log.Warn("Cannot read packets from null buffer");
				return;
			}

			using MemoryStream ms = new MemoryStream(segment.Array, segment.Offset, segment.Count);
			while (ms.Position < ms.Length)
			{
				BasePacket? packet = PacketExtension.Deserialize(ms);
				if (packet != null)
				{
					if (_container.TryGetValue(packet.GetType(), out IMessageQueue handler))
					{
						handler.Enqueue(packet, source);
					}
				}
				else
				{
					return;
				}
			}
		}
	}
}
