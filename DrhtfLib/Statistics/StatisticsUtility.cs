using DrhLib.Compress;
using DrhLib.Utility;
using DrhtfLib.Utility;
using System.Text;

namespace DrhLib.Statistics
{
	public static class StatisticsUtility
	{
		public static void WriteByteStatistics(string header, long length, long byteLength)
		{
			var builder = new StringBuilder();
			builder.AppendLine(header);

			builder.Append("Length : ");
			builder.Append(TextConvertUtility.ToByteString(length));
			builder.Append(" ( ");
			builder.Append(length * 100 / byteLength);
			builder.Append(" % )");
			builder.AppendLine();

			Logger.WriteLine(builder.ToString());
		}

		public static void WriteBitStatistics(string header, long bitLength)
		{
			Logger.WriteLine(header + " : " + TextConvertUtility.ToByteString(bitLength) + " , length : " + TextConvertUtility.ToByteString(bitLength >> 3) + " , part : " + TextConvertUtility.ToByteString(bitLength & 0b_111));
		}

		public static void WriteByteStatistics(string header, AlgorithmStatistics statistics, long length, long byteLength, int channelCount)
		{
			var builder = new StringBuilder();
			builder.AppendLine(header);

			builder.Append("Length : ");
			builder.Append(TextConvertUtility.ToByteString(length));
			builder.Append(" ( ");
			builder.Append(length * 100 / byteLength);
			builder.Append(" % )");
			builder.AppendLine();

			builder.AppendLine();
			for (int index = 0; index < (int)DeltaKind.Count; index++)
			{
				builder.Append("Delta count [");
				builder.Append((DeltaKind)index);
				builder.Append("] : ");
				builder.Append(statistics.Delta[index]);
				builder.AppendLine();
			}

			builder.AppendLine();
			for (int channel = 0; channel < channelCount; channel++)
			{
				builder.Append("Channel rle count [");
				builder.Append(channel);
				builder.Append("] : ");
				builder.Append(TextConvertUtility.ToByteString(statistics.Rle[channel]));
				builder.AppendLine();
			}

			builder.AppendLine();
			for (int channel = 0; channel < channelCount; channel++)
			{
				builder.Append("Channel length [");
				builder.Append(channel);
				builder.Append("] : ");
				builder.Append(TextConvertUtility.ToByteString(statistics.Length[channel]));
				builder.AppendLine();
			}

			for (int channel = 0; channel < channelCount; channel++)
			{
				builder.AppendLine();
				for (var index = 0; index < (int)AlgorithmKind.Count; index++)
				{
					var value = statistics.Algorithms[channel, index];
					builder.Append((AlgorithmKind)index);
					builder.Append(" [");
					builder.Append(channel);
					builder.Append("] : ");
					builder.Append(TextConvertUtility.ToByteString(value));
					builder.AppendLine();
				}
			}

			Logger.WriteLine(builder.ToString());
		}
	}
}
