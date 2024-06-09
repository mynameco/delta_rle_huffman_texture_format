using Drh.Utility;
using DrhLib.Rles;
using DrhLib.Statistics;
using DrhLib.Streams;
using DrhLib.Utility;
using DrhtfLib.Commons;

namespace Drh
{
	public class Program
	{
		private const string pngExtension = ".png";
		private const string drhExtension = ".drh";

		private static string fileName = "../../../../Data/Lenna.png";

		private static bool useRle = true;
		private static bool checkAlgorithms = false;
		private static bool useAsyncWriter = true;
		private static bool useAsyncReader = true;
		private static bool computeHash = false;

		private static bool writeDrhFile = true;
		private static bool writeDrhMemory = true;
		private static bool writeDrhPngFile = true;

		static void Main(string[] args)
		{
			if (args.Length > 0)
			{
				fileName = args[0];
				if (fileName.StartsWith("\"") &&
					fileName.EndsWith("\""))
				{
					fileName = fileName.Substring(1, fileName.Length - 2);
					Console.WriteLine("Argument : " + fileName);
				}

				if (!File.Exists(fileName))
				{
					Console.WriteLine("[Error] File not found : " + fileName);
					return;
				}
			}

			Compress(fileName);
			Decompress(fileName);
		}

		private static void Compress(string fileName)
		{
			Logger.Prepare(fileName + ".compress.txt");

			var width = 0;
			var height = 0;
			Color32[] pixels = null;
			var hasAlpha = false;

			Logger.WriteLine("----------------------------------------------------------");
			Logger.WriteLine("Compress");
			Logger.WriteLine(fileName);

			if (File.Exists(fileName))
			{
				if (!TextureUtility.LoadTexture(fileName, out width, out height, out pixels, out hasAlpha))
				{
					Logger.WriteLine("[Error] Error load file : " + fileName);
					Logger.Save();
					return;
				}
			}
			else
			{
				Logger.WriteLine("[Error] File not found : " + fileName);
				Logger.Save();
				return;
			}

			var startTime = DateTime.UtcNow;

			var channelCount = hasAlpha ? 4 : 3;
			var byteLength = pixels.LongLength * channelCount;
			Logger.WriteLine(
				"Width : " + width +
				" , Height : " + height +
				" , Has alpha : " + hasAlpha);

			Logger.WriteLine("In memory : " + TextConvertUtility.ToByteString(byteLength));

			if (fileName.EndsWith(pngExtension, StringComparison.OrdinalIgnoreCase))
			{
				var fi = new FileInfo(fileName);
				StatisticsUtility.WriteByteStatistics("Original png file length :", (int)fi.Length, byteLength);
			}

			if (computeHash)
				Logger.WriteLine("Pixels hash : " + HashUtility.ComputeHash(pixels, channelCount));

			Logger.WriteLine("----------------------------------------------------------");

			IByteStreamWriter writer = null;
			if (writeDrhMemory)
			{
				writer = new ByteStreamWriter(width, byteLength);
			}
			else
			{
				writer = new ByteStatisticWriter();
			}

			ComputeRle rle = null;
			if (useRle)
				rle = new ComputeRle();

			var byteStatistics = new AlgorithmStatistics();

			var version = 1;
			WriteFileUtility.WriteHeader(
				writer,
				version,
				width,
				height,
				hasAlpha);

			WriteFileUtility.EncodePixels(
				writer,
				rle,
				pixels,
				width,
				height,
				channelCount,
				useAsyncWriter,
				checkAlgorithms,
				byteStatistics);

			var bitLength = writer.BitLength;

			writer.Flush();

			StatisticsUtility.WriteBitStatistics("Bit length", bitLength);
			StatisticsUtility.WriteByteStatistics("Byte total", byteStatistics, writer.Length, byteLength, channelCount);

			var fileNameDrh = fileName + drhExtension;
			if (File.Exists(fileNameDrh))
				File.Delete(fileNameDrh);

			if (writeDrhFile)
			{
				if (writer is ByteStreamWriter writer2)
				{
					using (var stream = new FileStream(fileNameDrh, FileMode.Create, FileAccess.Write))
					{
						writer2.Stream.WriteTo(stream);
					}
				}
			}

			writer.Dispose();

			var endTime = DateTime.UtcNow;
			var deltaTime = endTime - startTime;
			Logger.WriteLine("Time seconds : " + (long)deltaTime.TotalSeconds);
			Logger.WriteLine();
			Logger.Save();
		}

		private static void Decompress(string fileName)
		{
			Logger.Prepare(fileName + ".decompress.txt");

			var startTime = DateTime.UtcNow;

			Logger.WriteLine("----------------------------------------------------------");
			Logger.WriteLine("Decompress");
			Logger.WriteLine(fileName);

			var bytes = File.ReadAllBytes(fileName + drhExtension);
			var reader = new BitStreamReader(bytes, bytes.LongLength << 3);

			ComputeRle rle = null;
			if (useRle)
				rle = new ComputeRle();

			if (!ReadFileUtility.ReadHeader(
				reader,
				out var version,
				out var width,
				out var height,
				out var hasAlpha))
			{
				Logger.WriteLine("[Error] Error read header");
				Logger.Save();
				return;
			}

			Logger.WriteLine(
				"Version : " + version +
				" , Width : " + width +
				" , Height : " + height +
				" , Has alpha : " + hasAlpha);
			Logger.WriteLine("----------------------------------------------------------");

			var channelCount = hasAlpha ? 4 : 3;
			var pixels = new Color32[width * height];
			var byteLength = pixels.LongLength * channelCount;

			if (!ReadFileUtility.DecodePixels(
				reader,
				rle,
				pixels,
				width,
				height,
				channelCount,
				useAsyncReader))
			{
				Logger.WriteLine("[Error] Error read body");
				Logger.Save();
				return;
			}

			var realBitIndex = reader.BitIndex;
			StatisticsUtility.WriteBitStatistics("Real bit length", realBitIndex);
			var bitIndex = Number7Utility.Read(reader);
			StatisticsUtility.WriteBitStatistics("Bit length", bitIndex);

			if (realBitIndex != bitIndex)
				Logger.WriteLine("[Error] Error bit lengths");

			reader.Dispose();

			if (computeHash)
				Logger.WriteLine("Pixels hash : " + HashUtility.ComputeHash(pixels, channelCount));

			if (writeDrhPngFile)
			{
				var fileNameDrhPng = fileName + drhExtension + pngExtension;
				if (!TextureUtility.SaveTexture(fileNameDrhPng, width, height, hasAlpha, pixels))
				{
					Logger.WriteLine("[Error] Error save file : " + fileNameDrhPng);
					Logger.Save();
					return;
				}

				var fi = new FileInfo(fileNameDrhPng);
				StatisticsUtility.WriteByteStatistics("Test png file :", (int)fi.Length, byteLength);
			}

			var endTime = DateTime.UtcNow;
			var deltaTime = endTime - startTime;
			Logger.WriteLine("Time seconds : " + (long)deltaTime.TotalSeconds);
			Logger.WriteLine();
			Logger.Save();
		}
	}
}
