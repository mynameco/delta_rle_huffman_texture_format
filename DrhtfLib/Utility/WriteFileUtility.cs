using DrhLib.Rles;
using DrhLib.Statistics;
using DrhLib.Streams;
using DrhtfLib.Commons;
using DrhtfLib.Encoders;

namespace DrhLib.Utility
{
	public static class WriteFileUtility
	{
		public static void WriteHeader(
			IByteStreamWriter writer,
			int version,
			int width,
			int height,
			bool hasAlpha)
		{
			var writer2 = writer.Create();

			writer2.Write('D', 8);
			writer2.Write('R', 8);
			writer2.Write('H', 8);

			Number4Utility.Write(writer2, version);

			Number7Utility.Write(writer2, width);
			Number7Utility.Write(writer2, height);

			writer2.Write(hasAlpha);

			writer.WriteAndDispose(writer2);
		}

		public static void EncodePixels(
			IByteStreamWriter writer,
			ComputeRle rle,
			Color32[] pixels,
			int width,
			int height,
			int channelCount,
			bool useAsync,
			bool checkAlgorithms,
			AlgorithmStatistics byteStatistics)
		{
			var encoder = new Encoder(rle, width, height, channelCount);
			encoder.EncodeLines(writer, pixels, useAsync, checkAlgorithms, byteStatistics);
		}
	}
}
