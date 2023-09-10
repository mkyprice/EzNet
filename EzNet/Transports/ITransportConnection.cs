using System;

namespace EzNet.Transports
{
	public interface ITransportConnection : IDisposable
	{
		public Action<ArraySegment<byte>, ITransportConnection> OnReceive { get; set; }
		public Action<ITransportConnection> OnDisconnect { get; set; }
		public void Send(byte[] bytes);
		public void Shutdown();
	}
}
