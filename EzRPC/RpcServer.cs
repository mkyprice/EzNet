using System.Threading.Tasks;

namespace EzRPC
{
	public class RpcServer : BaseRpc
	{
		protected override Task<object> SendRequestAsync(RpcRequest request)
		{
			TaskCompletionSource<object> completion = new TaskCompletionSource<object>();

			

			return completion.Task;
		}
	}
}
