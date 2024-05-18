using DrhLib.Huffmans;
using DrhLib.Rles;
using DrhLib.Streams;
using DrhLib.Utility;
using System;

namespace DrhLib.Compress.Algorithms
{
	public class Algorithm_H :
		ICompressAlgorithm
	{
		private IComputeRle rle;
		private ChannelInfo[] infos;

		public Algorithm_H(IComputeRle rle, int channelCount)
		{
			this.rle = rle;

			infos = new ChannelInfo[channelCount];

			for (int index = 0; index < infos.Length; index++)
			{
				infos[index] = new ChannelInfo();
				infos[index].LoadFirstTable();
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

			UpdateTableUtility.UpdateTableH(values, channel, lineIndex, kind, info, true, ref info.MinZeroCount, rle);
		}
	}
}
