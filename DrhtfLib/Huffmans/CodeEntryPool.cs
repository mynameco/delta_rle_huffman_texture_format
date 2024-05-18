using System.Collections.Generic;

namespace DrhLib.Huffmans
{
	public class CodeEntryPool
	{
		private List<CodeEntry> entries = new List<CodeEntry>(258);
		private List<CodeEntry> usedEntries = new List<CodeEntry>(258);

		public CodeEntry Get()
		{
			if (entries.Count == 0)
			{
				var result = new CodeEntry();
				usedEntries.Add(result);
				return result;
			}

			var result2 = entries[entries.Count - 1];
			entries.RemoveAt(entries.Count - 1);
			usedEntries.Add(result2);

			result2.Reset();
			return result2;
		}

		public void Reset()
		{
			entries.AddRange(usedEntries);
			usedEntries.Clear();
		}
	}
}
