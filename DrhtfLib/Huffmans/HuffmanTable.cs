namespace DrhLib.Huffmans
{
	public class HuffmanTable
	{
		public List<CodeEntry> Codes;
		public List<CodeEntry> TmpCodes;
		public CodeEntry RleCode;

		public CodeEntryPool Pool;

		public int MinZeroCount = 3;

		public HuffmanTable(int count)
		{
			Codes = new List<CodeEntry>(count);
			TmpCodes = new List<CodeEntry>(count + 1);
			RleCode = new CodeEntry() { Index = 1001 };

			Pool = new CodeEntryPool(count);

			for (int index = 0; index < count; index++)
			{
				var code = new CodeEntry();
				code.Index = index;
				Codes.Add(code);
			}
		}

		public void Cleanup()
		{
			for (int index = 0; index < Codes.Count; index++)
			{
				var code = Codes[index];
				code.Cleanup();
			}

			RleCode.Cleanup();
		}

		public void CleanupWithoutCount()
		{
			for (int index = 0; index < Codes.Count; index++)
			{
				var code = Codes[index];
				code.CleanupWithoutCount();
			}

			RleCode.CleanupWithoutCount();
		}
	}
}
