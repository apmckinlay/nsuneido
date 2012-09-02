using System;
using System.IO.MemoryMappedFiles;

namespace Suneido.Database
{
	public class MappedBuffer : ByteBuffer
	{
		readonly MemoryMappedViewAccessor view;
		readonly int pos;
		readonly int len;

		public MappedBuffer(MemoryMappedViewAccessor view, int pos, int len)
		{
			this.view = view;
			this.pos = pos;
			this.len = len;
			Check.Slice(pos, len, view.Capacity);
		}

		public override byte this[int i]
		{
			get { return view.ReadByte(pos + i); }
			set { view.Write(pos + i, value); }
		}

		public override int Length
		{
			get { return len; }
		}

		public override ByteBuffer Slice(int i, int n)
		{
			return new MappedBuffer(view, pos + i, n);
		}

	}
}

