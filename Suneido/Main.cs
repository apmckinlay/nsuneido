using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace Suneido
{
	class MainClass
	{
		public static void Main(string[] args)
		{
		}

		public static void Benchmark()
		{
			var sw = new Stopwatch();
			sw.Start();

			long n = 0;
			int loops = 1000;
			do {
				for (int i = 0; i < loops; ++i, ++n) {

				}
				loops *= 2;
			} while (sw.ElapsedMilliseconds < 2000);

			sw.Stop();

			long rate = n  / sw.ElapsedMilliseconds;
			Console.WriteLine(rate + " per ms (" + n + " in " + sw.Elapsed + ")");
		}
	}

}
