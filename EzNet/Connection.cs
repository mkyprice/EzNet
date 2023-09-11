using EzNet.Messaging.Extensions;
using EzNet.Messaging.Handling.Abstractions;
using EzNet.Transports;
using System;
using System.IO;
using System.Threading;

namespace EzNet
{
	public class Connection : Network
	{
		public readonly int Id;
		protected readonly ITransportConnection RawConnection;
		private Action<ArraySegment<byte>, Connection> OnBytesReceived;

		private static volatile int _nextId = 1;

		internal Connection(ITransportConnection rawConnection)
		{
			MessageHandler = EzNet.Messaging.Handling.MessageHandler.Build();
			RawConnection = rawConnection;
			RawConnection.OnReceive += ReceiveRaw;
			MessageHandler.Streamer.RegisterByteHandler(ref OnBytesReceived);
			Id = _nextId;
			Interlocked.Increment(ref _nextId);
		}

		internal Connection(ITransportConnection rawConnection, IMessageHandler handler)
		{
			MessageHandler = handler;
			RawConnection = rawConnection;
			RawConnection.OnReceive += ReceiveRaw;
			MessageHandler.Streamer.RegisterByteHandler(ref OnBytesReceived);
			Id = _nextId;
			Interlocked.Increment(ref _nextId);
		}

		
		public override void Send<T>(T packet)
		{
			using (MemoryStream? ms = new MemoryStream())
			{
				PacketExtension.Serialize(ms, packet);
				byte[] bytes = ms.ToArray();
				RawConnection.Send(bytes);
			}
		}

		public override void Dispose()
		{
			RawConnection.Shutdown();
			MessageHandler.Dispose();
		}
		
		private void ReceiveRaw(ArraySegment<byte> bytes, ITransportConnection connection) => OnBytesReceived?.Invoke(bytes, this);
	}
}
