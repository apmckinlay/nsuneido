using System;
using System.Text;

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

		public static string Repeat(this string s, int n)
		{
		   StringBuilder sb = new StringBuilder(n * s.Length);
		   for (int i = 0; i < n; i++)
		       sb.Append(s);
		   return sb.ToString();
		}
	}
}

