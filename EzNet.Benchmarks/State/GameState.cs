using EzRpc;
using System.Collections.Concurrent;
using System.Numerics;

namespace EzNet.Benchmarks.State
{
	public class GameState
	{
		public readonly ConcurrentDictionary<int, GameObject> Gameobjects = new ConcurrentDictionary<int, GameObject>();
		
		[Synced(CallLocal = true, IsReliable = true)]
		public void Create(GameObject gameObject)
		{
			Gameobjects[gameObject.Id] = gameObject;
		}
		
		[Synced(CallLocal = true, IsReliable = true)]
		public void SetPosition(int id, Vector2 position)
		{
			if (Gameobjects.TryGetValue(id, out var gameObject))
			{
				gameObject.Position = position;
				Gameobjects[id] = gameObject;
			}
		}
		
		[Synced(CallLocal = true, IsReliable = true)]
		public void SetVelocity(int id, Vector2 velocity)
		{
			if (Gameobjects.TryGetValue(id, out var gameObject))
			{
				gameObject.Velocity = velocity;
				Gameobjects[id] = gameObject;
			}
		}

		[Synced(CallLocal = false, IsReliable = true)]
		public void SyncObjects(GameObject[] gameObjects)
		{
			for (int i = 0; i < gameObjects.Length; i++)
			{
				Gameobjects[gameObjects[i].Id] = gameObjects[i];
			}
		}
	}
}
