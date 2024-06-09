using DrhLib.Huffmans;
using DrhLib.Rles;

namespace DrhtfLib.Huffmans
{
	public static class HuffmanTableUtility
	{
		public static void LoadDefaultTable(HuffmanTable table, ComputeRle rle)
		{
			table.PrepareTable(rle, true);

			var max = 16;
			var count = 1;
			for (int index = max; index >= 0; index--)
			{
				table.Codes[index].Count = count;
				table.Codes[(byte)-index].Count = count;

				count <<= 1;
			}

			table.ComputeTable();

			for (int index = max; index >= 0; index--)
			{
				table.Codes[index].Count = 0;
				table.Codes[(byte)-index].Count = 0;
			}
		}
	}
}
