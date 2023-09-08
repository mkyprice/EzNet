using System.IO;

namespace EzRPC
{
	public class RpcResponse
	{
		/// <summary>
		/// Matches a RequestId given in the request
		/// </summary>
		public int RequestId { get; set; }
		
		/// <summary>
		/// Result of the method call
		/// Null if error or void
		/// </summary>
		public object Result { get; set; }

		/// <summary>
		/// Error message
		/// </summary>
		public RPC_ERROR Error { get; set; }
	}
}
