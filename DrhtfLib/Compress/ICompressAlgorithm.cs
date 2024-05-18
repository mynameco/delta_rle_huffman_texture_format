using DrhLib.Streams;
using DrhLib.Utility;

namespace DrhLib.Compress
{
	public interface ICompressAlgorithm
	{
		void Write(IBitStreamWriter writer, Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, ref int rleCount);
		void Read(IBitStreamReader reader, Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind);
		void UpdateTable(Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind);
	}
}
