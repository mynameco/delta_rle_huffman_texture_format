using DrhLib.Rles;

namespace DrhLib.Huffmans
{
	public class ChannelInfo
	{
		public List<CodeEntry> Codes;
		public List<CodeEntry> TmpCodes;
		public CodeEntry RleCode;

		public CodeEntryPool Pool;

		public int MinZeroCount = 3;

		public ChannelInfo(int count)
		{
			Codes = new List<CodeEntry>(count);
			TmpCodes = new List<CodeEntry>(count + 1);
			RleCode = new CodeEntry() { Index = 2001 };

			Pool = new CodeEntryPool(count);

			for (int index = 0; index < count; index++)
			{
				var code = new CodeEntry();
				code.Index = index;
				Codes.Add(code);
			}
		}

		public void LoadFirstTable(ComputeRle rle)
		{
			UpdateTableUtility.PrepareTable(this, rle);

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
