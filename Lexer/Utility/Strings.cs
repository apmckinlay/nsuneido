using System;

namespace Suneido.Utility
{
	public static class Strings
	{
		public static bool IsLower(this string s)
		{
			foreach (var c in s)
				if (! Char.IsLower(c))
					return false;
			return true;
		}
	}
}

