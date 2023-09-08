using EzNet.Logging;

namespace EzNet.IO.Extensions
{
	public static class MemoryExtensions
	{
		public static byte[] ToBytes(this Stream stream, int offset, int size)
		{
			long position = stream.Position;
			byte[] buffer = new byte[size];
			stream.Position = offset;
			stream.Read(buffer, 0, size);
			stream.Position = position;
			return buffer;
		}
	}
}
