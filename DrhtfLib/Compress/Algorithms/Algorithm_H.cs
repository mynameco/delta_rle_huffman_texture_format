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
		private HuffmanTable[] tables2;
		private HuffmanTable[] tables3;

		public Algorithm_H(ComputeRle rle, int channelCount, bool resetAll)
		{
			this.rle = rle;
			this.resetAll = resetAll;

			CreateTables(ref tables, rle, channelCount, 256);
			CreateTables(ref tables2, null, channelCount, 256);
			CreateTables(ref tables3, null, channelCount, 256);
		}

		private static void CreateTables(ref HuffmanTable[] tables, ComputeRle rle, int channelCount, int count)
		{
			tables = new HuffmanTable[channelCount];

			for (int index = 0; index < tables.Length; index++)
			{
				tables[index] = new HuffmanTable(count);
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
			var table2 = tables2[channel];
			var table3 = tables3[channel];

			UpdateTableUtility.UpdateTableH(values, channel, lineIndex, kind, table, true, resetAll, rle);
		}
	}
}
