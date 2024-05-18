using DrhLib.Huffmans;
using DrhLib.Streams;
using DrhLib.Utility;

namespace DrhLib.Rles
{
	public interface IComputeRle
	{
		int MinSize { get; }
		int GetCount(Span<Color32> values, int channel, int startIndex);
		bool Compute(IBitStreamWriter writer, Span<Color32> values, int channel, ref int index, CodeEntry rleEntry, CodeEntry zeroEntry, ref int rleCount);
		void Write(IBitStreamWriter writer, long number);
		long Read(IBitStreamReader reader);
	}
}
