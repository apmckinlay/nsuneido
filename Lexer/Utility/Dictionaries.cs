using System;
using System.Collections.Generic;

namespace Suneido.Utility
{
	public static class Dictionaries
	{
		public static V GetOrElse<K,V>(this Dictionary<K,V> dict, K key, V def)
		{
			V val = default(V);
			return dict.TryGetValue(key, out val) ? val : def;
		}
	}
}

