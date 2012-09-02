using System;

namespace Suneido.Database
{
	internal class HeapStorage : Storage
	{

		internal HeapStorage() : base(32, 32)
		{
		}

		internal HeapStorage(int chunkSize, int initChunks) : base(Align(chunkSize), initChunks)
		{
		}

		protected override ByteBuffer Get(int chunk)
		{
			return new ArrayBuffer(CHUNK_SIZE);
		}

	}
}

namespace Suneido.Database
{
	using NUnit.Framework;

	[TestFixture]
	public class HeapStorageTest
	{
		[Test]
		public void Basic()
		{
			var stor = new HeapStorage();
			int adr1 = stor.Alloc(20);
			Assert.That(adr1, Is.EqualTo(1));
			ByteBuffer buf = stor.Buffer(adr1);
			buf[0] = 12;
			int adr2 = stor.Alloc(20);
			Assert.That(adr2, Is.EqualTo(5));
			buf = stor.Buffer(adr2);
			buf[0] = 34;
			buf = stor.Buffer(adr1);
			Assert.That(buf[0], Is.EqualTo(12));
			buf = stor.Buffer(adr2);
			Assert.That(buf[0], Is.EqualTo(34));
		}
	}
}