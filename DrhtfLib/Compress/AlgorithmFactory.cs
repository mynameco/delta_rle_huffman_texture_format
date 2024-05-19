using DrhLib.Compress.Algorithms;
using DrhLib.Rles;
using DrhtfLib.Utility;

namespace DrhLib.Compress
{
	public class AlgorithmFactory
	{
		private ICompressAlgorithm[,] algorithms = new ICompressAlgorithm[(int)AlgorithmKind.Count, (int)DeltaKind.Count];
		private static IComputeRle stubRle = new StubRle();

		public AlgorithmFactory(IComputeRle rle, int channelCount)
		{
			algorithms[(int)AlgorithmKind.HA, (int)DeltaKind.H] = new Algorithm_HA(stubRle, channelCount);
			algorithms[(int)AlgorithmKind.SHA, (int)DeltaKind.H] = new Algorithm_SHA(stubRle, channelCount);
			algorithms[(int)AlgorithmKind.H, (int)DeltaKind.H] = new Algorithm_H(stubRle, channelCount);
			algorithms[(int)AlgorithmKind.SH, (int)DeltaKind.H] = new Algorithm_SH(stubRle, channelCount);
			algorithms[(int)AlgorithmKind.RHA, (int)DeltaKind.H] = new Algorithm_HA(rle, channelCount);
			algorithms[(int)AlgorithmKind.RSHA, (int)DeltaKind.H] = new Algorithm_SHA(rle, channelCount);
			algorithms[(int)AlgorithmKind.RH, (int)DeltaKind.H] = new Algorithm_H(rle, channelCount);
			algorithms[(int)AlgorithmKind.RSH, (int)DeltaKind.H] = new Algorithm_SH(rle, channelCount);

			algorithms[(int)AlgorithmKind.HA, (int)DeltaKind.V] = new Algorithm_HA(stubRle, channelCount);
			algorithms[(int)AlgorithmKind.SHA, (int)DeltaKind.V] = new Algorithm_SHA(stubRle, channelCount);
			algorithms[(int)AlgorithmKind.H, (int)DeltaKind.V] = new Algorithm_H(stubRle, channelCount);
			algorithms[(int)AlgorithmKind.SH, (int)DeltaKind.V] = new Algorithm_SH(stubRle, channelCount);
			algorithms[(int)AlgorithmKind.RHA, (int)DeltaKind.V] = new Algorithm_HA(rle, channelCount);
			algorithms[(int)AlgorithmKind.RSHA, (int)DeltaKind.V] = new Algorithm_SHA(rle, channelCount);
			algorithms[(int)AlgorithmKind.RH, (int)DeltaKind.V] = new Algorithm_H(rle, channelCount);
			algorithms[(int)AlgorithmKind.RSH, (int)DeltaKind.V] = new Algorithm_SH(rle, channelCount);

			algorithms[(int)AlgorithmKind.HA, (int)DeltaKind.HV] = new Algorithm_HA(stubRle, channelCount);
			algorithms[(int)AlgorithmKind.SHA, (int)DeltaKind.HV] = new Algorithm_SHA(stubRle, channelCount);
			algorithms[(int)AlgorithmKind.H, (int)DeltaKind.HV] = new Algorithm_H(stubRle, channelCount);
			algorithms[(int)AlgorithmKind.SH, (int)DeltaKind.HV] = new Algorithm_SH(stubRle, channelCount);
			algorithms[(int)AlgorithmKind.RHA, (int)DeltaKind.HV] = new Algorithm_HA(rle, channelCount);
			algorithms[(int)AlgorithmKind.RSHA, (int)DeltaKind.HV] = new Algorithm_SHA(rle, channelCount);
			algorithms[(int)AlgorithmKind.RH, (int)DeltaKind.HV] = new Algorithm_H(rle, channelCount);
			algorithms[(int)AlgorithmKind.RSH, (int)DeltaKind.HV] = new Algorithm_SH(rle, channelCount);
		}

		public ICompressAlgorithm Get(AlgorithmKind kind, DeltaKind deltaKind)
		{
			return algorithms[(int)kind, (int)deltaKind];
		}
	}
}
