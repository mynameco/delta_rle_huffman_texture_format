using DrhLib.Compress.Algorithms;
using DrhLib.Rles;
using DrhtfLib.Utility;

namespace DrhLib.Compress
{
	public class AlgorithmFactory
	{
		private ICompressAlgorithm[] algorithmsH = new ICompressAlgorithm[(int)AlgorithmKind.Count];
		private ICompressAlgorithm[] algorithmsV = new ICompressAlgorithm[(int)AlgorithmKind.Count];
		private ICompressAlgorithm[] algorithmsHV = new ICompressAlgorithm[(int)AlgorithmKind.Count];

		public AlgorithmFactory(IComputeRle rle, int channelCount)
		{
			algorithmsH[(int)AlgorithmKind.HA] = new Algorithm_HA(rle, channelCount);
			algorithmsH[(int)AlgorithmKind.SHA] = new Algorithm_SHA(rle, channelCount);
			algorithmsH[(int)AlgorithmKind.H] = new Algorithm_H(rle, channelCount);
			algorithmsH[(int)AlgorithmKind.SH] = new Algorithm_SH(rle, channelCount);

			algorithmsV[(int)AlgorithmKind.HA] = new Algorithm_HA(rle, channelCount);
			algorithmsV[(int)AlgorithmKind.SHA] = new Algorithm_SHA(rle, channelCount);
			algorithmsV[(int)AlgorithmKind.H] = new Algorithm_H(rle, channelCount);
			algorithmsV[(int)AlgorithmKind.SH] = new Algorithm_SH(rle, channelCount);

			algorithmsHV[(int)AlgorithmKind.HA] = new Algorithm_HA(rle, channelCount);
			algorithmsHV[(int)AlgorithmKind.SHA] = new Algorithm_SHA(rle, channelCount);
			algorithmsHV[(int)AlgorithmKind.H] = new Algorithm_H(rle, channelCount);
			algorithmsHV[(int)AlgorithmKind.SH] = new Algorithm_SH(rle, channelCount);
		}

		// TODO
		public ICompressAlgorithm Get(AlgorithmKind kind, DeltaKind deltaKind)
		{
			if (deltaKind == DeltaKind.H)
				return algorithmsH[(int)kind];

			if (deltaKind == DeltaKind.V)
				return algorithmsV[(int)kind];

			return algorithmsHV[(int)kind];
		}
	}
}
