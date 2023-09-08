namespace EzRPC.Transport
{
	public interface INetworkReceiver
	{
		public void ReceiveBytes(byte[] bytes);
	}
}
