using System;
using System.Threading;

namespace Cave.IO
{
    /// <summary>Provides a lock free ring buffer without overflow checking.</summary>
    /// <typeparam name="TValue">Item type.</typeparam>
    public class UncheckedRingBuffer<TValue> : IRingBuffer<TValue> where TValue : class
    {
        readonly TValue[] Buffer;
        readonly int Mask;
        long readCount;
        int readPosition;
        long writeCount;
        int writePosition = -1;

        /// <summary>Initializes a new instance of the <see cref="UncheckedRingBuffer{TValue}" /> class.</summary>
        /// <param name="bits">Number of bits to use for item capacity (defaults to 12 = 4096 items).</param>
        public UncheckedRingBuffer(int bits = 12)
        {
            if (bits < 1 || bits > 31)
            {
                throw new ArgumentOutOfRangeException(nameof(bits));
            }

            Buffer = new TValue[1 << bits];
            Mask = Capacity - 1;
        }

        /// <inheritdoc/>
        public int Available
        {
            get
            {
                var diff = writePosition - readPosition;
                if (diff < 0) diff = Capacity - diff;
                return diff;
            }
        }

        /// <inheritdoc/>
        public int Capacity => Buffer.Length;

        /// <inheritdoc/>
        public long LostCount => Math.Max(0, WriteCount - ReadCount - Capacity);

        /// <inheritdoc/>
        public long ReadCount => Interlocked.Read(ref readCount);

        /// <inheritdoc/>
        public int ReadPosition => readPosition;

        /// <inheritdoc/>
        public long RejectedCount => 0;

        /// <inheritdoc/>
        public long WriteCount => Interlocked.Read(ref writeCount);

        /// <inheritdoc/>
        public int WritePosition => writePosition;

        /// <inheritdoc/>
        public void CopyTo(TValue[] array, int index) => Buffer.CopyTo(array, index);

        /// <inheritdoc/>
        public TValue Read()
        {
            var i = Interlocked.Increment(ref readPosition) & Mask;
            var result = Buffer[i];
            Interlocked.Increment(ref readCount);
            return result;
        }

        /// <inheritdoc/>
        public TValue[] ToArray()
        {
            var clone = new TValue[Capacity];
            CopyTo(clone, 0);
            return clone;
        }

        /// <inheritdoc/>
        public bool TryRead(out TValue item)
        {
            if (readPosition >= writePosition)
            {
                item = default;
                return false;
            }

            item = Read();
            return true;
        }

        /// <inheritdoc/>
        public bool Write(TValue item)
        {
            var i = Interlocked.Increment(ref writePosition) & Mask;
            Buffer[i] = item;
            Interlocked.Increment(ref writeCount);
            return true;
        }
    }
}
