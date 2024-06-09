using DrhLib.Huffmans;
using DrhLib.Streams;
using DrhLib.Utility;
using DrhtfLib.Commons;

namespace DrhLib.Rles
{
	public class ComputeRle
	{
		public int MinSize => Number4Utility.MinSize;

		public int GetCount(Span<Color32> values, int channel, int startIndex)
		{
			var count = 0;

			for (var index = startIndex; index < values.Length; index++)
			{
				if (values[index][channel] != 0)
					break;

				count++;
			}

			return count;
		}

		public bool Compute(IBitStreamWriter writer, Span<Color32> values, int channel, ref int index, CodeEntry rleEntry, CodeEntry zeroEntry, ref int rleCount)
		{
			var minCode = rleEntry.Size + MinSize;
			var minCodeCount = minCode / zeroEntry.Size;

			var count = GetCount(values, channel, index);
			if (count <= minCodeCount)
				return false;

			count--;

			var count2 = count - minCodeCount;
			writer.Write(rleEntry.Code, rleEntry.Size);
			Write(writer, count2);
			rleCount++;

			index += count;
			return true;
		}

		public void Write(IBitStreamWriter writer, long number)
		{
			Number4Utility.Write(writer, number);
		}

		public long Read(IBitStreamReader reader)
		{
			return Number4Utility.Read(reader);
		}
	}
}
