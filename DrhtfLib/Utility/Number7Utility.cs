using DrhLib.Streams;

namespace DrhLib.Utility
{
	public static class Number7Utility
	{
		public const int MinSize = 7;

		private const int mask = 0b_111111;
		private const int count = MinSize - 1;

		public static void Write(IBitStreamWriter writer, long number)
		{
			if (number < 0)
				throw new Exception();

			do
			{
				var part = number & mask;
				writer.Write((int)part, count);
				number >>= count;
				writer.Write(number > 0);
			}
			while (number != 0);
		}

		public static long Read(IBitStreamReader reader)
		{
			var result = 0L;
			var shift = 0;
			do
			{
				var value = reader.Read(count);
				result |= value << shift;
				shift += count;
			} while (reader.Read());

			return result;
		}
	}
}
