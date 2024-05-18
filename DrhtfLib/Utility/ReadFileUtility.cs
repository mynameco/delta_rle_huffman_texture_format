using DrhLib.Rles;
using DrhLib.Streams;
using System;

namespace DrhLib.Utility
{
	public static class ReadFileUtility
	{
		public static bool ReadHeader(
			IBitStreamReader reader,
			out int version,
			out int width,
			out int height,
			out bool hasAlpha)
		{
			version = 0;
			width = 0;
			height = 0;
			hasAlpha = false;

			var value = reader.Read(8);
			if (value != 'D')
				return false;

			value = reader.Read(8);
			if (value != 'R')
				return false;

			value = reader.Read(8);
			if (value != 'H')
				return false;

			version = (int)Number4Utility.Read(reader);
			width = (int)Number7Utility.Read(reader);
			height = (int)Number7Utility.Read(reader);
			hasAlpha = reader.Read();

			return true;
		}

		public static bool DecodePixels(
			IBitStreamReader reader,
			IComputeRle rle,
			Color32[] pixels,
			int width,
			int height,
			int channelCount,
			bool useAsync)
		{
			var lineState = new DecodeLineState(rle, width, channelCount);

			var pixelsSize = pixels.Length;

			var startTime = DateTime.UtcNow;
			var needNewLine = false;

			var lineIndex = 0;
			for (int offset = 0; offset < pixelsSize; offset += width, lineIndex++)
			{
				if (!lineState.DecodeLine(reader, pixels, offset, lineIndex, useAsync))
					return false;

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

			return true;
		}
	}
}
