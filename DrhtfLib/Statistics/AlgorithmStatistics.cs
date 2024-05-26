using DrhLib.Compress;
using DrhtfLib.Utility;

namespace DrhLib.Statistics
{
	public class AlgorithmStatistics
	{
		public long[,] Algorithms = new long[4, (int)AlgorithmKind.Count];
		public long[] Rle = new long[4];
		public long[] Length = new long[4];
		public long[] Delta = new long[(int)DeltaKind.Count];
	}
}
