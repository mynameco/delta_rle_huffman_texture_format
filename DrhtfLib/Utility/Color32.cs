using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DrhLib.Utility
{
	[StructLayout(LayoutKind.Explicit)]
	public unsafe struct Color32
	{
		[FieldOffset(0)]
		public byte r;

		[FieldOffset(1)]
		public byte g;

		[FieldOffset(2)]
		public byte b;

		[FieldOffset(3)]
		public byte a;

		[FieldOffset(0)]
		public fixed byte bytes[4];

		public byte this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return bytes[index];
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				bytes[index] = value;
			}
		}

		public static Color32 operator +(Color32 a, Color32 b)
		{
			a.r += b.r;
			a.g += b.g;
			a.b += b.b;
			a.a += b.a;
			return a;
		}

		public static Color32 operator -(Color32 a, Color32 b)
		{
			a.r -= b.r;
			a.g -= b.g;
			a.b -= b.b;
			a.a -= b.a;
			return a;
		}
	}
}
