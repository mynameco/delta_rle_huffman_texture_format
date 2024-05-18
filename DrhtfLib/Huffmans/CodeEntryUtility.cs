using DrhLib.Streams;

namespace DrhLib.Huffmans
{
	public static class CodeEntryUtility
	{
		public static void UpLastEntry(ChannelInfo info, CodeEntry entry, int zeroStartIndex)
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

		public static void UpdateCodes(ChannelInfo info)
		{
			var codeEntry = info.TmpCodes[0];
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
