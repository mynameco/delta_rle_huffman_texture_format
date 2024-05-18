namespace DrhLib.Huffmans
{
	public class CodeEntry
	{
		public int Index = -1;
		public long Count;
		public int Size;
		public long Code;
		public CodeEntry Entry0;
		public CodeEntry Entry1;

		public void CleanupWithoutCount()
		{
			Entry0 = null;
			Entry1 = null;
		}

		public void Cleanup()
		{
			Count = 0;
			Entry0 = null;
			Entry1 = null;
		}

		public void Reset()
		{
			Index = -1;
			Count = 0;
			Size = 0;
			Code = 0;
			Entry0 = null;
			Entry1 = null;
		}
	}
}
