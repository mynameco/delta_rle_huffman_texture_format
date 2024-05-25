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
			IComputeRle rle,
			Color32[] pixels,
			int width,
			int height,
			int channelCount,
			bool useAsync,
			bool checkAlgorithms,
			AlgorithmStatistics byteStatistics)
		{
			var lineState = new EncodeLineState(rle, width, channelCount);

			var pixelsSize = pixels.Length;

			var startTime = DateTime.UtcNow;
			var needNewLine = false;

			var lineIndex = 0;
			for (int offset = 0; offset < pixelsSize; offset += width, lineIndex++)
			{
				lineState.EncodeLine(writer, pixels, offset, lineIndex, useAsync, checkAlgorithms, byteStatistics);

				var endTime = DateTime.UtcNow;
				var deltaTime = endTime - startTime;
				if (deltaTime.TotalSeconds > 2)
				{
					startTime = endTime;
					needNewLine = true;
					Console.Write(((long)offset * 100 / pixelsSize) + " % , ");
				}
			}

			if (needNewLine)
				Console.WriteLine();
		}
	}
}
