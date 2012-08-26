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

		public class CheckFailedException : Exception
		{
			public CheckFailedException(string msg) : base(msg)
			{
			}
		}
	}
}

