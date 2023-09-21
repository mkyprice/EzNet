using EzRpc;

namespace EzNet.Tests.Models.Rpc
{
	public class BasicRpc
	{
		[Synced(CallLocal = false, IsReliable = true)]
		public float Test(float a, float b)
		{
			return a + b;
		}
		
		[Synced(CallLocal = false, IsReliable = true)]
		public TestValues TestAdvanced(TestValues values)
		{
			return values;
		}
	}
}
