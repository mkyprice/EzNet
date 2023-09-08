using System;

namespace EzRPC
{
	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class Synced : Attribute
	{
		/// <summary>
		/// Used to identify the type instance across assemblies
		/// </summary>
		public readonly string Name;

		public Synced(string name = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				name = nameof(Synced);
			}
			Name = name;
		}
	}
}
