using System;
using System.Diagnostics;
using System.Threading;

namespace Cave.IO
{
    /// <summary>Provides a lock free circular buffer with overflow checking.</summary>
    /// <typeparam name="TValue">Item type.</typeparam>
    public class CircularBuffer<TValue> : IRingBuffer<TValue> where TValue : class
    {
        readonly TValue[] Buffer;
        readonly int Mask;
        int queued;
        long readCount;
        int readPosition;
        long rejected;
        long writeCount;
        int writePosition = -1;

        /// <summary>Initializes a new instance of the <see cref="CircularBuffer{TValue}" /> class.</summary>
        /// <param name="bits">Number of bits to use for item capacity (defaults to 12 = 4096 items).</param>
        public CircularBuffer(int bits = 12)
        {
            if (bits < 1 || bits > 31)
            {
                throw new ArgumentOutOfRangeException(nameof(bits));
            }

            Buffer = new TValue[1 << bits];
            Mask = Capacity - 1;
        }

        /// <inheritdoc/>
        public int ReadPosition => readPosition;

        /// <inheritdoc/>
        public int WritePosition => writePosition;

        /// <inheritdoc/>
        public long RejectedCount => Interlocked.Read(ref rejected);

        /// <summary>Gets the maximum number of items queued.</summary>
        public int MaxQueuedCount { get; set; }

        /// <inheritdoc/>
        public long ReadCount => Interlocked.Read(ref readCount);

        /// <inheritdoc/>
        public long WriteCount => Interlocked.Read(ref writeCount);

        /// <summary>Write overflow (buffer under run) to <see cref="Trace" />.</summary>
        public bool OverflowTrace { get; set; }

        /// <summary>Throw exceptions at <see cref="Write" /> on overflow (buffer under run)</summary>
        public bool OverflowExceptions { get; set; }

        /// <inheritdoc/>
        public int Capacity => Buffer.Length;

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
        public long LostCount => 0;

        /// <inheritdoc/>
        public bool Write(TValue item)
        {
            var n = Interlocked.Increment(ref queued);
            if (n > Mask)
            {
                Interlocked.Decrement(ref queued);
                Interlocked.Increment(ref rejected);
                const string Message = "Buffer overflow!";
                if (OverflowExceptions)
                {
                    throw new Exception(Message);
                }

                if (OverflowTrace)
                {
                    Trace.TraceError(Message);
                }

                return false;
            }

            if (n > MaxQueuedCount)
            {
                MaxQueuedCount = n;
            }

            var i = Interlocked.Increment(ref writePosition) & Mask;
            Buffer[i] = item;
            Interlocked.Increment(ref writeCount);
            return true;
        }

        /// <inheritdoc/>
        public bool TryRead(out TValue item)
        {
            var n = Interlocked.Decrement(ref queued);
            if (n < 0)
            {
                Interlocked.Increment(ref queued);
                item = default;
                return false;
            }

            var i = Interlocked.Increment(ref readPosition) & Mask;
            item = Buffer[i];
            Interlocked.Increment(ref readCount);
            return true;
        }

        /// <inheritdoc/>
        public TValue Read() => TryRead(out var result) ? result : default;

        /// <inheritdoc/>
        public TValue[] ToArray() => (TValue[])Buffer.Clone();

        /// <inheritdoc/>
        public void CopyTo(TValue[] array, int index) => Buffer.CopyTo(array, index);
    }
}
