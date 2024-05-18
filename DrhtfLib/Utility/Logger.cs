using System.Text;

namespace DrhLib.Utility
{
	public static class Logger
	{
		private static string fileName;
		private static StringBuilder builder = new StringBuilder();

		public static void WriteLine()
		{
			lock (builder)
				builder.AppendLine();

			Console.WriteLine();
		}

		public static void WriteLine(string text)
		{
			lock (builder)
				builder.AppendLine(text);

			Console.WriteLine(text);
		}

		public static void Prepare(string fileName)
		{
			lock (builder)
			{
				Logger.fileName = fileName;
				builder.Clear();
			}
		}

		public static void Save()
		{
			lock (builder)
			{
				File.WriteAllText(fileName, builder.ToString());
				builder.Clear();
			}
		}
	}
}
