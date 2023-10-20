using EzNet.Tests.Models;
using EzRpc.Injection;

namespace EzNet.Tests.Unit
{
	[TestClass]
	public class DITests
	{
		private interface ITest
		{
			
		}

		private class Test : ITest
		{
			private readonly Test2 _test2;
			public Test(Test2 test2)
			{
				_test2 = test2;
			}

			public override string ToString()
			{
				return base.ToString() + _test2.ToString();
			}
		}

		private class Test2
		{
			private string TestStr;
			public Test2()
			{
				TestStr = "Constructor called";
			}

			public override string ToString()
			{
				return base.ToString() + TestStr;
			}
		}
		
		[TestMethod]
		public void ServicesTests()
		{
			ServiceProvider services = new ServiceProvider();
			services.AddSingleton<ITest, Test>();
			services.AddTransient<Test2>();

			ITest t = services.GetService<ITest>();
			Console.WriteLine(t.ToString());
			Assert.IsNotNull(t);
		}
	}
}
