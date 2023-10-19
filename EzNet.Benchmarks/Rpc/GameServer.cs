using EzNet.Benchmarks.State;
using EzNet.Transports.Extensions;
using EzRpc;
using Raylib_cs;
using System.Net;
using System.Numerics;

namespace EzNet.Benchmarks
{
	public class GameServer : IDisposable
	{
		public static readonly EndPoint TCP = SocketExtensions.GetEndPoint(5500);
		private readonly GameState State = new GameState();
		private readonly Rectangle Bounds = new Rectangle(0, 0, 128, 128);
		private RpcServer Rpc;
		private bool _isDisposed = false;

		public GameServer()
		{
			var tcp = ConnectionFactory.BuildServer(TCP, true);
			Rpc = new RpcServer(tcp, null);

			Rpc.Bind(State);
			
			Rpc.OnClientConnected += OnClientConnected;
		}
		
		private void OnClientConnected(RpcClient obj)
		{
			Vector2 pos = GetRandomVector(Bounds.width, Bounds.height);
			Vector2 velocity = GetRandomVector(2, 2);
			GameObject gameObject = new GameObject()
			{
				Id = Random.Shared.Next(),
				Position = pos,
				Velocity = velocity
			};
			Rpc.Call<GameState>(nameof(GameState.Create), gameObject);
		}

		public void Run()
		{
			Task.Run(MainLoop);
		}

		public async Task MainLoop()
		{
			const float dt = 1f / 60;
			while (_isDisposed == false)
			{
				await Task.Delay(1);

				foreach (GameObject gameObject in State._gameobjects.Values)
				{
					Vector2 p = gameObject.Position + gameObject.Velocity * dt;
					Rpc.Call<GameState>(nameof(GameState.SetPosition), gameObject.Id, p);

					Vector2 velocity = gameObject.Velocity;
					if (p.X <= Bounds.x || p.Y <= Bounds.y)
					{
						velocity.X *= -1;
					}
					if (p.X >= Bounds.width || p.Y >= Bounds.height)
					{
						velocity.Y *= -1;
					}
					if (velocity != gameObject.Velocity)
					{
						Rpc.Call<GameState>(nameof(GameState.SetVelocity), gameObject.Id, velocity);
					}
				}
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
