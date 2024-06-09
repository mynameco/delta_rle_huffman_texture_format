﻿using DrhLib.Compress;
using DrhLib.Rles;
using DrhLib.Streams;
using DrhtfLib.Commons;

namespace DrhLib.Huffmans
{
	public static class EncodeTableUtility
	{
		public static void EncodeH(IBitStreamWriter writer, Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, HuffmanTable table, ComputeRle rle, ref int rleCount)
		{
			var rleEntry = table.RleCode;
			var zeroEntry = table.ZeroCode;

			for (int index = 0; index < values.Length; index++)
			{
				var value = values[index][channel];

				if (value == 0)
				{
					if (rle != null)
					{
						if (rle.Compute(writer, values, channel, ref index, rleEntry, zeroEntry, ref rleCount))
							continue;
					}
				}

				var entry = table.GetEntry(value);
				writer.Write(entry.Code, entry.Size);
			}
		}

		public static void EncodeSH(IBitStreamWriter writer, Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, HuffmanTable table, ComputeRle rle, ref int rleCount)
		{
			var rleEntry = table.RleCode;
			var zeroEntry = table.ZeroCode;

			var prevSign = false;

			for (int index = 0; index < values.Length; index++)
			{
				var bb = values[index][channel];
				var value = (int)(sbyte)bb;

				if (value == 0)
				{
					if (rle != null)
					{
						if (rle.Compute(writer, values, channel, ref index, rleEntry, zeroEntry, ref rleCount))
							continue;
					}
				}
				else if (value != -128)
				{
					var sign = value < 0;
					if (prevSign != sign)
					{
						if (value > 0)
							value = -value;
						prevSign = sign;
					}
					else
					{
						if (value < 0)
							value = -value;
					}
				}

				var bbb = (byte)value;
				var entry = table.GetEntry(bbb);
				writer.Write(entry.Code, entry.Size);
			}
		}
	}
}
