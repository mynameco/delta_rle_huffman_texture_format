using DrhLib.Compress;
using DrhLib.Rles;
using DrhLib.Streams;
using DrhtfLib.Commons;

namespace DrhLib.Huffmans
{
	public static class DecodeTableUtility
	{
		public static void DecodeH(IBitStreamReader reader, Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, HuffmanTable table, ComputeRle rle)
		{
			var rleEntry = table.RleCode;
			var zeroEntry = table.ZeroCode;

			var minCodeCount = 0;
			if (rle != null)
			{
				var minCode = rleEntry.Size + rle.MinSize;
				minCodeCount = minCode / zeroEntry.Size;
			}

			var entry = table.TmpCodes[0];

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

		public static void DecodeSH(IBitStreamReader reader, Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, HuffmanTable table, ComputeRle rle)
		{
			var rleEntry = table.RleCode;
			var zeroEntry = table.ZeroCode;

			var minCodeCount = 0;
			if (rle != null)
			{
				var minCode = rleEntry.Size + rle.MinSize;
				minCodeCount = minCode / zeroEntry.Size;
			}

			var entry = table.TmpCodes[0];

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
