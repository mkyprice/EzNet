using EzNet;
using EzRpc.Injection;
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
		
		public int Connections { get => _clients.Count; }
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

		public RpcServer() : base(new RpcSession())
		{ }

		public RpcServer(Server tcp, Server udp) : base(tcp, udp, new RpcSession())
		{ }

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
			if (HandleLocalCall(type, method, args, out RpcRequest request, out Network sender) == false ||
			    (sender is Server server) == false) return Array.Empty<T>();
			
			Connection[] connections = server.GetConnections().ToArray();
			Task<RpcResponse>[] sendTasks = new Task<RpcResponse>[connections.Length];
			for (int i = 0; i < connections.Length; i++)
			{
				sendTasks[i] = connections[i].SendAsync<RpcResponse, RpcRequest>(request);
			}
			RpcResponse[]? responses = await Task.WhenAll(sendTasks);
			T[] results = new T[responses.Length];
			for (int i = 0; i < responses.Length; i++)
			{
				RpcResponse response = responses[i];
				if (response.Error != RPC_ERROR.None)
				{
					Log.Warn("RPC encountered error: {0}", response.Error);
				}
				T result = (T)response.Result;
				if (result == null)
				{
					Log.Warn("Result was null");
				}
				results[i] = result;
			}
			return results;
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
			if (Tcp?.TryGetConnection(obj, out Connection connection) == true ||
			    Udp?.TryGetConnection(obj, out connection) == true)
			{
				RpcClient client = new RpcClient(connection, null, Session);
				_clients.Add(client);
				OnClientConnected?.Invoke(client);
			}
		}
	}
}
