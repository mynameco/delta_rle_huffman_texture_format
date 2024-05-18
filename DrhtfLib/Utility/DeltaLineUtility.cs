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

					pixelH[Color32.Red] -= pixelH[Color32.Green];
					pixelH[Color32.Blue] -= pixelH[Color32.Green];

					lineH[index] = pixelH;
				}

				{
					var pixelV = currentPixel;
					pixelV -= currentPrevLinePixel;

					pixelV[Color32.Red] -= pixelV[Color32.Green];
					pixelV[Color32.Blue] -= pixelV[Color32.Green];

					lineV[index] = pixelV;
				}

				{
					var pixelHV = currentPixel;
					pixelHV -= prevPixel;
					pixelHV -= currentPrevLinePixel - prevLinePixel;

					pixelHV[Color32.Red] -= pixelHV[Color32.Green];
					pixelHV[Color32.Blue] -= pixelHV[Color32.Green];

					lineHV[index] = pixelHV;
				}

				prevPixel = currentPixel;
				prevLinePixel = currentPrevLinePixel;

				prevLine[index] = currentPixel;
			}
		}
	}
}
