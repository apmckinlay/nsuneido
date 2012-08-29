using System;
using System.Linq;
using System.Collections.Generic;

namespace Suneido.Utility
{
	/// <summary>
	/// A list stored in chunks in a List<T> of List<T>.
	/// Intended for very large lists to avoid making single huge arrays
	/// since they are hard on allocation and garbage collection.
	/// </summary>
	/// Does not currently implement standard interfaces like IList
	public class ArraysList<T>
	{
		const int CHUNK_SIZE = 1024;
		readonly List<List<T>> data = new List<List<T>> { new List<T>(CHUNK_SIZE) };

		public void Add(T value)
		{
			var last = data.Last();
			if (last.Count >= CHUNK_SIZE)
				data.Add(last = new List<T>(CHUNK_SIZE));
			last.Add(value);
		}

		public int Count
		{
			get { return (data.Count - 1) * CHUNK_SIZE + data.Last().Count; }
		}

		public void Clear()
		{
			data.Clear();
			data.Add(new List<T>(CHUNK_SIZE));
		}

		public T this[int i]
		{
			get { return data[chunk(i)][offset(i)]; }
			set { data[chunk(i)][offset(i)] = value; }
		}

		int chunk(int i)
		{
			return i / CHUNK_SIZE;
		}

		int offset(int i)
		{
			return i % CHUNK_SIZE;
		}

	}
}

namespace Suneido.Utility
{
	using NUnit.Framework;

	[TestFixture]
	public class ArraysListTest
	{
		[Test]
		public void Basic()
		{
			var list = new ArraysList<int>();
			Assert.That(list.Count, Is.EqualTo(0));
			list.Add(123);
			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(list[0], Is.EqualTo(123));
			list.Clear();
			Assert.That(list.Count, Is.EqualTo(0));
			const int N = 1500;
			for (int i = 0; i < N; ++i)
				list.Add(i);
			Assert.That(list.Count, Is.EqualTo(N));
			for (int i = 0; i < N; ++i)
				Assert.That(list[i], Is.EqualTo(i));
			list[123] = 456;
			list[1234] = 789;
			Assert.That(list[123], Is.EqualTo(456));
			Assert.That(list[1234], Is.EqualTo(789));
		}

	}

}
