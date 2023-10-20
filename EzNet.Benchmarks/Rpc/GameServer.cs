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
		public Rectangle Bounds = new Rectangle(0, 0, 128, 128);
		
		private readonly GameState State = new GameState();
		private RpcServer Rpc;
		private bool _isDisposed = false;

		public GameServer()
		{
			Rpc = new RpcServer();
			Rpc.Tcp = ConnectionFactory.BuildServer(TCP, true);

			Rpc.Bind(State);
			
			Rpc.OnClientConnected += OnClientConnected;
			BuildWorld();
		}

		private void BuildWorld()
		{
			for (int i = 0; i < 10; i++)
			{
				Vector2 pos = GetRandomVector(Bounds.width, Bounds.height);
				Vector2 velocity = GetRandomVector(1, 1) * 100;
				GameObject gameObject = new GameObject()
				{
					Id = Random.Shared.Next(),
					Position = pos,
					Velocity = velocity,
					Color = ChooseRandom(Color.RED, Color.BLUE, Color.GREEN)
				};
				State.Gameobjects[gameObject.Id] = gameObject;
			}
		}
		
		private void OnClientConnected(RpcClient obj)
		{
			GameObject[] gameObjects = State.Gameobjects.Values.ToArray();
			Rpc.Call<GameState>(nameof(GameState.SyncObjects), gameObjects);
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
				await Task.Delay(10);
			
				if (Rpc.Connections > 0)
				{
					foreach (GameObject gameObject in State.Gameobjects.Values)
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
		}
		public void Dispose()
		{
			Rpc.Dispose();
		}


		private static Vector2 GetRandomVector(float scalarX, float scalarY)
			=> new Vector2(Random.Shared.NextSingle() * scalarX, Random.Shared.NextSingle() * scalarY);

		private static T ChooseRandom<T>(params T[] args)
			=> args[Random.Shared.Next(args.Length)];
	}
}
