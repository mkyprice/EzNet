using EzRPC.Transport.Tcp;
using EzRPC.Transport.Udp;
using System.Net;
using System.Threading.Tasks;

namespace EzRPC
{
	public class RpcClient : BaseRpc
	{
		private TcpConnection _tcp;
		private IUdpConnection _udp;

		public RpcClient(EndPoint tcpEndpoint)
		{
			_tcp = new TcpConnection();
			_udp = new UdpConnection();

			_tcp.Connect(tcpEndpoint);
		}
		
		public async Task<bool> Connect(EndPoint endPoint)
		{
			return false;
		}
		protected override Task<object> SendRequestAsync(RpcRequest request)
		{
			TaskCompletionSource<object> completion = new TaskCompletionSource<object>();

			

			return completion.Task;
		}
	}
}
