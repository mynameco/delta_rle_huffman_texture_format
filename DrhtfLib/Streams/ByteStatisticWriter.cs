using DrhLib.Utility;

namespace DrhLib.Streams
{
	public class ByteStatisticWriter :
		IByteStreamWriter,
		IBitStreamWriter
	{
		private long bitLength;

		private bool flush;

		public long Length => bitLength >> 3;
		public long BitLength => bitLength;

		public IBitStreamWriter Create()
		{
			return new BitStatisticWriter();
		}

		public void WriteAndDispose(IBitStreamWriter writer)
		{
			var writer2 = (BitStatisticWriter)writer;
			var length = writer2.BitLength;
			bitLength += length;
		}

		public void Write(long value, int count)
		{
			bitLength += count;
		}

		public void Write(bool value)
		{
			bitLength++;
		}

		public void Flush()
		{
			if (!flush)
			{
				flush = true;

				Number7Utility.Write(this, bitLength);

				var part = bitLength & 0b_111;
				bitLength &= ~0b_111;
				if (part != 0)
					bitLength += 8;
			}
		}

		public void Dispose()
		{
			Flush();

			bitLength = 0;
		}
	}
}
