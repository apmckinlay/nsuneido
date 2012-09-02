using System;
using Suneido.Utility;

namespace Suneido.Database
{
	[ThreadSafe]
	internal abstract class Storage
	{
		const int FIRST_ADR = 1;
		const int SHIFT = 3;
		const long MAX_SIZE = 0xffffffffL << SHIFT;
		const int ALIGN = (1 << SHIFT); // must be power of 2
		const int MASK = ALIGN - 1;
		protected readonly int CHUNK_SIZE;
		protected ByteBuffer[] chunks;
		protected /*volatile*/ long file_size;

		internal Storage(int chunkSize, int initChunks)
		{
			CHUNK_SIZE = chunkSize;
			chunks = new ByteBuffer[initChunks];
		}

		/**
		 * Allocate a block of storage.
		 * It will be aligned, and may require advancing to next chunk.
		 * (Leaving padding filled with zero bytes.)
		 * @param n The size of the block required.
		 * @return The "address" of the block. (Not just an offset.)
		 */
		/*synchronized*/
		internal int Alloc(int n)
		{
			Check.That(n < CHUNK_SIZE, n + " not < " + CHUNK_SIZE);
			n = Align(n);

			// if insufficient room in this chunk, advance to next
			int remaining = CHUNK_SIZE - (int)(file_size % CHUNK_SIZE);
			if (n > remaining)
				file_size += remaining;

			long offset = file_size;
			file_size += n;

			int chunk = OffsetToChunk(offset);
			if (chunk >= chunks.Length)
				growChunks(chunk);

			return OffsetToAdr(offset);
		}

		void growChunks(int chunk)
		{
			chunks = Arrays.CopyOf(chunks, 2 * chunk);
		}

		internal static int Align(int n)
		{
			// requires ALIGN to be power of 2
			return ((n - 1) | (ALIGN - 1)) + 1;
		}

		internal int Advance(int adr, int length)
		{
			long offset = AdrToOffset(adr);
			offset += Align(length);
			if (offset < file_size)
			{
				ByteBuffer buf = GetBuf(offset);
				if (buf.GetLong(0) == 0)
					// skip end of chunk padding
					offset += buf.Length;
			}
			return OffsetToAdr(offset);
		}

		/**
		 * @param offset An address (not an offset) as returned by alloc
		 * @returns A unique instance of a ByteBuffer
		 * i.e. not shared so it may be modified.
		 * extending from the offset to the end of the chunk.
		 */
		internal ByteBuffer Buffer(int adr)
		{
			Check.That(adr != 0, "storage address should never be 0");
			return GetBuf(AdrToOffset(adr));
		}

		/**
		 * @param rpos A negative offset from the end of the file
		 */
		internal ByteBuffer Rbuffer(long rpos)
		{
			Check.That(rpos < 0);
			return GetBuf(file_size + rpos);
		}

		internal int RposToAdr(long rpos)
		{
			Check.That(rpos < 0);
			return OffsetToAdr(file_size + rpos);
		}

		private ByteBuffer GetBuf(long offset)
		{
			ByteBuffer buf = Map(offset);
			int pos = (int)(offset % CHUNK_SIZE);
			long startOfLastChunk = (file_size / CHUNK_SIZE) * CHUNK_SIZE;
			int len = (offset >= startOfLastChunk)
				? (int)(file_size - startOfLastChunk)
				: CHUNK_SIZE - pos;
			return buf.Slice(pos, len);
		}

		/** @return the chunk containing the specified offset */
		protected ByteBuffer Map(long offset)
		{
			Check.That(0 <= offset && offset < file_size);
			int chunk = OffsetToChunk(offset);
			if (chunks[chunk] == null)
			{
				chunks[chunk] = Get(chunk);
			}
			return chunks[chunk];
		}

		protected int OffsetToChunk(long offset)
		{
			return (int)(offset / CHUNK_SIZE);
		}

		protected abstract ByteBuffer Get(int chunk);

		internal static int OffsetToAdr(long n)
		{
			Check.That((n & MASK) == 0);
			Check.That(n <= MAX_SIZE);
			return (int)((ulong)n >> SHIFT) + 1; // +1 to avoid 0
		}

		internal static long AdrToOffset(int adr)
		{
			return ((adr - 1) & 0xffffffffL) << SHIFT;
		}

		/**
		 * Faster than buffer because it does not slice.<p>
		 * @return The buffer containing the address.
		 */
		internal ByteBuffer BufferBase(int adr)
		{
			return Map(AdrToOffset(adr));
		}

		/** @return The position of adr in bufferBase */
		internal int BufferPos(int adr)
		{
			return (int)(AdrToOffset(adr) % CHUNK_SIZE);
		}

		/** @return checksum for bytes from adr to end of file */
//		int checksum(int adr) {
//			Checksum cksum = new Checksum();
//			long offset = adrToOffset(adr);
//			while (offset < file_size) {
//				ByteBuffer buf = buf(offset);
//				offset += buf.remaining();
//				cksum.update(buf);
//			}
//			return cksum.getValue();
//		}

		/** @return Number of bytes from adr to current offset */
		internal long SizeFrom(int adr)
		{
			return adr == 0 ? file_size : file_size - AdrToOffset(adr);
		}

		internal bool isValidPos(long pos)
		{
			if (pos < 0)
				pos += file_size;
			return 0 <= pos && pos < file_size;
		}

		internal virtual void force()
		{
		}

		internal virtual void close()
		{
		}

	}
}

