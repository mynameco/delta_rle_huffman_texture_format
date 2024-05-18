﻿using DrhLib.Compress;
using DrhLib.Rles;
using DrhLib.Streams;
using DrhLib.Utility;

namespace DrhLib.Huffmans
{
	public static class DecodeTableUtility
	{
		public static void DecodeH(IBitStreamReader reader, Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, ChannelInfo info, IComputeRle rle)
		{
			var rleEntry = info.RleCode;
			var zeroEntry = info.Codes[0];

			var minCode = rleEntry.Size + rle.MinSize;
			var minCodeCount = minCode / zeroEntry.Size;

			var entry = info.TmpCodes[0];

			for (int index = 0; index < values.Length; index++)
			{
				var code = CodeEntryUtility.GetCode(reader, entry);

				if (code == rleEntry)
				{
					var count = (int)rle.Read(reader);
					index += count + minCodeCount;
					continue;
				}

				var bb = (byte)code.Index;
				values[index][channel] = bb;
			}
		}

		public static void DecodeSH(IBitStreamReader reader, Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, ChannelInfo info, IComputeRle rle)
		{
			var rleEntry = info.RleCode;
			var zeroEntry = info.Codes[0];

			var minCode = rleEntry.Size + rle.MinSize;
			var minCodeCount = minCode / zeroEntry.Size;

			var entry = info.TmpCodes[0];

			var prevSign = false;

			for (int index = 0; index < values.Length; index++)
			{
				var code = CodeEntryUtility.GetCode(reader, entry);

				if (code == rleEntry)
				{
					var count = (int)rle.Read(reader);
					index += count + minCodeCount;
					continue;
				}
				else if (code.Index == 0)
				{
					continue;
				}
				else if (code.Index == 128)
				{
					values[index][channel] = 128;
					continue;
				}

				var value = (int)(sbyte)(byte)code.Index;

				var sign = value < 0;
				if (sign)
				{
					if (prevSign)
						value = -value;

					prevSign = !prevSign;
				}
				else
				{
					if (prevSign)
						value = -value;
				}

				var bb = (byte)value;
				values[index][channel] = bb;
			}
		}
	}
}
