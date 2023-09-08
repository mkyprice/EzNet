using EzRPC.Reflection.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EzRPC
{
	public class RpcRequest
	{
		public int RequestId { get; set; }
		public MethodModel Method { get; set; }
		public object[] Params { get; set; }
	}
}
