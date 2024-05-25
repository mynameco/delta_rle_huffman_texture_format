using DrhLib.Rles;
using DrhLib.Streams;
using DrhtfLib.Commons;
using DrhtfLib.Encoders;

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
			var decoder = new Decoder(rle, width, height, channelCount);
			return decoder.DecodeLines(reader, pixels, useAsync);
		}
	}
}
