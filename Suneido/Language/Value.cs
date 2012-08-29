using System;
using System.Runtime.InteropServices;

namespace Suneido
{
	/// <summary>
	/// A value struct to avoid boxing and unboxing for integer calculations.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	struct Value
	{
		// how can this be "safe"???
		[FieldOffset(0)] object o;
		[FieldOffset(0)] int i;

		public Value(object o)
		{
			this.i = 0;
			this.o = o;
			Check.That(i != 0);
			if ((o as int?) != null)
				i = ((int) o << 2) | 3; // TODO check range
			else
				Check.That((i & 3) == 0);
		}
		public Value(int i)
		{
			this.o = null;
			this.i = (i << 2) | 3; // TODO check range
		}
		public bool IsInt()
		{
			return (i & 3) == 3;
		}
		public bool IsObject()
		{
			return (i & 3) != 3;
		}
		private int intVal
			{ get { return i >> 2; } }
		private object obVal
			{ get { return o; } }
		public int AsInt
			{ get { return IsInt() ? intVal : (int) o; }}
		public object AsObject
			{ get { return IsObject() ? o : intVal; } }
		public static Value operator*(Value x, Value y)
		{
			if (x.IsInt() && y.IsInt())
				return new Value(x.intVal * y.intVal);
			else
				return new Value((dynamic) x.o * (dynamic) x.o);
		}
		// TODO other operators
		// TODO equals
		// TODO compare
		public override string ToString()
		{
			return IsInt() ? intVal.ToString() : o.ToString();
		}
	}
}

namespace Suneido.Language
{
	using NUnit.Framework;

	[TestFixture]
	public class ValueTest
	{
		[Test]
		public void basic()
		{
			Value x = new Value(3);
			Assert.That(x.IsInt());
			Value y = new Value(4);
			Assert.That(y.IsInt());
			Value z = x * y;
			Assert.That(z.IsInt());
			Assert.That(z.ToString(), Is.EqualTo("12"));

			object o = 8;
			x = new Value(o);
			Assert.That(x.IsInt());
			z = x * y;
			Assert.That(z.IsInt());
			Assert.That(z.ToString(), Is.EqualTo("32"));

			x = new Value("hello");
			Assert.That(x.ToString(), Is.EqualTo("hello"));

			Assert.That((new Value(1.5) * new Value(1.5)).ToString(), Is.EqualTo("2.25"));
		}
	}
}
