using System;

namespace EzRPC.Injection.Abstractions
{
	public interface IServiceContainer
	{
		public bool Add<T>(T value) where T : notnull;
		public T Get<T>();
		public object? Get(Type type);
		public bool Contains<T>();
	}
}
