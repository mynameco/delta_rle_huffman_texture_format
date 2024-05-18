using System;

namespace DrhLib.Streams
{
	public interface IBitStreamWriter :
		IDisposable
	{
		long BitLength { get; }

		void Write(long value, int count);
		void Write(bool value);
	}
}
