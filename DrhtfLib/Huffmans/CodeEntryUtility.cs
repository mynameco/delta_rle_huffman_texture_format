using DrhLib.Streams;

namespace DrhLib.Huffmans
{
	public static class CodeEntryUtility
	{
		public static void UpLastEntry(HuffmanTable info, CodeEntry entry, int zeroStartIndex)
		{
			var tmpCodes = info.TmpCodes;
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

		public static CodeEntry GetCode(IBitStreamReader reader, CodeEntry entry)
		{
			while (true)
			{
				if (entry.Index != -1)
					return entry;

				var value = reader.Read();
				entry = value ? entry.Entry1 : entry.Entry0;
			}
		}
	}
}
