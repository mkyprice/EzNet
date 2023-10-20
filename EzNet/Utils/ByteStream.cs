using System;
using System.Collections.Generic;
using System.IO;

namespace EzNet.Utils
{
	public class ByteStream : Stream, IDisposable
	{
		private readonly Queue<ArraySegment<byte>> _queue = new Queue<ArraySegment<byte>>();
		private bool _isDiposed = false;
		
		

		public void Dispose()
		{
			if (_isDiposed) return;
			_isDiposed = true;
			_queue.Clear();
		}
		public override void Flush()
		{
			_queue.Clear();
		}
		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}
		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}
		public override bool CanRead { get; }
		public override bool CanSeek { get; }
		public override bool CanWrite { get; }
		public override long Length { get; }
		public override long Position { get; set; }
	}
}
