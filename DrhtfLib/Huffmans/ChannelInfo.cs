using System.Collections.Generic;

namespace DrhLib.Huffmans
{
	public class ChannelInfo
	{
		public List<CodeEntry> Codes = new List<CodeEntry>(258);
		public List<CodeEntry> TmpCodes = new List<CodeEntry>(258);
		public CodeEntry RleCode = new CodeEntry() { Index = 1001 };
		public CodeEntry SignCode = new CodeEntry() { Index = 1002 };

		public CodeEntryPool Pool = new CodeEntryPool();

		public int MinZeroCount = 3;

		public void LoadFirstTable()
		{
			for (int index = 0; index < 256; index++)
			{
				var code = new CodeEntry();
				code.Index = index;
				Codes.Add(code);
			}

			UpdateTableUtility.PrepareTable(this);

			var max = 16;
			var count = 1;
			for (int index = max; index >= 0; index--)
			{
				Codes[index].Count = count;
				Codes[(byte)-index].Count = count;

				count <<= 1;
			}

			UpdateTableUtility.ComputeTable(this);

			CodeEntryUtility.UpdateCodes(this);

			for (int index = max; index >= 0; index--)
			{
				Codes[index].Count = 0;
				Codes[(byte)-index].Count = 0;
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
			SignCode.Cleanup();
		}

		public void CleanupWithoutCount()
		{
			for (int index = 0; index < Codes.Count; index++)
			{
				var code = Codes[index];
				code.CleanupWithoutCount();
			}

			RleCode.CleanupWithoutCount();
			SignCode.Cleanup();
		}
	}
}
