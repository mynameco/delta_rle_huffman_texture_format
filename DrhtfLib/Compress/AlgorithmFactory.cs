using DrhLib.Compress.Algorithms;
using DrhLib.Rles;
using DrhtfLib.Utility;

namespace DrhLib.Compress
{
	public class AlgorithmFactory
	{
		private ICompressAlgorithm[,] algorithms = new ICompressAlgorithm[(int)AlgorithmKind.Count, (int)DeltaKind.Count];

		public AlgorithmFactory(ComputeRle rle, int channelCount)
		{
			for (int deltaKind = 0; deltaKind < (int)DeltaKind.Count; deltaKind++)
			{
				algorithms[(int)AlgorithmKind.HA, deltaKind] = new Algorithm_H(null, channelCount, false);
				algorithms[(int)AlgorithmKind.SHA, deltaKind] = new Algorithm_SH(null, channelCount, false);
				algorithms[(int)AlgorithmKind.RHA, deltaKind] = new Algorithm_H(rle, channelCount, false);
				algorithms[(int)AlgorithmKind.RSHA, deltaKind] = new Algorithm_SH(rle, channelCount, false);

				algorithms[(int)AlgorithmKind.H, deltaKind] = new Algorithm_H(null, channelCount, true);
				algorithms[(int)AlgorithmKind.SH, deltaKind] = new Algorithm_SH(null, channelCount, true);
				algorithms[(int)AlgorithmKind.RH, deltaKind] = new Algorithm_H(rle, channelCount, true);
				algorithms[(int)AlgorithmKind.RSH, deltaKind] = new Algorithm_SH(rle, channelCount, true);
			}
		}

		public ICompressAlgorithm Get(AlgorithmKind kind, DeltaKind deltaKind)
		{
			return algorithms[(int)kind, (int)deltaKind];
		}
	}
}
