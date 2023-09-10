namespace EzNet
{
	public interface IConnection
	{
		public Action<ArraySegment<byte>> OnReceive { get; set; }
		public void Send(byte[] bytes);
		public void Shutdown();
	}
}
