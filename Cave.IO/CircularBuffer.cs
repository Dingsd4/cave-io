using System;
using System.Diagnostics;
using System.Threading;

namespace Cave.IO
{
    /// <summary>Provides a lock free circular buffer.</summary>
    /// <typeparam name="TValue">Item type.</typeparam>
    public class CircularBuffer<TValue>
    {
        readonly TValue[] buffer;
        readonly int mask;
        int queued;
        long readCount;
        int readPosition;
        long rejected;
        long writeCount;
        int writePosition;

        /// <summary>Initializes a new instance of the <see cref="CircularBuffer{TValue}" /> class.</summary>
        /// <param name="bits">Number of bits to use for item capacity.</param>
        public CircularBuffer(int bits)
        {
            if (bits < 2)
            {
                throw new Exception();
            }

            if (bits > 31)
            {
                throw new Exception();
            }

            Capacity = 1 << bits;
            mask = Capacity - 1;
            buffer = new TValue[Capacity];
        }

        /// <summary>Gets the current read position [0..Capacity-1].</summary>
        public int ReadPosition => readPosition;

        /// <summary>Gets the current write position [0..Capacity-1].</summary>
        public int WritePosition => writePosition;

        /// <summary>Gets the number of rejected items (items that could not be queued).</summary>
        public long RejectedCount => Interlocked.Read(ref rejected);

        /// <summary>Gets the maximum number of items queued.</summary>
        public int MaxQueuedCount { get; set; }

        /// <summary>Gets the number of successful <see cref="Read" /> calls.</summary>
        public long ReadCount => Interlocked.Read(ref readCount);

        /// <summary>Gets the number of successful <see cref="Write" /> calls.</summary>
        public long WriteCount => Interlocked.Read(ref writeCount);

        /// <summary>Write overflow (buffer under run) to <see cref="Trace" />.</summary>
        public bool OverflowTrace { get; private set; }

        /// <summary>Throw exceptions at <see cref="Write" /> on overflow (buffer under run)</summary>
        public bool OverflowExceptions { get; private set; }

        /// <summary>Gets the number of items maximum present at the buffer.</summary>
        public int Capacity { get; }

        /// <summary>Writes an item to the buffer if there is space left.</summary>
        /// <param name="item">Item to write to the buffer.</param>
        /// <returns>Returns true if the item could be written, false otherwise.</returns>
        public bool Write(TValue item)
        {
            var n = Interlocked.Increment(ref queued);
            if (n > mask)
            {
                Interlocked.Decrement(ref queued);
                Interlocked.Increment(ref rejected);
                const string message = "Buffer overflow!";
                if (OverflowExceptions)
                {
                    throw new Exception(message);
                }

                if (OverflowTrace)
                {
                    Trace.TraceError(message);
                }

                return false;
            }

            if (n > MaxQueuedCount)
            {
                MaxQueuedCount = n;
            }

            var i = Interlocked.Increment(ref writePosition) & mask;
            buffer[i] = item;
            Interlocked.Increment(ref writeCount);
            return true;
        }

        /// <summary>Reads an item from the buffer is there is one left.</summary>
        /// <param name="item">Item read from the buffer or default.</param>
        /// <returns>Returns true if the item could be read, false otherwise.</returns>
        public bool Read(out TValue item)
        {
            var n = Interlocked.Decrement(ref queued);
            if (n < 0)
            {
                Interlocked.Increment(ref queued);
                item = default;
                return false;
            }

            var i = Interlocked.Increment(ref readPosition) & mask;
            item = buffer[i];
            Interlocked.Increment(ref readCount);
            return true;
        }
    }
}
