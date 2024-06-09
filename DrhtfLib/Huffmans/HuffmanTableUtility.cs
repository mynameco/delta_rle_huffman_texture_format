using DrhLib.Huffmans;
using DrhLib.Rles;

namespace DrhtfLib.Huffmans
{
	public static class HuffmanTableUtility
	{
		public static void LoadDefaultTable(HuffmanTable table, ComputeRle rle)
		{
			table.PrepareTable(rle, true);

			// TODO для нуля два раза добавляется, любые изменения тут, скачет размер
			var max = 16;
			var count = 1;
			for (int index = max; index >= 0; index--)
			{
				table.AddCount(index, count);
				table.AddCount((byte)-index, count);

				count <<= 1;
			}

			table.ComputeTable(null);

			table.Cleanup();
		}
	}
}
