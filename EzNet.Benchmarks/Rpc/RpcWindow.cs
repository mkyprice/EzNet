using EzNet.Benchmarks.State;
using EzNet.Transports.Extensions;
using EzRpc;
using Raylib_cs;
using System.Net;

namespace EzNet.Benchmarks
{
	public class RpcWindow : Window
	{
		private readonly GameState State = new GameState();
		private RpcClient Rpc;
		public RpcWindow(int width, int height, string title) : base(width, height, title)
		{
		}

		protected override void Load()
		{
			Rpc = new RpcClient(
				ConnectionFactory.BuildClient(Server.TCP, true), 
				null);

			Rpc.Bind(State);
		}

		protected override void Render()
		{
			foreach (GameObject gameobject in State._gameobjects)
			{
				Raylib.DrawCircleV(gameobject.Position, 32, Color.RED);
			}
		}
	}
}