using System;

namespace EzRPC.Reflection.Extensions
{
	internal static class TypeExtensions
	{
		public static string Serialize(Type type) => type?.AssemblyQualifiedName ?? string.Empty;
		public static Type? Deserialize(string type) => Type.GetType(type);
	}
}
