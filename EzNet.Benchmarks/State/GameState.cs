using EzRpc;
using System.Collections.Concurrent;
using System.Numerics;

namespace EzNet.Benchmarks.State
{
	public class GameState
	{
		public readonly ConcurrentDictionary<int, GameObject> _gameobjects = new ConcurrentDictionary<int, GameObject>();
		
		[Synced(CallLocal = true, IsReliable = true)]
		public void Create(GameObject gameObject)
		{
			_gameobjects[gameObject.Id] = gameObject;
		}
		
		
		
		[Synced(CallLocal = true, IsReliable = true)]
		public void SetPosition(int id, Vector2 position)
		{
			if (_gameobjects.TryGetValue(id, out var gameObject))
			{
				gameObject.Position = position;
				_gameobjects[id] = gameObject;
			}
		}
		
		[Synced(CallLocal = true, IsReliable = true)]
		public void SetVelocity(int id, Vector2 velocity)
		{
			if (_gameobjects.TryGetValue(id, out var gameObject))
			{
				gameObject.Velocity = velocity;
				_gameobjects[id] = gameObject;
			}
		}
	}
}
