using System;
using Suneido.Utility;

namespace Suneido.Database
{
	[NotThreadSafe]
	public abstract class ByteBuffer
	{
		#region abstract
		public abstract byte this[int i]
		{ get; set; }

		public abstract int Length
		{ get; }

		public abstract ByteBuffer Slice(int pos, int len);
		#endregion

		#region accessors
		public short GetShort(int i)
		{
			return (short) (this[i] + (this[i + 1] << 8));
		}

		public void PutShort(int i, short x)
		{
			this[i] = (byte) x;
			this[i + 1] = (byte) (x >> 8);
		}

		public int GetInt(int i)
		{
			return this[i] + 
				(this[i + 1] << 8) + 
				(this[i + 2] << 16) +
				(this[i + 3] << 24);
		}

		public void PutInt(int i, int x)
		{
			this[i] = (byte) x;
			this[i + 1] = (byte) (x >> 8);
			this[i + 2] = (byte) (x >> 16);
			this[i + 3] = (byte) (x >> 24);
		}

		public long GetLong(int i)
		{
			return this[i] + 
				((long) this[i + 1] << 8) + 
				((long) this[i + 2] << 16) + 
				((long) this[i + 3] << 24) + 
				((long) this[i + 4] << 32) + 
				((long) this[i + 5] << 40) + 
				((long) this[i + 6] << 48) + 
				((long) this[i + 7] << 56);
		}

		public void PutLong(int i, long x)
		{
			this[i] = (byte) x;
			this[i + 1] = (byte) (x >> 8);
			this[i + 2] = (byte) (x >> 16);
			this[i + 3] = (byte) (x >> 24);
			this[i + 4] = (byte) (x >> 32);
			this[i + 5] = (byte) (x >> 40);
			this[i + 6] = (byte) (x >> 48);
			this[i + 7] = (byte) (x >> 56);
		}
		#endregion

	}
}

namespace Suneido.Database
{
	using NUnit.Framework;

	[TestFixture]
	public class ByteBufTest
	{
		[Test]
		public void Basic()
		{
			ByteBuffer buf = new ArrayBuffer(10);
			buf[0] = 123;
			Assert.That(buf[0], Is.EqualTo(123));

			buf.PutShort(0, 0x1234);
			Assert.That(buf.GetShort(0), Is.EqualTo(0x1234));
			buf.PutShort(2, 0x4321);
			Assert.That(buf.GetShort(2), Is.EqualTo(0x4321));
			buf = buf.Slice(2, 2);
			Assert.That(buf.GetShort(0), Is.EqualTo(0x4321));

			buf.PutInt(0, 0x12345678);
			Assert.That(buf.GetInt(0), Is.EqualTo(0x12345678));
			buf.PutInt(4, 0x76543210);
			Assert.That(buf.GetInt(4), Is.EqualTo(0x76543210));
			buf = buf.Slice(4, 4);
			Assert.That(buf.GetInt(0), Is.EqualTo(0x76543210));
		}

	}
}