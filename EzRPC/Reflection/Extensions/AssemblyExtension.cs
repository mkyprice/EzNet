using System;
using System.Collections.Generic;
using System.Reflection;

namespace EzRPC.Reflection.Extensions
{
	internal static class AssemblyExtension
	{
		public static IEnumerable<Type> GetTypesWithAttribute<T>()
			where T : Attribute
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assembly.GetTypes())
				{
					T? attr = type.GetCustomAttribute<T>(true);
					if (attr != null)
					{
						yield return type;
					}
				}
			}
		}
	}
}
