namespace EzRPC.Transport.Tcp
{
	public interface ITcpConnection
	{
		public bool Send(byte[] bytes);
	}
}
