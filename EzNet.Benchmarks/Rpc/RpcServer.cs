using EzNet.Benchmarks.State;
using EzNet.Transports.Extensions;
using EzRpc;
using System.Net;
using System.Numerics;

namespace EzNet.Benchmarks
{
	public class Server : IDisposable
	{
		public static readonly EndPoint TCP = SocketExtensions.GetEndPoint(5500);
		private readonly GameState State = new GameState();
		private RpcServer Rpc;
		private bool _isDisposed = false;

		public Server()
		{
			var tcp = ConnectionFactory.BuildServer(TCP, true);
			Rpc = new RpcServer(tcp, null);

			Rpc.Bind(State);
			
			Rpc.OnClientConnected += OnClientConnected;
		}
		
		private void OnClientConnected(RpcClient obj)
		{
			Vector2 pos = GetRandomVector(128, 64);
			Vector2 velocity = GetRandomVector(2, 2);
			Rpc.Call<GameState>(nameof(GameState.Create), pos, velocity);
		}

		public void Run()
		{
			Task.Run(MainLoop);
		}

		public async Task MainLoop()
		{
			while (_isDisposed == false)
			{
				
			}
		}
		public void Dispose()
		{
			Rpc.Dispose();
		}


		private static Vector2 GetRandomVector(float scalarX, float scalarY)
			=> new Vector2(Random.Shared.NextSingle() * scalarX, Random.Shared.NextSingle() * scalarY);
	}
}
