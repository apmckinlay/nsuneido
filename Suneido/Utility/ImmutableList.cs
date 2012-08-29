using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Suneido.Utility
{
	/// <summary>
	/// An array backed immutable list.
	/// </summary>
	public class ImmutableList<T> : IList<T>
		// with .Net 4.5 could derive from IReadOnlyList<T>
	{
		readonly T[] data;
		static ImmutableList<T> EMPTY = new ImmutableList<T>(new T[0]);

		// private - create instances with static "of" methods or with builder
		internal ImmutableList(T[] data)
		{
			this.data = data;
		}

		/// <returns>An empty list.</returns>
		public static ImmutableList<T> Of()
		{
			return EMPTY;
		}

		public T this[int i]
			{ 
			get { return data[i];	}
			set { throw new NotSupportedException("ImmutableList[i] ="); }
			}

		public int Count
			{ get { return data.Length; } }

		public int IndexOf(T x)
		{
			return Array.IndexOf(data, x);
		}

		public bool Contains(T x)
		{
			return Array.IndexOf(data, x) >= 0;
		}

		public void CopyTo(T[] a, int i)
		{
			data.CopyTo(a, i);
		}

		public bool IsReadOnly
			{ get { return true; } }

		public void Add(T value)
		{
			throw new NotSupportedException("ImmutableList.Add");
		}

		public void Insert(int i, T value)
		{
			throw new NotSupportedException("ImmutableList.Insert");
		}

		public bool Remove(T value)
		{
			throw new NotSupportedException("ImmutableList.Remove");
		}

		public void RemoveAt(int i)
		{
			throw new NotSupportedException("ImmutableList.RemoveAt");
		}

		public void Clear()
		{
			throw new NotSupportedException("ImmutableList.Clear");
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (var x in data)
				yield return x;
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator) data.GetEnumerator();
		}
	}
	public static class ImmutableList
	{
		/// <returns>An immutable list containing the specified values.</returns>
		public static ImmutableList<T> Of<T>(params T[] data)
		{
			return new ImmutableList<T>(data);
		}

	}
}

namespace Suneido.Utility
{
	using NUnit.Framework;

	[TestFixture]
	public class ImmutableListTest
	{
		[Test]
		public void of()
		{
			var x = ImmutableList<int>.Of();
			Assert.That(x.Count, Is.EqualTo(0));

			x = ImmutableList.Of(1, 2, 3);
			Assert.That(x.Count, Is.EqualTo(3));
			Assert.That(x[0], Is.EqualTo(1));
			Assert.That(x[1], Is.EqualTo(2));
			Assert.That(x[2], Is.EqualTo(3));
			Assert.That(x.IndexOf(2), Is.EqualTo(1));
			Assert.That(x.IndexOf(99), Is.EqualTo(-1));
			Assert.That(x.Contains(3));
			Assert.That(x.Contains(99), Is.EqualTo(false));
			var y = new List<int> { 1, 2, 3 };
			Assert.That(Enumerable.SequenceEqual(x, y));
		}
	}
}
