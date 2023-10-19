using EzRpc;
using System.Collections.Concurrent;
using System.Numerics;

namespace EzNet.Benchmarks.State
{
	public class GameState
	{
		public readonly ConcurrentBag<GameObject> _gameobjects = new ConcurrentBag<GameObject>();
		
		[Synced(CallLocal = true, IsReliable = true)]
		public void Create(Vector2 position, Vector2 velocity)
		{
			_gameobjects.Add(new GameObject()
			{
				Position = position,
				Velocity = velocity
			});
		}
	}
}
