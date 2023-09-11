using EzNet;
using System.Net;
using System.Threading.Tasks;

namespace EzRPC
{
	public class RpcServer : Rpc
	{
		public override void Start(EndPoint tcp, EndPoint udp)
		{
			Tcp = ConnectionFactory.BuildServer(tcp, true);
			Udp = ConnectionFactory.BuildServer(udp, false);
            
			Tcp.RegisterResponseHandler<RpcResponse, RpcRequest>(ResponseHandler);
			Udp.RegisterResponseHandler<RpcResponse, RpcRequest>(ResponseHandler);
		}
	}
}
