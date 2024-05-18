namespace DrhLib.Streams
{
	public interface IBitStreamReader :
		IDisposable
	{
		long BitLength { get; }
		long BitIndex { get; }

		long Read(int count);
		bool Read();
	}
}
