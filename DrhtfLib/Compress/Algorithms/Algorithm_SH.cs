using DrhLib.Huffmans;
using DrhLib.Rles;
using DrhLib.Streams;
using DrhLib.Utility;

namespace DrhLib.Compress.Algorithms
{
	public class Algorithm_SH :
		ICompressAlgorithm
	{
		private IComputeRle rle;
		private bool resetAll;
		private ChannelInfo[] infos;

		public Algorithm_SH(IComputeRle rle, int channelCount, bool resetAll)
		{
			this.rle = rle;
			this.resetAll = resetAll;

			infos = new ChannelInfo[channelCount];

			for (int index = 0; index < infos.Length; index++)
			{
				infos[index] = new ChannelInfo();
				infos[index].LoadFirstTable(rle);
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
