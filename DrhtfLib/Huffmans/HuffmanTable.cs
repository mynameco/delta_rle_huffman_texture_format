﻿using DrhLib.Rles;
using DrhLib.Streams;

namespace DrhLib.Huffmans
{
	public class HuffmanTable
	{
		private List<CodeEntry> codes;
		private List<CodeEntry> tmpCodes;

		private CodeEntryPool pool;

		public CodeEntry ZeroCode => codes[0];
		public CodeEntry RleCode { get; private set; }

		public int MinZeroCount { get; private set; } = 3;

		public HuffmanTable(int count)
		{
			codes = new List<CodeEntry>(count);
			tmpCodes = new List<CodeEntry>(count + 1);
			RleCode = new CodeEntry() { Index = 1001 };

			pool = new CodeEntryPool(count);

			for (int index = 0; index < count; index++)
			{
				var code = new CodeEntry();
				code.Index = index;
				codes.Add(code);
			}
		}

		public CodeEntry GetEntry(int index)
		{
			return codes[index];
		}

		public CodeEntry GetCode(IBitStreamReader reader)
		{
			var entry = tmpCodes[0];

			while (true)
			{
				if (entry.Index != -1)
					return entry;

				var value = reader.Read();
				entry = value ? entry.Entry1 : entry.Entry0;
			}
		}

		public void AddCount(int index, int count = 1)
		{
			tmpCodes[index].Count += count;
		}

		public void Cleanup()
		{
			for (int index = 0; index < codes.Count; index++)
			{
				var code = codes[index];
				code.Cleanup();
			}

			RleCode.Cleanup();
		}

		private void CleanupWithoutCount()
		{
			for (int index = 0; index < codes.Count; index++)
			{
				var code = codes[index];
				code.CleanupWithoutCount();
			}

			RleCode.CleanupWithoutCount();
		}

		public void PrepareTable(ComputeRle rle, bool resetAll)
		{
			if (resetAll)
				Cleanup();
			else
				CleanupWithoutCount();

			pool.Reset();

			tmpCodes.Clear();

			for (int index = 0; index < codes.Count; index++)
			{
				var entry = codes[index];
				tmpCodes.Add(entry);
			}

			if (rle != null)
				tmpCodes.Add(RleCode);
		}

		public void ComputeTable(ComputeRle rle)
		{
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
				var entry = pool.Get();

				entry.Entry0 = tmpCodes[lastIndex - 1];
				entry.Entry1 = tmpCodes[lastIndex];

				tmpCodes.RemoveAt(lastIndex);
				tmpCodes.RemoveAt(lastIndex - 1);

				entry.Count = entry.Entry0.Count + entry.Entry1.Count;

				UpLastEntry(entry, zeroStartIndex);
			}

			var codeEntry = tmpCodes[0];
			codeEntry.Code = 0;
			codeEntry.Size = 0;

			UpdateCodesInternal(codeEntry);

			if (rle != null)
			{
				var rleEntry = RleCode;
				var zeroSize = ZeroCode.Size;
				var minCode = rleEntry.Size + rle.MinSize;
				MinZeroCount = minCode / zeroSize;
			}
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

		private void UpLastEntry(CodeEntry entry, int zeroStartIndex)
		{
			var lastIndex = tmpCodes.Count;

			if (entry.Count == 0)
			{
				if (zeroStartIndex == lastIndex)
				{
					tmpCodes.Insert(lastIndex, entry);
					return;
				}

				tmpCodes.Insert(zeroStartIndex, entry);
				return;
			}

			int insertIndex;
			for (insertIndex = lastIndex; insertIndex >= 1; insertIndex--)
			{
				var prevEntry = tmpCodes[insertIndex - 1];
				if (prevEntry.Count > entry.Count)
					break;
			}

			tmpCodes.Insert(insertIndex, entry);
		}
	}
}
