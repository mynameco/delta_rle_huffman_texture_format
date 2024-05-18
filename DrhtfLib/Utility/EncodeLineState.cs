using DdhtfLib.Utility;
using DrhLib.Compress;
using DrhLib.Rles;
using DrhLib.Statistics;
using DrhLib.Streams;
using System;
using System.Threading.Tasks;

namespace DrhLib.Utility
{
	public class EncodeLineState
	{
		private int width;
		private int channelCount;

		private AlgorithmFactory factory;

		private Color32[] prevLine;

		private Color32[] lineH;
		private Color32[] lineV;
		private Color32[] lineHV;

		private Task[,] tasksState;

		private IBitStreamWriter[] writersStateH;
		private IBitStreamWriter[] writersStateV;
		private IBitStreamWriter[] writersStateHV;

		private int[] resultKindsH;
		private int[] resultKindsV;
		private int[] resultKindsHV;

		private int[] resultRlesH;
		private int[] resultRlesV;
		private int[] resultRlesHV;

		public EncodeLineState(IComputeRle rle, int width, int channelCount)
		{
			this.width = width;
			this.channelCount = channelCount;

			factory = new AlgorithmFactory(rle, channelCount);

			prevLine = new Color32[width];

			lineH = new Color32[width];
			lineV = new Color32[width];
			lineHV = new Color32[width];

			tasksState = new Task[channelCount, 3];

			writersStateH = new IBitStreamWriter[channelCount];
			writersStateV = new IBitStreamWriter[channelCount];
			writersStateHV = new IBitStreamWriter[channelCount];

			resultKindsH = new int[channelCount];
			resultKindsV = new int[channelCount];
			resultKindsHV = new int[channelCount];

			resultRlesH = new int[channelCount];
			resultRlesV = new int[channelCount];
			resultRlesHV = new int[channelCount];
		}

		public void EncodeLine(IByteStreamWriter writer, Color32[] pixels, int offset, int lineIndex, bool useAsync, bool checkAlgorithms, AlgorithmStatistics byteStatistics)
		{
			DeltaLineUtility.DeltaEncodeChannels(pixels, offset, width, prevLine, channelCount, lineH, lineV, lineHV);

			if (useAsync)
				EncodeChannelsAsync(writer, byteStatistics, lineIndex, checkAlgorithms);
			else
				EncodeChannels(writer, byteStatistics, lineIndex, checkAlgorithms);
		}

		private void EncodeChannelsAsync(IByteStreamWriter writer, AlgorithmStatistics byteStatistics, int lineIndex, bool checkAlgorithms)
		{
			var lengthH = 0L;
			var lengthV = 0L;
			var lengthHV = 0L;

			for (int channel = 0; channel < channelCount; channel++)
			{
				var tmpChannel = channel;

				var task =
					new Task(
						() =>
						{
							writersStateH[tmpChannel] = AlgorithmsUtility.WriteLineAsync(
								writer,
								lineH,
								DeltaKind.H,
								tmpChannel,
								lineIndex,
								factory,
								checkAlgorithms,
								ref resultKindsH[tmpChannel],
								ref resultRlesH[tmpChannel]);
						});

				task.Start();
				tasksState[channel, (int)DeltaKind.H] = task;

				task =
					new Task(
						() =>
						{
							writersStateV[tmpChannel] = AlgorithmsUtility.WriteLineAsync(
								writer,
								lineV,
								DeltaKind.V,
								tmpChannel,
								lineIndex,
								factory,
								checkAlgorithms,
								ref resultKindsV[tmpChannel],
								ref resultRlesV[tmpChannel]);
						});

				task.Start();
				tasksState[channel, (int)DeltaKind.V] = task;

				task =
					new Task(
						() =>
						{
							writersStateHV[tmpChannel] = AlgorithmsUtility.WriteLineAsync(
								writer,
								lineHV,
								DeltaKind.HV,
								tmpChannel,
								lineIndex,
								factory,
								checkAlgorithms,
								ref resultKindsHV[tmpChannel],
								ref resultRlesHV[tmpChannel]);
						});

				task.Start();
				tasksState[channel, (int)DeltaKind.HV] = task;
			}

			for (int channel = 0; channel < channelCount; channel++)
			{
				tasksState[channel, (int)DeltaKind.H].Wait();
				tasksState[channel, (int)DeltaKind.H] = null;

				tasksState[channel, (int)DeltaKind.V].Wait();
				tasksState[channel, (int)DeltaKind.V] = null;

				tasksState[channel, (int)DeltaKind.HV].Wait();
				tasksState[channel, (int)DeltaKind.HV] = null;

				var entryH = writersStateH[channel];
				lengthH += entryH.BitLength;

				var entryV = writersStateV[channel];
				lengthV += entryV.BitLength;

				var entryHV = writersStateHV[channel];
				lengthHV += entryHV.BitLength;
			}

			ComputeResults(writer, byteStatistics, lengthH, lengthV, lengthHV);
		}

		private void EncodeChannels(IByteStreamWriter writer, AlgorithmStatistics byteStatistics, int lineIndex, bool checkAlgorithms)
		{
			var lengthH = 0L;
			var lengthV = 0L;
			var lengthHV = 0L;

			for (int channel = 0; channel < channelCount; channel++)
			{
				var entry = AlgorithmsUtility.WriteLine(
					writer,
					lineH,
					DeltaKind.H,
					channel,
					lineIndex,
					factory,
					checkAlgorithms,
					ref resultKindsH[channel],
					ref resultRlesH[channel]);
				lengthH += entry.BitLength;
				writersStateH[channel] = entry;
			}

			for (int channel = 0; channel < channelCount; channel++)
			{
				var entry = AlgorithmsUtility.WriteLine(
					writer,
					lineV,
					DeltaKind.V,
					channel,
					lineIndex,
					factory,
					checkAlgorithms,
					ref resultKindsV[channel],
					ref resultRlesV[channel]);
				lengthV += entry.BitLength;
				writersStateV[channel] = entry;
			}

			for (int channel = 0; channel < channelCount; channel++)
			{
				var entry = AlgorithmsUtility.WriteLine(
					writer,
					lineHV,
					DeltaKind.HV,
					channel,
					lineIndex,
					factory,
					checkAlgorithms,
					ref resultKindsHV[channel],
					ref resultRlesHV[channel]);
				lengthHV += entry.BitLength;
				writersStateHV[channel] = entry;
			}

			ComputeResults(writer, byteStatistics, lengthH, lengthV, lengthHV);
		}

		private void ComputeResults(IByteStreamWriter writer, AlgorithmStatistics byteStatistics, long lengthH, long lengthV, long lengthHV)
		{
			var deltaKind = DeltaKind.HV;
			if (lengthH < lengthV &&
				lengthH < lengthHV)
			{
				deltaKind = DeltaKind.H;
			}
			else if (lengthV < lengthH &&
				lengthV < lengthHV)
			{
				deltaKind = DeltaKind.V;
			}

			if (deltaKind == DeltaKind.H)
			{
				((IBitStreamWriter)writer).Write(0b_10, 2);

				for (int channel = 0; channel < channelCount; channel++)
				{
					writer.WriteAndDispose(writersStateH[channel]);
					writersStateV[channel].Dispose();
					writersStateHV[channel].Dispose();

					writersStateH[channel] = null;
					writersStateV[channel] = null;
					writersStateHV[channel] = null;

					lock (byteStatistics)
					{
						byteStatistics.Tables[resultKindsH[channel]]++;
						byteStatistics.Rle += resultRlesH[channel];
					}
				}

				lock (byteStatistics)
					byteStatistics.DeltaH++;
			}
			else if (deltaKind == DeltaKind.V)
			{
				((IBitStreamWriter)writer).Write(0b_11, 2);

				for (int channel = 0; channel < channelCount; channel++)
				{
					writersStateH[channel].Dispose();
					writer.WriteAndDispose(writersStateV[channel]);
					writersStateHV[channel].Dispose();

					writersStateH[channel] = null;
					writersStateV[channel] = null;
					writersStateHV[channel] = null;

					lock (byteStatistics)
					{
						byteStatistics.Tables[resultKindsV[channel]]++;
						byteStatistics.Rle += resultRlesV[channel];
					}
				}

				lock (byteStatistics)
					byteStatistics.DeltaV++;
			}
			else
			{
				((IBitStreamWriter)writer).Write(0, 1);

				for (int channel = 0; channel < channelCount; channel++)
				{
					writersStateH[channel].Dispose();
					writersStateV[channel].Dispose();
					writer.WriteAndDispose(writersStateHV[channel]);

					writersStateH[channel] = null;
					writersStateV[channel] = null;
					writersStateHV[channel] = null;

					lock (byteStatistics)
					{
						byteStatistics.Tables[resultKindsHV[channel]]++;
						byteStatistics.Rle += resultRlesHV[channel];
					}
				}

				lock (byteStatistics)
					byteStatistics.DeltaHV++;
			}

			Array.Clear(resultKindsH, 0, resultKindsH.Length);
			Array.Clear(resultKindsV, 0, resultKindsV.Length);
			Array.Clear(resultKindsHV, 0, resultKindsHV.Length);

			Array.Clear(resultRlesH, 0, resultRlesH.Length);
			Array.Clear(resultRlesV, 0, resultRlesV.Length);
			Array.Clear(resultRlesHV, 0, resultRlesHV.Length);
		}
	}
}
