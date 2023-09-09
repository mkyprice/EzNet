using EzNet.Logging;
using EzNet.Messaging;
using EzNet.Messaging.Requests;
using System.Net.Sockets;

namespace EzNet.Tcp
{
	public class TcpPacketConnection : RawTcpConnection
	{
		public readonly MessageHandler MessageHandler;
		
		private Task PacketReaderTask;
		
		public TcpPacketConnection()
		{
			MessageHandler = new MessageHandler();
			PacketReaderTask = Task.Run(ReadRawMessageQueue);
		}

		public TcpPacketConnection(Socket socket, MessageHandler handler) : base(socket)
		{
			MessageHandler = handler;
			PacketReaderTask = Task.Run(ReadRawMessageQueue);
		}
		
		public bool Send<T>(T packet)
			where T : BasePacket
		{
			byte[] bytes = PacketSerializerExtension.Serialize(packet);
			return base.Send(bytes);
		}

		private async Task ReadRawMessageQueue()
		{
			while (IsDisposed == false)
			{
				if (TryDequeuePacket(out var packet))
				{
					MessageHandler.ReadPackets(packet, packet.Length, this);
				}
				else
				{
					await Task.Delay(1);
				}
			}
		}
		
		public override void Dispose()
		{
			base.Dispose();
			if (IsDisposed) return;
			
			MessageHandler.Dispose();
		}
	}
}
