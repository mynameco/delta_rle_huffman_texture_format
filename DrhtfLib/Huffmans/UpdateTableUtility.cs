using DrhLib.Compress;
using DrhLib.Rles;
using DrhtfLib.Commons;
using DrhtfLib.Huffmans;

namespace DrhLib.Huffmans
{
	public static class UpdateTableUtility
	{
		public static void UpdateTableH(Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, HuffmanTable info, bool resetAll, ref int minZeroCount, ComputeRle rle)
		{
			if (resetAll)
				info.Cleanup();
			else
				info.CleanupWithoutCount();

			HuffmanTableUtility.PrepareTable(info, rle);

			var tmpCodes = info.TmpCodes;

			for (int index = 0; index < values.Length; index++)
			{
				var value = values[index][channel];

				if (value == 0)
				{
					if (rle != null)
					{
						var count = rle.GetCount(values, channel, index);
						if (count >= minZeroCount)
						{
							info.RleCode.Count++;
							index += count - 1;
							continue;
						}
					}
				}

				tmpCodes[value].Count++;
			}

			HuffmanTableUtility.ComputeTable(info);

			HuffmanTableUtility.UpdateCodes(info);

			if (rle != null)
			{
				var rleEntry = info.RleCode;
				var zeroSize = info.Codes[0].Size;
				var minCode = rleEntry.Size + rle.MinSize;
				minZeroCount = minCode / zeroSize;
			}
		}

		public static void UpdateTableSH(Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, HuffmanTable info, bool resetAll, ref int minZeroCount, ComputeRle rle)
		{
			if (resetAll)
				info.Cleanup();
			else
				info.CleanupWithoutCount();

			HuffmanTableUtility.PrepareTable(info, rle);

			var prevSign = false;

			var tmpCodes = info.TmpCodes;

			for (int index = 0; index < values.Length; index++)
			{
				var value = (int)(sbyte)values[index][channel];

				if (value == 0)
				{
					if (rle != null)
					{
						var count = rle.GetCount(values, channel, index);
						if (count >= minZeroCount)
						{
							info.RleCode.Count++;
							index += count - 1;
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

				if (value < 0)
					value = 256 + value;

				tmpCodes[value].Count++;
			}

			HuffmanTableUtility.ComputeTable(info);

			HuffmanTableUtility.UpdateCodes(info);

			if (rle != null)
			{
				var rleEntry = info.RleCode;
				var zeroSize = info.Codes[0].Size;
				var minCode = rleEntry.Size + rle.MinSize;
				minZeroCount = minCode / zeroSize;
			}
		}
	}
}
