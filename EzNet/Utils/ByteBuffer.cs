using System;

namespace EzNet.Utils
{
	public class ByteBuffer : IDisposable
	{
		public int Length => _position;
		public int Capacity => _buffer.Length;
		private byte[] _buffer;
		private int _position = 0;

		public ByteBuffer(int capacity)
		{
			_buffer = new byte[capacity];
		}

		public void Enqueue(ArraySegment<byte> bytes)
		{
			int nextPos = Math.Min(_buffer.Length, _position + bytes.Count);
			int copyCount = (bytes.Offset - bytes.Count) - (_position + bytes.Count - nextPos);
			Buffer.BlockCopy(bytes.Array, bytes.Offset, _buffer, _position, copyCount);
			_position = nextPos;
		}

		public byte[] GetBuffer() => _buffer;
		public void Dispose()
		{
			_buffer = null;
			_position = 0;
		}
	}
}
