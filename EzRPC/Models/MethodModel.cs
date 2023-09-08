using System;

namespace EzRPC
{
	public struct MethodModel
	{
		public string MethodName { get; set; }
		public Type DeclaringType { get; set; }
		public Type[] ParameterTypes { get; set; }
		public Type[] GenericArgs { get; set; }
	}
}
