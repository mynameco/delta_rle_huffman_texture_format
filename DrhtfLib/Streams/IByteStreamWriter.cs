using System;

namespace DrhLib.Streams
{
	public interface IByteStreamWriter :
		IDisposable
	{
		long Length { get; }
		long BitLength { get; }

		IBitStreamWriter Create();
		void WriteAndDispose(IBitStreamWriter stream);
		void Flush();
	}
}
