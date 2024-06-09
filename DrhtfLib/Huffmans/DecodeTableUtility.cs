using DrhLib.Compress;
using DrhLib.Rles;
using DrhLib.Streams;
using DrhtfLib.Commons;

namespace DrhLib.Huffmans
{
	public static class DecodeTableUtility
	{
		public static void DecodeH(IBitStreamReader reader, Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, HuffmanTable table, HuffmanTable table2, HuffmanTable table3, ComputeRle rle)
		{
			var rleEntry = table.RleCode;
			var zeroEntry = table.ZeroCode;

			var minCodeCount = 0;
			if (rle != null)
			{
				var minCode = rleEntry.Size + rle.MinSize;
				minCodeCount = minCode / zeroEntry.Size;
			}

			var prevRle = false;
			var prevZero = false;

			for (int index = 0; index < values.Length; index++)
			{
				CodeEntry code;
				if (prevRle)
				{
					code = table2.GetCode(reader);
					prevRle = false;
				}
				else if (prevZero)
				{
					code = table3.GetCode(reader);
				}
				else
				{
					code = table.GetCode(reader);
				}

				if (code == rleEntry &&
					!prevZero)
				{
					var count = (int)rle.Read(reader);
					index += count + minCodeCount;
					prevRle = true;
					continue;
				}

				var value = (byte)code.Index;
				values[index][channel] = value;

				if (value == 0)
					prevZero = true;
				else
					prevZero = false;
			}
		}

		public static void DecodeSH(IBitStreamReader reader, Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, HuffmanTable table, HuffmanTable table2, HuffmanTable table3, ComputeRle rle)
		{
			var rleEntry = table.RleCode;
			var zeroEntry = table.ZeroCode;

			var minCodeCount = 0;
			if (rle != null)
			{
				var minCode = rleEntry.Size + rle.MinSize;
				minCodeCount = minCode / zeroEntry.Size;
			}

			var prevSign = false;
			var prevRle = false;
			var prevZero = false;

			for (int index = 0; index < values.Length; index++)
			{
				CodeEntry code;
				if (prevRle)
				{
					code = table2.GetCode(reader);
					prevRle = false;
				}
				else if (prevZero)
				{
					code = table3.GetCode(reader);
				}
				else
				{
					code = table.GetCode(reader);
				}

				if (code == rleEntry &&
					!prevZero)
				{
					var count = (int)rle.Read(reader);
					index += count + minCodeCount;
					prevRle = true;
					continue;
				}
				else if (code.Index == 0)
				{
					prevZero = true;
					continue;
				}
				else if (code.Index == 128)
				{
					values[index][channel] = 128;
					prevZero = false;
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

				value = (byte)value;
				values[index][channel] = (byte)value;

				prevZero = false;
			}
		}
	}
}
