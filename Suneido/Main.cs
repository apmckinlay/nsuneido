using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Suneido
{
	class MainClass
	{
		public volatile static int z;

		public static void Main(string[] args)
		{
			var sw = new Stopwatch();
			sw.Start();

			long n = 0;
			int loops = 1000;
			do {
				for (int i = 0; i < loops; ++i, ++n) {

//					dynamic x = 123;
//					dynamic y = 456;
//					dynamic z = x * y;

//					int x = 123;
//					int y = 456;
//					z = x * y;

//					var x = new Value(123);
//					var y = new Value(456);
//					var z = x * y;

//					decimal x = 123;
//					decimal y = 456;
//					decimal z = x * y;

				}
				loops *= 2;
			} while (sw.ElapsedMilliseconds < 2000);

			sw.Stop();

			long rate = n  / sw.ElapsedMilliseconds;
			Console.WriteLine(rate + " per ms (" + n + " in " + sw.Elapsed + ")");
		}
	}

	struct Value
	{
		object o;
		int i;

		public Value(object o)
		{
			this.o = o;
			this.i = 0;
		}
		public Value(int i)
		{
			this.i = i;
			this.o = null;
		}
		public bool isInt()
		{
			return o == null;
		}
		public bool isObject()
		{
			return o != null;
		}
		public int IntVal
			{ get { return i; } }
		public object ObVal
			{ get { return o; } }
		public static Value operator*(Value x, Value y)
		{
			if (x.isInt() && y.isInt())
				return new Value(x.i * y.i);
			else
				throw new NotImplementedException();
		}
	}
}
