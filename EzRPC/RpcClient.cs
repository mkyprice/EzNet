
using EzNet;
using System.Net;
using System.Threading.Tasks;

namespace EzRPC
{
	public class RpcClient : BaseRpc
	{
		public RpcClient(EndPoint tcpEndpoint, EndPoint udpEndpoint)
		{
			Tcp = ConnectionFactory.BuildClient(tcpEndpoint, true);
			Udp = ConnectionFactory.BuildClient(udpEndpoint, false);
			
			Tcp.RegisterResponseHandler<RpcResponse, RpcRequest>(ResponseHandler);
			Udp.RegisterResponseHandler<RpcResponse, RpcRequest>(ResponseHandler);
		}
	}
}
