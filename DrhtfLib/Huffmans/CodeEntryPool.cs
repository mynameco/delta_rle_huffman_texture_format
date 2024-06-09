namespace DrhLib.Huffmans
{
	public class CodeEntryPool
	{
		private List<CodeEntry> entries;
		private int freeIndex = 0;

		public CodeEntryPool(int count)
		{
			entries = new List<CodeEntry>(count);
		}

		public CodeEntry Get()
		{
			if (freeIndex >= entries.Count)
			{
				var result = new CodeEntry();
				entries.Add(result);
				freeIndex++;
				return result;
			}

			var result2 = entries[freeIndex];
			freeIndex++;

			result2.Reset();
			return result2;
		}

		public void Reset()
		{
			freeIndex = 0;
		}
	}
}
