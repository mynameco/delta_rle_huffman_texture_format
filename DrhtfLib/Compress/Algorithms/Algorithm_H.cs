using DrhLib.Huffmans;
using DrhLib.Rles;
using DrhLib.Streams;
using DrhtfLib.Commons;
using DrhtfLib.Huffmans;

namespace DrhLib.Compress.Algorithms
{
	public class Algorithm_H :
		ICompressAlgorithm
	{
		private ComputeRle rle;
		private bool resetAll;

		private HuffmanTable[] tables;

		public Algorithm_H(ComputeRle rle, int channelCount, bool resetAll)
		{
			this.rle = rle;
			this.resetAll = resetAll;

			tables = new HuffmanTable[channelCount];

			for (int index = 0; index < tables.Length; index++)
			{
				tables[index] = new HuffmanTable(256);
				HuffmanTableUtility.LoadDefaultTable(tables[index], rle);
			}
		}

		public void Write(IBitStreamWriter writer, Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, ref int rleCount)
		{
			var table = tables[channel];

			EncodeTableUtility.EncodeH(writer, values, channel, lineIndex, kind, table, rle, ref rleCount);
		}

		public void Read(IBitStreamReader reader, Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind)
		{
			var table = tables[channel];

			DecodeTableUtility.DecodeH(reader, values, channel, lineIndex, kind, table, rle);
		}

		public void UpdateTable(Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind)
		{
			var table = tables[channel];

			UpdateTableUtility.UpdateTableH(values, channel, lineIndex, kind, table, resetAll, ref table.MinZeroCount, rle);
		}
	}
}
