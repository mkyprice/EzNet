using System;

namespace EzRpc
{
	/// <summary>
	/// Apply to methods that will be used in RPC
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class Synced : Attribute
	{
		public bool IsReliable = true;
		public bool CallLocal = false;
	}
}
