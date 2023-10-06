using EzNet;
using EzRpc.Injection;
using EzRpc.Logging;
using EzRpc.Messaging;
using EzRpc.State;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace EzRpc
{
	public class RpcServer : Rpc
	{
		private readonly List<RpcClient> _clients = new List<RpcClient>();
		protected new Server Tcp;
		protected new Server Udp;
		
		public RpcServer(Server tcp, Server udp) : this(tcp, udp, new ServiceProvider())
		{
		}
		
		public RpcServer(Server tcp, Server udp, ServiceProvider services) : base(tcp, udp, new RpcSession(), services)
		{
			Tcp = tcp;
			Udp = udp;
			tcp.OnConnectionAdded += OnConnectionAdded;
		}

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
			MethodInfo? info = Session.GetMethod(type, method);
			Synced? sync = Session.GetMethodSyncData(type, method);
			if (info == null || sync == null)
			{
				Log.Warn("No bound instance for type {0}", type);
				return default;
			}
			// Local
			if (sync.CallLocal)
			{
				CallMethod(type, method, args);
			}

			// Send request
			Server sender = sync.IsReliable ? Tcp : Udp;
			RpcRequest request = new RpcRequest()
			{
				Type = type,
				Method = method,
				Args = args
			};
			List<Task<RpcResponse>> sendTasks = new List<Task<RpcResponse>>();
			foreach (Connection connection in sender.GetConnections())
			{
				sendTasks.Add(connection.SendAsync<RpcResponse, RpcRequest>(request));
			}
			var responses = await Task.WhenAll(sendTasks.ToArray());
			List<T> results = new List<T>();
			foreach (var response in responses)
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
		
		private void OnConnectionAdded(int obj)
		{
			if (Tcp.TryGetConnection(obj, out Connection connection))
			{
				RpcClient client = new RpcClient(connection, null, Session, Services);
				_clients.Add(client);
			}
		}
	}
}
