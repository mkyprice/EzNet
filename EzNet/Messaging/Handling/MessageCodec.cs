﻿using EzNet.Messaging.Handling.Abstractions;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace EzNet.Messaging.Handling
{
	public class MessageCodec<T> : IMessageCodec
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

		public void Update()
		{
			if (_callback != null && TryDequeue(out var message))
			{
				_callback.Invoke(message.Message, message.Source);
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
		
		public void ReadPacket(Stream? stream, Connection source)
		{
			T packet = new T();
			packet.Read(stream);
			// Log.Info("Read packet {0}", packet);
			_receivePacketQueue.Enqueue(new MessageNotification<T>(packet, source));
		}
	}
}
