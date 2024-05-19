using DrhLib.Streams;
using DrhLib.Utility;
using DrhtfLib.Utility;

namespace DrhLib.Compress
{
	public static class AlgorithmsUtility
	{
		public const int KindSize = 3;

		public static IBitStreamWriter WriteLineAsync(
			IByteStreamWriter writer,
			Color32[] line,
			DeltaKind deltaKind,
			int channel,
			int lineIndex,
			AlgorithmFactory factory,
			bool checkAlgorithms,
			ref int resultKind,
			ref int resultRle)
		{
			var infos = new IBitStreamWriter[(int)AlgorithmKind.Count];
			var rles = new int[(int)AlgorithmKind.Count];
			var tasks = new Task[(int)AlgorithmKind.Count];

			for (int kind = 0; kind < (int)AlgorithmKind.Count; kind++)
			{
				var tmpKind = kind;

				var task =
					new Task(
						() =>
						{
							var algorithm = factory.Get((AlgorithmKind)tmpKind, deltaKind);
							if (algorithm == null)
								return;

							var tmpWriter = writer.Create();
							tmpWriter.Write(tmpKind, KindSize);

							algorithm.Write(tmpWriter, line, channel, lineIndex, (AlgorithmKind)tmpKind, ref rles[tmpKind]);

							if (checkAlgorithms)
								CheckAlgorithm(tmpWriter, algorithm, channel, lineIndex, (AlgorithmKind)tmpKind, line);

							algorithm.UpdateTable(line, channel, lineIndex, (AlgorithmKind)tmpKind);
							infos[tmpKind] = tmpWriter;
						});

				task.Start();
				tasks[tmpKind] = task;
			}

			resultKind = 0;

			tasks[resultKind].Wait();
			var minWriter = infos[resultKind];
			infos[resultKind] = null;

			for (int kind = 1; kind < (int)AlgorithmKind.Count; kind++)
			{
				tasks[kind].Wait();

				ComputeMinWriter(kind, ref minWriter, infos, ref resultKind);
			}

			resultRle += rles[resultKind];

			return minWriter;
		}

		public static IBitStreamWriter WriteLine(
			IByteStreamWriter writer,
			Color32[] line,
			DeltaKind deltaKind,
			int channel,
			int lineIndex,
			AlgorithmFactory factory,
			bool checkAlgorithms,
			ref int resultKind,
			ref int resultRle)
		{
			var infos = new IBitStreamWriter[(int)AlgorithmKind.Count];
			var rles = new int[(int)AlgorithmKind.Count];

			for (int kind = 0; kind < (int)AlgorithmKind.Count; kind++)
			{
				var algorithm = factory.Get((AlgorithmKind)kind, deltaKind);
				if (algorithm == null)
					continue;

				var tmpWriter = writer.Create();
				tmpWriter.Write(kind, KindSize);

				algorithm.Write(tmpWriter, line, channel, lineIndex, (AlgorithmKind)kind, ref rles[kind]);

				if (checkAlgorithms)
					CheckAlgorithm(tmpWriter, algorithm, channel, lineIndex, (AlgorithmKind)kind, line);

				algorithm.UpdateTable(line, channel, lineIndex, (AlgorithmKind)kind);
				infos[kind] = tmpWriter;
			}

			resultKind = 0;

			var minWriter = infos[resultKind];
			infos[resultKind] = null;

			for (int kind = 1; kind < (int)AlgorithmKind.Count; kind++)
			{
				ComputeMinWriter(kind, ref minWriter, infos, ref resultKind);
			}

			resultRle += rles[resultKind];

			return minWriter;
		}

		private static void ComputeMinWriter(int kind, ref IBitStreamWriter minWriter, IBitStreamWriter[] infos, ref int kindResult)
		{
			var currentWriter = infos[kind];
			infos[kind] = null;

			if (currentWriter.BitLength < minWriter.BitLength)
			{
				kindResult = kind;
				minWriter.Dispose();
				minWriter = currentWriter;
			}
			else
			{
				currentWriter.Dispose();
			}
		}

		private static void CheckAlgorithm(IBitStreamWriter writer, ICompressAlgorithm algorithm, int channel, int lineIndex, AlgorithmKind kind, ReadOnlySpan<Color32> line)
		{
			var writer2 = writer as BitStreamWriter;
			if (writer2 == null)
				return;

			var writerBitLength = writer2.BitLength;
			var reader = writer2.CreateReader();
			var kind2 = (AlgorithmKind)reader.Read(KindSize);
			if (kind != kind2)
				throw new Exception("!!!");

			var pixels = new Color32[line.Length];
			algorithm.Read(reader, pixels, channel, lineIndex, kind);

			var readerBitIndex = reader.BitIndex;
			if (writerBitLength != readerBitIndex)
			{
				Logger.WriteLine("[Error] Wrong bit length" +
					" , writer bit length : " + writerBitLength +
					" , reader bit index : " + readerBitIndex +
					" , algorithm : " + kind +
					" , line : " + lineIndex +
					" , channel : " + channel);
			}

			for (int index = 0; index < line.Length; index++)
			{
				if (line[index][channel] != pixels[index][channel])
				{
					Logger.WriteLine("[Error] Wrong color" +
						" , color index : " + index +
						" , algorithm : " + kind +
						" , line : " + lineIndex +
						" , channel : " + channel);
					break;
				}
			}

			reader.Dispose();
		}
	}
}
