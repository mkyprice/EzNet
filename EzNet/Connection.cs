using EzNet.Messaging;
using EzNet.Messaging.Extensions;
using EzNet.Messaging.Handling;
using EzNet.Messaging.Handling.Abstractions;
using EzNet.Transports;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EzNet
{
	public class Connection : IDisposable
	{
		protected readonly IMessageHandler MessageHandler;
		protected readonly ITransportConnection RawConnection;
		private Action<ArraySegment<byte>, Connection> OnBytesReceived;

		internal Connection(ITransportConnection rawConnection)
		{
			MessageHandler = new MessageHandler();
			RawConnection = rawConnection;
			RawConnection.OnReceive += ReceiveRaw;
			MessageHandler.RegisterByteHandler(ref OnBytesReceived);
		}

		internal Connection(ITransportConnection rawConnection, IMessageHandler handler)
		{
			MessageHandler = handler;
			RawConnection = rawConnection;
			RawConnection.OnReceive += ReceiveRaw;
			MessageHandler.RegisterByteHandler(ref OnBytesReceived);
		}
		
		public void Send<T>(T packet)
			where T : BasePacket
		{
			using (MemoryStream ms = new MemoryStream())
			{
				PacketExtension.Serialize(ms, packet);
				byte[] bytes = ms.ToArray();
				RawConnection.Send(bytes);
			}
		}

		public async Task<TResponse> SendAsync<TResponse, TRequest>(TRequest packet, int timeoutMS = -1)
			where TRequest : BasePacket, new()
			where TResponse : BasePacket, new()
			=> await MessageHandler.SendAsync<TResponse, TRequest>(packet, Send, timeoutMS);

		private void ReceiveRaw(ArraySegment<byte> bytes, ITransportConnection connection) => OnBytesReceived?.Invoke(bytes, this);
		
		public void Dispose()
		{
			RawConnection.Shutdown();
			MessageHandler.Dispose();
		}
	}
}
