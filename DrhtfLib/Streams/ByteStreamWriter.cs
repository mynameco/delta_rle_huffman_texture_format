using DrhLib.Utility;
using System.IO;

namespace DrhLib.Streams
{
	public class ByteStreamWriter :
		IByteStreamWriter,
		IBitStreamWriter
	{
		private MemoryStream stream;
		private MemoryStreamPool pool;
		private byte buffer;
		private const byte maxBitMask = 1 << 7;
		private byte mask = maxBitMask;
		private long bitLength;

		private bool flush;

		public long Length => bitLength >> 3;
		public long BitLength => bitLength;

		public MemoryStream Stream => stream;

		public ByteStreamWriter(int width, long byteLength)
		{
			pool = new MemoryStreamPool();
			pool.Capacity = width + (width / 2);

			stream = new MemoryStream((int)byteLength);
		}

		public IBitStreamWriter Create()
		{
			return new BitStreamWriter(pool);
		}

		public void WriteAndDispose(IBitStreamWriter writer)
		{
			var writer2 = (BitStreamWriter)writer;
			var stream2 = writer2.Stream;
			var length = stream2.Length;
			var bytes = stream2.GetBuffer();
			for (int index = 0; index < length; index++)
			{
				var value = bytes[index];
				Write(value, 8);
			}

			var mask3 = maxBitMask;
			var mask2 = writer2.Mask;
			var buffer2 = writer2.Buffer;
			while (mask3 != mask2)
			{
				Write((buffer2 & mask3) != 0);
				mask3 >>= 1;
			}

			writer2.Dispose();
		}

		public void Write(long value, int count)
		{
			for (int index = count - 1; index >= 0; index--)
			{
				var mask = 1L << index;
				Write((value & mask) != 0);
			}
		}

		public void Write(bool value)
		{
			bitLength++;

			if (value)
				buffer |= mask;

			mask >>= 1;
			if (mask == 0)
			{
				stream.WriteByte(buffer);
				buffer = 0;
				mask = maxBitMask;
			}
		}

		public void Flush()
		{
			if (!flush)
			{
				flush = true;

				Number7Utility.Write(this, bitLength);

				while (mask != maxBitMask)
					Write(false);

				stream.Flush();
			}
		}

		public void Dispose()
		{
			if (stream != null)
			{
				Flush();

				stream.Dispose();
				stream = null;

				pool.Dispose();
				pool = null;
			}
		}
	}
}
