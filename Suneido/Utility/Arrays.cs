using System;

namespace Suneido.Utility
{
	public static class Arrays
	{
		public static T[] CopyOf<T>(T[] src, int newLength)
		{
			T[] dst = new T[newLength];
			Array.Copy(src, dst, Math.Min(src.Length, newLength));
			return dst;
		}
	}
}

