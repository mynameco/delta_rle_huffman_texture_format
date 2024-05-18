using System;
using System.Collections.Generic;
using System.IO;

namespace DrhLib.Streams
{
	public class MemoryStreamPool :
		IDisposable
	{
		private List<MemoryStream> pool = new List<MemoryStream>();

		public int Capacity { get; set; }

		public MemoryStream Get(int capacity = 0)
		{
			lock (pool)
			{
				if (pool.Count == 0)
					return new MemoryStream(capacity != 0 ? capacity : Capacity);

				var result = pool[pool.Count - 1];
				pool.RemoveAt(pool.Count - 1);
				return result;
			}
		}

		public void Release(MemoryStream stream)
		{
			lock (pool)
			{
				stream.SetLength(0);
				pool.Add(stream);
			}
		}

		public void Dispose()
		{
			lock (pool)
			{
				foreach (var stream in pool)
					stream.Dispose();

				pool.Clear();
			}
		}
	}
}
