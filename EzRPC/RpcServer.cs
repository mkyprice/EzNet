using EzNet;
using System.Net;
using System.Threading.Tasks;

namespace EzRPC
{
	public class RpcServer : BaseRpc
	{
		
		public RpcServer(EndPoint tcpEndpoint, EndPoint udpEndpoint)
		{
			Tcp = ConnectionFactory.BuildServer(tcpEndpoint, true);
			Udp = ConnectionFactory.BuildServer(udpEndpoint, false);
            
			Tcp.RegisterResponseHandler<RpcResponse, RpcRequest>(ResponseHandler);
			Udp.RegisterResponseHandler<RpcResponse, RpcRequest>(ResponseHandler);
		}
	}
}
