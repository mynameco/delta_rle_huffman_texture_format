using DrhLib.Compress;

namespace DrhLib.Statistics
{
	public class AlgorithmStatistics
	{
		public long[] Tables = new long[(int)AlgorithmKind.Count];
		public long Rle;
		public long DeltaH;
		public long DeltaV;
		public long DeltaHV;
	}
}
