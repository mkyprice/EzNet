using EzNet.IO.Extensions;
using EzRpc.Serialization;
using System.Numerics;

namespace EzNet.Benchmarks.Serializers
{
	public class Vector2Serializer : ISerializer
	{

		public void Serialize(Stream stream, object value)
		{
			if (value is Vector2 v)
			{
				stream.Write(v.X);
				stream.Write(v.Y);
			}
		}
		public object Deserialize(Stream stream, Type type)
		{
			return (object)new Vector2(stream.ReadSingle(), stream.ReadSingle());
		}
	}
}
