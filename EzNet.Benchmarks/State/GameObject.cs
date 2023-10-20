using Raylib_cs;
using System.Numerics;

namespace EzNet.Benchmarks.State
{
	[Serializable]
	public struct GameObject
	{
		public int Id { get; set; }
		public Vector2 Position { get; set; }
		public Vector2 Velocity { get; set; }
		public Color Color { get; set; }

		public GameObject()
		{
			Id = 0;
			Position = Vector2.Zero;
			Velocity = Vector2.Zero;
			Color = Color.WHITE;
		}
	}
}
