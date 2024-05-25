using DrhtfLib.Commons;

namespace DrhtfLib.Encoders
{
	public static class EncoderUtility
	{
		public static void DeltaEncodeChannels(Color32[] pixels, int offset, int width, Color32[] prevLine, Color32[] lineH, Color32[] lineV, Color32[] lineHV)
		{
			var indexX = offset;

			Color32 prevPixel = default;
			Color32 prevLinePixel = default;
			for (int index = 0; index < width; index++, indexX++)
			{
				var currentPixel = pixels[indexX];
				var currentPrevLinePixel = prevLine[index];

				currentPixel[Color32.Red] -= currentPixel[Color32.Green];
				currentPixel[Color32.Blue] -= currentPixel[Color32.Green];

				lineH[index] = currentPixel - prevPixel;
				lineV[index] = currentPixel - currentPrevLinePixel;
				lineHV[index] = currentPixel - prevPixel - (currentPrevLinePixel - prevLinePixel);

				prevPixel = currentPixel;
				prevLinePixel = currentPrevLinePixel;

				prevLine[index] = currentPixel;
			}
		}
	}
}
