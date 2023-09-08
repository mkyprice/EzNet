using EzNet.Messaging;

namespace EzNet.Tcp
{
	public class TcpClient : TcpRawConnection
	{
		public TcpClient()
		{
		}
		
		public bool Send<T>(T packet)
			where T : BasePacket
		{
			byte[] bytes = PacketSerializerExtension.Serialize(packet);
			return base.Send(bytes);
		}

		
		public void Dispose()
		{
			
		}
	}
}
