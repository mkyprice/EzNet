
using EzNet;
using System.Net;
using System.Threading.Tasks;

namespace EzRPC
{
	public class RpcClient : Rpc
	{
		public override void Start(EndPoint tcp, EndPoint udp)
		{
			Tcp = ConnectionFactory.BuildClient(tcp, true);
			Udp = ConnectionFactory.BuildClient(udp, false);
			
			Tcp.RegisterResponseHandler<RpcResponse, RpcRequest>(ResponseHandler);
			Udp.RegisterResponseHandler<RpcResponse, RpcRequest>(ResponseHandler);
		}
	}
}
