using System;

namespace Suneido.Database
{
	public class ArrayBuffer : ByteBuffer
	{
		readonly byte[] data;
		readonly int pos;
		readonly int len;

		public ArrayBuffer(int len)
		{
			data = new byte[len];
			this.len = len;
			pos = 0;
		}

		public ArrayBuffer(byte[] data)
		{
			this.data = data;
			pos = 0;
			len = data.Length;
		}

		public ArrayBuffer(byte[] data, int pos, int len)
		{
			Check.Slice(pos, len, data.Length);
			this.data = data;
			this.pos = pos;
			this.len = len;
		}

		public override byte this[int i]
		{
			get { return data[pos + i]; }
			set { data[pos + i] = value; }
		}

		public override int Length
		{
			get { return len; }
		}

		public override ByteBuffer Slice(int i, int n)
		{
			return new ArrayBuffer(data, pos + i, n);
		}

	}
}

