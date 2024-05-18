namespace DrhLib.Utility
{
	public static class HashUtility
	{
		public const ulong StartHash = 5381;

		public static ulong ComputeHash(Color32[] value, int channelCount, ulong hash = StartHash)
		{
			for (var index = 0; index < value.Length; index++)
			{
				for (int index2 = 0; index2 < channelCount; index2++)
					hash = (hash << 5) + hash + value[index][index2];
			}

			return hash;
		}
	}
}
