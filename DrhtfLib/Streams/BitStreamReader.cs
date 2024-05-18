using System;

namespace DrhLib.Streams
{
	public class BitStreamReader :
		IBitStreamReader
	{
		private byte[] bytes;
		private long bitIndex;
		private long bitLength;
		private const byte maxBitMask = 1 << 7;

		public long BitLength => bitLength;
		public long BitIndex => bitIndex;

		public BitStreamReader(byte[] bytes, long bitLength)
		{
			this.bytes = bytes;
			this.bitLength = bitLength;
		}

		public long Read(int count)
		{
			var result = 0L;
			for (int index = 0; index < count; index++)
			{
				result <<= 1;
				result |= Read() ? 1L : 0L;
			}

			return result;
		}

		public bool Read()
		{
			var index = bitIndex >> 3;
			var part = bitIndex & 0b_111;
			bitIndex++;

			if (bitIndex > bitLength)
				throw new Exception("index : " + bitIndex + " , length : " + bitLength);

			var value = bytes[index];
			var result = value & (maxBitMask >> (int)part);
			return result != 0;
		}

		public void Dispose()
		{
		}
	}
}
