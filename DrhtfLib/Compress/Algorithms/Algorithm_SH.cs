using DrhLib.Huffmans;
using DrhLib.Rles;
using DrhLib.Streams;
using DrhtfLib.Commons;
using DrhtfLib.Huffmans;

namespace DrhLib.Compress.Algorithms
{
	public class Algorithm_SH :
		ICompressAlgorithm
	{
		private ComputeRle rle;
		private bool resetAll;

		private HuffmanTable[] infos;

		public Algorithm_SH(ComputeRle rle, int channelCount, bool resetAll)
		{
			this.rle = rle;
			this.resetAll = resetAll;

			infos = new HuffmanTable[channelCount];

			for (int index = 0; index < infos.Length; index++)
			{
				infos[index] = new HuffmanTable(256);
				HuffmanTableUtility.LoadFirstTable(infos[index], rle);
			}
		}

		public void Write(IBitStreamWriter writer, Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, ref int rleCount)
		{
			var info = infos[channel];

			EncodeTableUtility.EncodeSH(writer, values, channel, lineIndex, kind, info, rle, ref rleCount);
		}

		public void Read(IBitStreamReader reader, Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind)
		{
			var info = infos[channel];

			DecodeTableUtility.DecodeSH(reader, values, channel, lineIndex, kind, info, rle);
		}

		public void UpdateTable(Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind)
		{
			var info = infos[channel];

			UpdateTableUtility.UpdateTableSH(values, channel, lineIndex, kind, info, resetAll, ref info.MinZeroCount, rle);
		}
	}
}
