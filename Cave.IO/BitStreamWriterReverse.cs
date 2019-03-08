using System;
using System.IO;

namespace Cave.IO
{
    /// <summary>
    /// Bit Stream Reader Class for Bitstreams of the form: byte0[bit7,bit6,bit5,bit4,bit3,bit2,bit1,bit0] byte1[bit15,bit14,bit13,bit12,...].
    /// </summary>
    public sealed class BitStreamWriterReverse
    {
        int bufferedByte = 0;
        int position = 0;

        /// <summary>
        /// Gets the BaseStream.
        /// </summary>
        public Stream BaseStream { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitStreamWriterReverse"/> class.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public BitStreamWriterReverse(Stream stream)
        {
            BaseStream = stream;
        }

        /// <summary>
        /// writes a bit to the buffer.
        /// </summary>
        /// <param name="bit">The bit.</param>
        public void WriteBit(bool bit)
        {
            if (bit)
            {
                int bitmask = 1 << position;
                bufferedByte = bufferedByte | bitmask;
            }
            if (++position > 7)
            {
                BaseStream.WriteByte((byte)bufferedByte);
                bufferedByte = 0;
                position = 0;
            }
        }

        /// <summary>
        /// writes some bits.
        /// </summary>
        /// <param name="bits">The bits to write.</param>
        /// <param name="count">Number of bits to write.</param>
        public void WriteBits(long bits, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (Math.Abs(count) > 63)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            for (int i = count - 1; i > -1; i--)
            {
                WriteBit(((bits >> i) & 1) != 0);
            }
        }

        /// <summary>
        /// writes some bits.
        /// </summary>
        /// <param name="bits">The bits to write.</param>
        /// <param name="count">Number of bits to write.</param>
        public void WriteBits(int bits, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (Math.Abs(count) > 31)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            for (int i = count - 1; i > -1; i--)
            {
                WriteBit(((bits >> i) & 1) != 0);
            }
        }

        /// <summary>
        /// writes some bits (todo: optimize me).
        /// </summary>
        /// <param name="count">Number of bits to write.</param>
        /// <param name="bit">The bit to write count times.</param>
        public void WriteBits(int count, bool bit)
        {
            for (int i = 0; i < count; i++)
            {
                WriteBit(bit);
            }
        }

        /// <summary>
        /// Gets the current bitposition.
        /// </summary>
        public long Position => (BaseStream.Position * 8) + position;

        /// <summary>
        /// Gets retrieves the length in bits.
        /// </summary>
        public long Length => (BaseStream.Length * 8) + position;

        /// <summary>
        /// Closes the writer and the underlying stream.
        /// </summary>
        public void Close()
        {
            Flush();
#if NETSTANDARD13
            BaseStream?.Dispose();
#else
            BaseStream?.Close();
#endif
            BaseStream = null;
        }

        /// <summary>
        /// Flushes the buffered bits to the stream and closes the writer (not the underlying stream).
        /// </summary>
        public void Flush()
        {
            if (position > 0)
            {
                BaseStream.WriteByte((byte)bufferedByte);
            }
            BaseStream = null;
        }

        #region overrides

        /// <summary>
        /// Gets the name of the class and the current state.
        /// </summary>
        /// <returns>The class name and the current state.</returns>
        public override string ToString()
        {
            string result = base.ToString();
            if (BaseStream != null)
            {
                if (BaseStream.CanSeek)
                {
                    result += " [" + Position + "/" + Length + "]";
                }
                else
                {
                    result += " opened";
                }
            }
            else
            {
                result += " closed";
            }
            return result;
        }

        /// <summary>
        /// Gets a hash code for this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
