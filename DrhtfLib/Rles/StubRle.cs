using DrhLib.Huffmans;
using DrhLib.Streams;
using DrhLib.Utility;
using System;

namespace DrhLib.Rles
{
	public class StubRle :
		IComputeRle
	{
		public int MinSize => 0;

		public int GetCount(Span<Color32> values, int channel, int startIndex)
		{
			return -1;
		}

		public bool Compute(IBitStreamWriter writer, Span<Color32> values, int channel, ref int index, CodeEntry rleEntry, CodeEntry zeroEntry, ref int rleCount)
		{
			return false;
		}

		public void Write(IBitStreamWriter writer, long number)
		{
		}

		public long Read(IBitStreamReader reader)
		{
			return -1;
		}
	}
}
