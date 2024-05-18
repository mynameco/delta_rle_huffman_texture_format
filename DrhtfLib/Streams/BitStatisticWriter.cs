namespace DrhLib.Streams
{
	public class BitStatisticWriter :
		IBitStreamWriter
	{
		public long BitLength { get; private set; }

		public void Write(long value, int count)
		{
			BitLength += count;
		}

		public void Write(bool value)
		{
			BitLength++;
		}

		public void Dispose()
		{
		}
	}
}
