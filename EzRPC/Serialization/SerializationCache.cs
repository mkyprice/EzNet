using System;
using System.Reflection;

namespace EzRPC.Serialization
{
	public class SerializationCache
	{
		public readonly Type CacheType;
		public static BindingFlags Flags = BindingFlags.Default | 
		                                   BindingFlags.Instance | 
		                                   BindingFlags.Public | 
		                                   BindingFlags.NonPublic;
		public readonly FieldInfo[] Fields;
		public readonly PropertyInfo[] Properties;

		public SerializationCache(Type type)
		{
			CacheType = type;
			Fields = CacheType.GetFields(Flags) ?? new FieldInfo[0];
			Properties = CacheType.GetProperties(Flags) ?? new PropertyInfo[0];
		}
	}
}
