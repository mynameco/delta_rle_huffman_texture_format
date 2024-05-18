namespace DrhLib.Statistics
{
	public static class TextConvertUtility
	{
		public static string ToByteString(long value)
		{
			var result = value.ToString();
			for (var index = result.Length - 3; index > 0; index -= 3)
			{
				result = result.Insert(index, " ");
			}

			return result;
		}
	}
}
