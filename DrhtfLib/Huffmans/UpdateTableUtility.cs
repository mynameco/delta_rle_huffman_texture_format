using DrhLib.Compress;
using DrhLib.Rles;
using DrhtfLib.Commons;

namespace DrhLib.Huffmans
{
	public static class UpdateTableUtility
	{
		public static void PrepareTable(ChannelInfo info, ComputeRle rle)
		{
			info.Pool.Reset();

			var tmpCodes = info.TmpCodes;
			tmpCodes.Clear();

			for (int index = 0; index < info.Codes.Count; index++)
			{
				var entry = info.Codes[index];
				tmpCodes.Add(entry);
			}

			if (rle != null)
				tmpCodes.Add(info.RleCode);
		}

		public static void ComputeTable(ChannelInfo info)
		{
			var tmpCodes = info.TmpCodes;

			tmpCodes.Sort(SortCodeEntry);

			int zeroStartIndex;
			for (zeroStartIndex = 0; zeroStartIndex < tmpCodes.Count; zeroStartIndex++)
			{
				var count = tmpCodes[zeroStartIndex].Count;
				if (count == 0)
					break;
			}

			while (tmpCodes.Count > 1)
			{
				var lastIndex = tmpCodes.Count - 1;
				var entry = info.Pool.Get();

				entry.Entry0 = tmpCodes[lastIndex - 1];
				entry.Entry1 = tmpCodes[lastIndex];

				tmpCodes.RemoveAt(lastIndex);
				tmpCodes.RemoveAt(lastIndex - 1);

				entry.Count = entry.Entry0.Count + entry.Entry1.Count;

				CodeEntryUtility.UpLastEntry(info, entry, zeroStartIndex);
			}
		}

		private static int SortCodeEntry(CodeEntry a, CodeEntry b)
		{
			if (b.Count == a.Count)
			{
				var aa = (int)(sbyte)a.Index;
				if (aa < 0)
					aa = -aa;

				var bb = (int)(sbyte)b.Index;
				if (bb < 0)
					bb = -bb;

				return aa.CompareTo(bb);
			}

			return b.Count.CompareTo(a.Count);
		}

		public static void UpdateTableH(Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, ChannelInfo info, bool resetAll, ref int minZeroCount, ComputeRle rle)
		{
			if (resetAll)
				info.Cleanup();
			else
				info.CleanupWithoutCount();

			PrepareTable(info, rle);

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

			ComputeTable(info);

			CodeEntryUtility.UpdateCodes(info);

			if (rle != null)
			{
				var rleEntry = info.RleCode;
				var zeroSize = info.Codes[0].Size;
				var minCode = rleEntry.Size + rle.MinSize;
				minZeroCount = minCode / zeroSize;
			}
		}

		public static void UpdateTableSH(Span<Color32> values, int channel, int lineIndex, AlgorithmKind kind, ChannelInfo info, bool resetAll, ref int minZeroCount, ComputeRle rle)
		{
			if (resetAll)
				info.Cleanup();
			else
				info.CleanupWithoutCount();

			PrepareTable(info, rle);

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

			ComputeTable(info);

			CodeEntryUtility.UpdateCodes(info);

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
