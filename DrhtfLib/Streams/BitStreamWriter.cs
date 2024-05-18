namespace DrhLib.Streams
{
	public class BitStreamWriter :
		IBitStreamWriter
	{
		private MemoryStream stream;
		private MemoryStreamPool pool;
		private byte buffer;
		private const byte maxBitMask = 1 << 7;
		private byte mask = maxBitMask;

		public long BitLength { get; private set; }

		public MemoryStream Stream => stream;
		public byte Buffer => buffer;
		public byte Mask => mask;

		public BitStreamWriter(MemoryStreamPool pool)
		{
			this.pool = pool;
			stream = pool.Get();
		}

		public IBitStreamReader CreateReader()
		{
			var count = BitLength >> 3;
			var part = BitLength & 0b_111;
			if (part != 0)
				count++;

			var data = new byte[count];
			var source = stream.GetBuffer();
			Array.Copy(source, data, stream.Length);

			if (part != 0)
				data[data.Length - 1] = buffer;

			return new BitStreamReader(data, BitLength);
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
			BitLength++;

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

		public void Dispose()
		{
			if (stream != null)
			{
				pool.Release(stream);
				pool = null;
				stream = null;
			}
		}
	}
}
