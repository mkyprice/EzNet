using System;

namespace EzNet.Transports
{
	public interface ITransportServer : IDisposable
	{
		public Action<ITransportConnection> OnNewConnection { get; set; }
		public Action<ITransportConnection> OnEndConnection { get; set; }
		public bool Send(byte[] bytes);
		public void Shutdown();
	}
}
