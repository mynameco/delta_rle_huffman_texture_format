using DrhLib.Utility;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Drh.Utility
{
	public static class TextureUtility
	{
		public unsafe static bool LoadTexture(string fileName, out int width, out int height, out Color32[] pixels, out bool hasAlpha)
		{
			var bitmap = new Bitmap(fileName);

			Logger.WriteLine("Pixel format : " + bitmap.PixelFormat);

			hasAlpha = bitmap.PixelFormat == PixelFormat.Format32bppArgb;

			var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

			width = data.Width;
			height = data.Height;
			pixels = new Color32[data.Width * data.Height];

			var length = pixels.Length * 4;
			fixed (Color32* p = pixels)
			{
				Buffer.MemoryCopy((void*)data.Scan0, (void*)(IntPtr)p, length, length);
			}

			bitmap.UnlockBits(data);

			return true;
		}

		public unsafe static bool SaveTexture(string fileName, int width, int height, bool hasAlpha, Color32[] pixels)
		{
			var bitmap = new Bitmap(width, height, hasAlpha ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb);

			var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

			var length = pixels.Length * 4;
			fixed (Color32* p = pixels)
			{
				Buffer.MemoryCopy((void*)(IntPtr)p, (void*)data.Scan0, length, length);
			}

			bitmap.UnlockBits(data);

			bitmap.Save(fileName, ImageFormat.Png);
			return true;
		}
	}
}
