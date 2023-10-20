using EzNet;
using EzRpc.Injection;
using EzRpc.State;
using System;
using System.Net;

namespace EzRpc
{
	public class RpcBuilder
	{
		public static Rpc Build(Func<RpcBuilder, RpcBuilder> builder)
		{
			return builder.Invoke(new RpcBuilder()).Build();
		}

		public readonly ServiceProvider Services = new ServiceProvider();
		public bool IsServer = false;
		public EndPoint? EndPointTCP;
		public EndPoint? EndPointUDP;
		
		private Rpc Build()
		{
			Rpc rpc;
			if (IsServer)
			{
				Server? tcp = EndPointTCP != null ? ConnectionFactory.BuildServer(EndPointTCP, true) : null;
				Server? udp = EndPointUDP != null ? ConnectionFactory.BuildServer(EndPointUDP, false) : null;
				rpc = new RpcServer(tcp, udp, Services);
			}
			else
			{
				Connection? tcp = EndPointTCP != null ? ConnectionFactory.BuildClient(EndPointTCP, true) : null;
				Connection? udp = EndPointUDP != null ? ConnectionFactory.BuildClient(EndPointUDP, false) : null;
				rpc = new RpcClient(tcp, udp, new RpcSession(), Services);
			}
			return rpc;
		}
	}
}
