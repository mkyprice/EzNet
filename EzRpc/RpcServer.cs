using EzNet;
using EzRpc.Logging;
using EzRpc.Messaging;
using EzRpc.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EzRpc
{
	public class RpcServer : Rpc
	{
		#region Events

		public Action<RpcClient> OnClientConnected;

		#endregion
		private readonly List<RpcClient> _clients = new List<RpcClient>();
		public new Server? Tcp
		{
			get => base.Tcp as Server;
			set
			{
				base.Tcp = value;
				if (value != null)
				{
					value.OnConnectionAdded += OnConnectionAdded;
				}
			}
		}
		public new Server? Udp
		{
			get => base.Udp as Server;
			set
			{
				base.Udp = value;
				if (value != null)
				{
					value.OnConnectionAdded += OnConnectionAdded;
				}
			}
		}

		public RpcServer() : base(new RpcSession()) { }

		public RpcServer(Server tcp, Server udp) : base(tcp, udp, new RpcSession())
		{
			Tcp = tcp;
			Udp = udp;
		}

		/// <summary>
		/// Calls a method on all connections
		/// </summary>
		/// <param name="method"></param>
		/// <param name="args"></param>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TR"></typeparam>
		/// <returns></returns>
		public async Task<TR[]> CallAsync<T, TR>(string method, params object[] args)
			=> await CallAsync<TR>(typeof(T), method, args);

		/// <summary>
		/// Calls a method on all connections
		/// </summary>
		/// <param name="type"></param>
		/// <param name="method"></param>
		/// <param name="args"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public async Task<T[]> CallAsync<T>(Type type, string method, params object[] args)
		{
			if (HandleLocalCall(type, method, args, out RpcRequest request, out Network sender) &&
			    sender is Server server)
			{
				// List<Task<RpcResponse>> sendTasks = new List<Task<RpcResponse>>();
				// foreach (Connection connection in server.GetConnections())
				// {
				// 	sendTasks.Add(connection.SendAsync<RpcResponse, RpcRequest>(request));
				// }
				Connection[] connections = server.GetConnections().ToArray();
				RpcResponse[] responses = new RpcResponse[connections.Length];
				for (int i = 0; i < connections.Length; i++)
				{
					responses[i] = await connections[i].SendAsync<RpcResponse, RpcRequest>(request);
				}
				// RpcResponse[]? responses = await Task.WhenAll(sendTasks.ToArray());
				List<T> results = new List<T>();
				foreach (RpcResponse? response in responses)
				{
					if (response.Error != RPC_ERROR.None)
					{
						Log.Warn("RPC encountered error: {0}", response.Error);
					}
					T result = (T)response.Result;
					if (result == null)
					{
						Log.Warn("Result was null");
					}
					results.Add(result);
				}
				return results.ToArray();
			}
			return Array.Empty<T>();
		}
		
		public override void Call(Type type, string method, params object[] args)
		{
			if (HandleLocalCall(type, method, args, out RpcRequest request, out Network sender) &&
			    sender is Server server)
			{
				foreach (Connection connection in server.GetConnections())
				{
					connection.Send(request);
				}
			}
		}
		
		private void OnConnectionAdded(int obj)
		{
			if (Tcp.TryGetConnection(obj, out Connection connection))
			{
				RpcClient client = new RpcClient(connection, null, Session);
				_clients.Add(client);
				OnClientConnected?.Invoke(client);
			}
		}
		
	}
}
