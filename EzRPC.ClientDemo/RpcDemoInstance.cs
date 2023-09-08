namespace EzRPC.ClientDemo
{
	[Synced]
	public class RpcDemoInstance
	{
		public TestValueClass DemoTest(TestValueClass t)
		{
			return new TestValueClass();
		}
	}
	
	[Synced("Demo")]
	public class RpcDemoInstance2
	{
		public float DemoTest(float x, float y)
		{
			return x + y;
		}
	}
}
