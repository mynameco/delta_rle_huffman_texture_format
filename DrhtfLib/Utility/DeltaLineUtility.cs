namespace DrhLib.Utility
{
	public static class DeltaLineUtility
	{
		public static void DeltaEncodeChannels(Color32[] pixels, int offset, int width, Color32[] prevLine, int channelCount, Color32[] lineH, Color32[] lineV, Color32[] lineHV)
		{
			var indexX = offset;

			Color32 prevPixel = default;
			Color32 prevLinePixel = default;
			for (int index = 0; index < width; index++, indexX++)
			{
				var currentPixel = pixels[indexX];
				var currentPrevLinePixel = prevLine[index];

				{
					var pixelH = currentPixel;
					pixelH -= prevPixel;

					pixelH[0] -= pixelH[1];
					pixelH[2] -= pixelH[1];

					lineH[index] = pixelH;
				}

				{
					var pixelV = currentPixel;
					pixelV -= currentPrevLinePixel;

					pixelV[0] -= pixelV[1];
					pixelV[2] -= pixelV[1];

					lineV[index] = pixelV;
				}

				{
					var pixelHV = currentPixel;
					pixelHV -= prevPixel;
					pixelHV -= currentPrevLinePixel - prevLinePixel;

					pixelHV[0] -= pixelHV[1];
					pixelHV[2] -= pixelHV[1];

					lineHV[index] = pixelHV;
				}

				prevPixel = currentPixel;
				prevLinePixel = currentPrevLinePixel;

				prevLine[index] = currentPixel;
			}
		}
	}
}
