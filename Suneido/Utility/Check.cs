using System;

namespace Suneido
{
	public static class Check
	{
		public static void That(bool cond)
		{
			if (! cond)
				throw new CheckFailedException("Check failed");
		}

		public static void That(bool cond, string msg)
		{
			if (! cond)
				throw new CheckFailedException("Check failed: " + msg);
		}

		public static void Index(int i, int n)
		{
			if (i < 0 || n <= i)
				throw new CheckFailedException("Check failed: index " + i + " not in size " + n);
		}

		public static void Slice(int i, int n, int len)
		{
			if (i < 0 || len <= i)
				throw new CheckFailedException("Check failed: index " + i + " not in size " + len);
			if (i + n > len)
				throw new CheckFailedException("Check failed: length " + i + " + " + n + " not in size " + len);
		}

		public static void Slice(long i, long n, long len)
		{
			if (i < 0 || n <= i)
				throw new CheckFailedException("Check failed: index " + i + " not in size " + len);
			if (i + n > len)
				throw new CheckFailedException("Check failed: length " + n + " not in size " + len);
		}

		public class CheckFailedException : Exception
		{
			public CheckFailedException(string msg) : base(msg)
			{
			}
		}
	}
}

