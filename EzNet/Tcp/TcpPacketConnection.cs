using EzNet.Logging;
using EzNet.Messaging;
using EzNet.Messaging.Requests;
using System.Net.Sockets;

namespace EzNet.Tcp
{
	public class TcpPacketConnection : RawTcpConnection
	{
		public readonly MessageHandler MessageHandler;
		protected readonly RequestHandler RequestHandler;
		
		public TcpPacketConnection()
		{
			MessageHandler = new MessageHandler();
			RequestHandler = new RequestHandler(MessageHandler);
		}

		public TcpPacketConnection(Socket socket, MessageHandler handler, RequestHandler requestHandler) : base(socket)
		{
			MessageHandler = handler;
			RequestHandler = requestHandler;
		}
		
		public bool Send<T>(T packet)
			where T : BasePacket
		{
			using MemoryStream ms = new MemoryStream();
			PacketSerializerExtension.Serialize(ms, packet);
			byte[] bytes = ms.ToArray();
			return base.Send(bytes);
		}

		public bool Send(byte[] packetBytes) => base.Send(packetBytes);

		public async Task<TResponse> SendAsync<TResponse, T>(T packet, int timeoutMS = 2000)
			where T : BasePacket, new()
			where TResponse : BasePacket, new()
		{
			var response = await RequestHandler.SendAsync<T, TResponse>(packet, (p) => this.Send(p), timeoutMS);
			return response;
		}

		protected override void Receive(ArraySegment<byte> bytes)
		{
			MessageHandler.ReadPackets(bytes, this);
		}
		
		public override void Dispose()
		{
			base.Dispose();
			if (IsDisposed) return;
			
			MessageHandler.Dispose();
		}
	}
}
