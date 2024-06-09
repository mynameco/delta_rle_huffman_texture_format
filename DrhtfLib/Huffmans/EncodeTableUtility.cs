using DrhLib.Compress;
using DrhLib.Rles;
using DrhLib.Streams;
using DrhtfLib.Commons;

namespace DrhLib.Huffmans
{
	public static class EncodeTableUtility
	{
		public static void EncodeH(IBitStreamWriter writer, Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, HuffmanTable table, HuffmanTable table2, HuffmanTable table3, ComputeRle rle, ref int rleCount)
		{
			var rleEntry = table.RleCode;
			var zeroEntry = table.ZeroCode;

			var prevRle = false;
			var prevZero = false;

			for (int index = 0; index < values.Length; index++)
			{
				var value = values[index][channel];

				if (value == 0 &&
					!prevZero)
				{
					if (rle != null)
					{
						if (rle.Compute(writer, values, channel, ref index, rleEntry, zeroEntry, ref rleCount))
						{
							prevRle = true;
							continue;
						}
					}
				}

				CodeEntry entry = null;
				if (prevRle)
				{
					entry = table2.GetEntry(value);
					prevRle = false;
				}
				else if (prevZero)
				{
					entry = table3.GetEntry(value);
				}
				else
				{
					entry = table.GetEntry(value);
				}

				writer.Write(entry.Code, entry.Size);

				if (value == 0)
					prevZero = true;
				else
					prevZero = false;
			}
		}

		public static void EncodeSH(IBitStreamWriter writer, Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, HuffmanTable table, HuffmanTable table2, HuffmanTable table3, ComputeRle rle, ref int rleCount)
		{
			var rleEntry = table.RleCode;
			var zeroEntry = table.ZeroCode;

			var prevSign = false;
			var prevRle = false;
			var prevZero = false;

			for (int index = 0; index < values.Length; index++)
			{
				var bb = values[index][channel];
				var value = (int)(sbyte)bb;

				if (value == 0 &&
					!prevZero)
				{
					if (rle != null)
					{
						if (rle.Compute(writer, values, channel, ref index, rleEntry, zeroEntry, ref rleCount))
						{
							prevRle = true;
							continue;
						}
					}
				}

				if (value != 0 &&
					value != -128)
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

				value = (byte)value;
				CodeEntry entry = null;
				if (prevRle)
				{
					entry = table2.GetEntry(value);
					prevRle = false;
				}
				else if (prevZero)
				{
					entry = table3.GetEntry(value);
				}
				else
				{
					entry = table.GetEntry(value);
				}

				writer.Write(entry.Code, entry.Size);

				if (value == 0)
					prevZero = true;
				else
					prevZero = false;
			}
		}
	}
}
