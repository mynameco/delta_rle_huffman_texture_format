using DrhLib.Streams;

namespace DrhLib.Huffmans
{
	public static class CodeEntryUtility
	{
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
