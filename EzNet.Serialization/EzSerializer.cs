using EzNet.Serialization.Extensions;
using System.Text;

namespace EzNet.Serialization
{
	public class EzSerializer : IDisposable
	{
		public static Encoding Encoding = Encoding.UTF8;
		
		public void Serialize(Stream stream, object value)
		{
			
		}

		public object Deserialize(Span<byte> bytes, Type type)
		{
			return type.NewInstance();
		}
		
		public void Dispose()
		{
			// TODO release managed resources here
		}
	}
}
