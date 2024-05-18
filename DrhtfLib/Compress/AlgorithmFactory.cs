using DrhLib.Compress.Algorithms;
using DrhLib.Rles;
using DrhtfLib.Utility;

namespace DrhLib.Compress
{
	public class AlgorithmFactory
	{
		private ICompressAlgorithm[,] algorithms = new ICompressAlgorithm[(int)AlgorithmKind.Count, (int)DeltaKind.Count];

		public AlgorithmFactory(IComputeRle rle, int channelCount)
		{
			algorithms[(int)AlgorithmKind.HA, (int)DeltaKind.H] = new Algorithm_HA(rle, channelCount);
			algorithms[(int)AlgorithmKind.SHA, (int)DeltaKind.H] = new Algorithm_SHA(rle, channelCount);
			algorithms[(int)AlgorithmKind.H, (int)DeltaKind.H] = new Algorithm_H(rle, channelCount);
			algorithms[(int)AlgorithmKind.SH, (int)DeltaKind.H] = new Algorithm_SH(rle, channelCount);

			algorithms[(int)AlgorithmKind.HA, (int)DeltaKind.V] = new Algorithm_HA(rle, channelCount);
			algorithms[(int)AlgorithmKind.SHA, (int)DeltaKind.V] = new Algorithm_SHA(rle, channelCount);
			algorithms[(int)AlgorithmKind.H, (int)DeltaKind.V] = new Algorithm_H(rle, channelCount);
			algorithms[(int)AlgorithmKind.SH, (int)DeltaKind.V] = new Algorithm_SH(rle, channelCount);

			algorithms[(int)AlgorithmKind.HA, (int)DeltaKind.HV] = new Algorithm_HA(rle, channelCount);
			algorithms[(int)AlgorithmKind.SHA, (int)DeltaKind.HV] = new Algorithm_SHA(rle, channelCount);
			algorithms[(int)AlgorithmKind.H, (int)DeltaKind.HV] = new Algorithm_H(rle, channelCount);
			algorithms[(int)AlgorithmKind.SH, (int)DeltaKind.HV] = new Algorithm_SH(rle, channelCount);
		}

		public ICompressAlgorithm Get(AlgorithmKind kind, DeltaKind deltaKind)
		{
			return algorithms[(int)kind, (int)deltaKind];
		}
	}
}
