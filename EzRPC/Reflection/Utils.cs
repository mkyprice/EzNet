using System;
using System.Reflection;

namespace EzRPC.Reflection
{
	internal static class Utils
	{
		public static Type[] ToTypeArray(this ParameterInfo[] parameters)
		{
			Type[] types = new Type[parameters.Length];
			for (int i = 0; i < types.Length; i++)
			{
				types[i] = parameters[i].ParameterType;
			}
			return types;
		}
		
		public static Type[] ToTypeArray(this object[] objs)
		{
			Type[] types = new Type[objs.Length];
			for (int i = 0; i < types.Length; i++)
			{
				types[i] = objs[i] == null ? null : objs.GetType();
			}
			return types;
		}
	}
}
