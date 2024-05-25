using DrhLib.Compress;
using DrhLib.Rles;
using DrhLib.Streams;
using DrhLib.Utility;
using DrhtfLib.Commons;
using DrhtfLib.Utility;

namespace DrhtfLib.Encoders
{
	public class DecodeLineState
	{
		private int width;
		private int channelCount;

		private AlgorithmFactory factory;

		private Color32[] prevLine;

		private Color32[] lineH;
		private Color32[] lineV;
		private Color32[] lineHV;

		private int[] kinds;

		private Task[] tasks;

		public DecodeLineState(IComputeRle rle, int width, int channelCount)
		{
			this.width = width;
			this.channelCount = channelCount;

			factory = new AlgorithmFactory(rle, channelCount);

			prevLine = new Color32[width];

			lineH = new Color32[width];
			lineV = new Color32[width];
			lineHV = new Color32[width];

			kinds = new int[channelCount];

			tasks = new Task[(int)AlgorithmKind.Count * channelCount];
		}

		public bool DecodeLine(IBitStreamReader reader, Color32[] pixels, int offset, int lineIndex, bool useAsync)
		{
			var deltaKind = DeltaKind.HV;
			var value = reader.Read(1);
			if (value == 1)
			{
				value = reader.Read(1);
				if (value == 0)
					deltaKind = DeltaKind.H;
				else if (value == 1)
					deltaKind = DeltaKind.V;
			}

			if (!DecodeChannels(reader, pixels, deltaKind, offset, lineIndex))
				return false;

			// Декодируем, 
			DeltaDecodeChannels(pixels, deltaKind, offset);

			// А потом снова кодируем для статистики
			DeltaLineUtility.DeltaEncodeChannels(pixels, offset, width, prevLine, channelCount, lineH, lineV, lineHV);

			if (useAsync)
			{
				UpdateTableAsync(lineIndex, DeltaKind.H);
				UpdateTableAsync(lineIndex, DeltaKind.V);
				UpdateTableAsync(lineIndex, DeltaKind.HV);
			}
			else
			{
				UpdateTable(lineIndex, DeltaKind.H);
				UpdateTable(lineIndex, DeltaKind.V);
				UpdateTable(lineIndex, DeltaKind.HV);
			}

			return true;
		}

		private bool DecodeChannels(IBitStreamReader reader, Color32[] pixels, DeltaKind deltaKind, int offset, int lineIndex)
		{
			var values = pixels.AsSpan(offset, width);

			for (int channel = 0; channel < channelCount; channel++)
			{
				var kind = reader.Read(AlgorithmsUtility.KindSize);
				kinds[channel] = (int)kind;

				var algorithm = factory.Get((AlgorithmKind)kind, deltaKind);
				if (algorithm == null)
				{
					Logger.WriteLine("[Error] Algorithm not found" +
						" , kind : " + kind +
						" , pixel offset : " + offset +
						" , line : " + lineIndex +
						" , channel : " + channel);
					return false;
				}

				algorithm.Read(reader, values, channel, lineIndex, (AlgorithmKind)kind);
			}

			return true;
		}

		private void UpdateTableAsync(int lineIndex, DeltaKind deltaKind)
		{
			for (int channel = 0; channel < channelCount; channel++)
			{
				for (int kind = 0; kind < (int)AlgorithmKind.Count; kind++)
				{
					var tmpKind = kind;
					var tmpChannel = channel;
					var task =
						new Task(
							() =>
							{
								var algorithm = factory.Get((AlgorithmKind)tmpKind, deltaKind);

								Color32[] currentLine;
								if (deltaKind == DeltaKind.H)
									currentLine = lineH;
								else if (deltaKind == DeltaKind.V)
									currentLine = lineV;
								else
									currentLine = lineHV;

								algorithm.UpdateTable(currentLine, tmpChannel, lineIndex, (AlgorithmKind)tmpKind);
							});

					task.Start();
					tasks[(int)AlgorithmKind.Count * tmpChannel + tmpKind] = task;
				}
			}

			Task.WaitAll(tasks);
			Array.Clear(tasks, 0, tasks.Length);
		}

		private void UpdateTable(int lineIndex, DeltaKind deltaKind)
		{
			for (int channel = 0; channel < channelCount; channel++)
			{
				for (int kind = 0; kind < (int)AlgorithmKind.Count; kind++)
				{
					var algorithm = factory.Get((AlgorithmKind)kind, deltaKind);

					Color32[] currentLine;
					if (deltaKind == DeltaKind.H)
						currentLine = lineH;
					else if (deltaKind == DeltaKind.V)
						currentLine = lineV;
					else
						currentLine = lineHV;

					algorithm.UpdateTable(currentLine, channel, lineIndex, (AlgorithmKind)kind);
				}
			}
		}

		private void DeltaDecodeChannels(Color32[] pixels, DeltaKind deltaKind, int offset)
		{
			var indexX = offset;
			var indexPrevLineX = offset - width;

			Color32 prevPixel = default;
			Color32 prevLinePixel = default;
			for (int index = 0; index < width; index++, indexX++, indexPrevLineX++)
			{
				Color32 currentPrevLinePixel = default;
				if (indexPrevLineX >= 0)
					currentPrevLinePixel = pixels[indexPrevLineX];

				var currentPixel = pixels[indexX];
				currentPixel[Color32.Red] += currentPixel[Color32.Green];
				currentPixel[Color32.Blue] += currentPixel[Color32.Green];

				if (deltaKind == DeltaKind.H)
				{
					currentPixel += prevPixel;
				}
				else if (deltaKind == DeltaKind.V)
				{
					currentPixel += currentPrevLinePixel;
				}
				else
				{
					currentPixel += currentPrevLinePixel - prevLinePixel;
					currentPixel += prevPixel;
				}

				prevPixel = currentPixel;
				prevLinePixel = currentPrevLinePixel;

				pixels[indexX] = currentPixel;
			}
		}
	}
}
