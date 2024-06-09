using DrhLib.Huffmans;
using DrhLib.Rles;

namespace DrhtfLib.Huffmans
{
	public static class HuffmanTableUtility
	{
		public static void LoadDefaultTable(HuffmanTable table, ComputeRle rle)
		{
			PrepareTable(table, rle);

			var max = 16;
			var count = 1;
			for (int index = max; index >= 0; index--)
			{
				table.Codes[index].Count = count;
				table.Codes[(byte)-index].Count = count;

				count <<= 1;
			}

			ComputeTable(table);

			for (int index = max; index >= 0; index--)
			{
				table.Codes[index].Count = 0;
				table.Codes[(byte)-index].Count = 0;
			}
		}

		public static void PrepareTable(HuffmanTable table, ComputeRle rle)
		{
			table.Pool.Reset();

			var tmpCodes = table.TmpCodes;
			tmpCodes.Clear();

			for (int index = 0; index < table.Codes.Count; index++)
			{
				var entry = table.Codes[index];
				tmpCodes.Add(entry);
			}

			if (rle != null)
				tmpCodes.Add(table.RleCode);
		}

		public static void ComputeTable(HuffmanTable table)
		{
			var tmpCodes = table.TmpCodes;

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
				var entry = table.Pool.Get();

				entry.Entry0 = tmpCodes[lastIndex - 1];
				entry.Entry1 = tmpCodes[lastIndex];

				tmpCodes.RemoveAt(lastIndex);
				tmpCodes.RemoveAt(lastIndex - 1);

				entry.Count = entry.Entry0.Count + entry.Entry1.Count;

				CodeEntryUtility.UpLastEntry(table, entry, zeroStartIndex);
			}

			var codeEntry = table.TmpCodes[0];
			codeEntry.Code = 0;
			codeEntry.Size = 0;

			UpdateCodesInternal(codeEntry);
		}

		private static void UpdateCodesInternal(CodeEntry codeEntry)
		{
			if (codeEntry.Entry0 != null)
			{
				codeEntry.Entry0.Size = codeEntry.Size + 1;
				codeEntry.Entry0.Code = codeEntry.Code << 1;
				UpdateCodesInternal(codeEntry.Entry0);
			}

			if (codeEntry.Entry1 != null)
			{
				codeEntry.Entry1.Size = codeEntry.Size + 1;
				codeEntry.Entry1.Code = (codeEntry.Code << 1) | 1;
				UpdateCodesInternal(codeEntry.Entry1);
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
	}
}
