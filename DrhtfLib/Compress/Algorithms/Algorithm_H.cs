using DrhLib.Huffmans;
using DrhLib.Rles;
using DrhLib.Streams;
using DrhtfLib.Commons;

namespace DrhLib.Compress.Algorithms
{
	public class Algorithm_H :
		ICompressAlgorithm
	{
		private ComputeRle rle;
		private bool resetAll;

		private ChannelInfo[] infos;

		public Algorithm_H(ComputeRle rle, int channelCount, bool resetAll)
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

			EncodeTableUtility.EncodeH(writer, values, channel, lineIndex, kind, info, rle, ref rleCount);
		}

		public void Read(IBitStreamReader reader, Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind)
		{
			var info = infos[channel];

			DecodeTableUtility.DecodeH(reader, values, channel, lineIndex, kind, info, rle);
		}

		public void UpdateTable(Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind)
		{
			var info = infos[channel];

			UpdateTableUtility.UpdateTableH(values, channel, lineIndex, kind, info, resetAll, ref info.MinZeroCount, rle);
		}
	}
}
