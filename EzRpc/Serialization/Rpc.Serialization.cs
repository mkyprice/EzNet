using EzRpc.Serialization.Extensions;
using System;
using System.Collections.Generic;

namespace EzRpc.Serialization
{
	public partial class Rpc
	{
		private static Func<object> _defaultSerializer = CreationExtension.GetActivator<XmlByteSerializer>();
		private static readonly Dictionary<Type, Func<object>> _typeSerializers = 
			new Dictionary<Type, Func<object>>();

		public static void SetDefaultSerializer<T>()
			where T : ISerializer
		{
			_defaultSerializer = CreationExtension.GetActivator(typeof(T));
		}
		
		public static ISerializer GetSerializer(Type type)
		{
			if (_typeSerializers.TryGetValue(type, out Func<object> creation))
			{
				return (ISerializer)creation();
			}
			return (ISerializer)_defaultSerializer();
		}

		public static void RegisterTypeSerializer<T>(params Type[] types)
			where T : ISerializer
		{
			foreach (Type type in types)
			{
				_typeSerializers[type] = CreationExtension.GetActivator(typeof(T));
			}
		}
	}
}
