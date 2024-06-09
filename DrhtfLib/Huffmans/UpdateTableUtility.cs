using DrhLib.Compress;
using DrhLib.Rles;
using DrhtfLib.Commons;

namespace DrhLib.Huffmans
{
	public static class UpdateTableUtility
	{
		public static void UpdateTableH(Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, HuffmanTable table, bool resetAll, ref int minZeroCount, ComputeRle rle)
		{
			table.PrepareTable(rle, resetAll);

			var tmpCodes = table.TmpCodes;

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
							table.RleCode.Count++;
							index += count - 1;
							continue;
						}
					}
				}

				tmpCodes[value].Count++;
			}

			table.ComputeTable(rle);
		}

		public static void UpdateTableSH(Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, HuffmanTable table, bool resetAll, ref int minZeroCount, ComputeRle rle)
		{
			table.PrepareTable(rle, resetAll);

			var prevSign = false;

			var tmpCodes = table.TmpCodes;

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
							table.RleCode.Count++;
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

			table.ComputeTable(rle);
		}
	}
}
