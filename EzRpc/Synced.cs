using System;

namespace EzRpc
{
	[AttributeUsage(AttributeTargets.Method)]
	public class Synced : Attribute
	{
		public bool IsReliable = true;
		public bool CallLocal = false;
	}
}
