using EzRPC.Reflection.Core;
using EzRPC.Reflection.Extensions;
using System;

namespace EzRPC
{
	public static class Rpc
	{
		internal static readonly MethodContainer MethodCache = new MethodContainer();
		private static bool _isInitialized = false;
		public static bool IsInitialized() => _isInitialized;
		public static bool Initialize()
		{
			if (_isInitialized) return true;

			foreach (Type type in AssemblyExtension.GetTypesWithAttribute<Synced>())
			{
				MethodCache.Cache(type);
			}
			
			_isInitialized = true;
			return true;
		}
	}
}
