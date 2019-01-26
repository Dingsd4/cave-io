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
        /// Obtains the BaseStream.
        /// </summary>
        public Stream BaseStream { get; private set; }

        /// <summary>
        /// creates a new csBitStreamWriter.
        /// </summary>
        /// <param name="stream"></param>
        public BitStreamWriterReverse(Stream stream)
        {
            BaseStream = stream;
        }

        /// <summary>
        /// writes a bit to the buffer.
        /// </summary>
        /// <returns></returns>
        public void WriteBit(bool items)
        {
            if (items)
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
        /// <param name="bits"></param>
        /// <param name="count"></param>
        /// <returns></returns>
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
        /// <param name="bits"></param>
        /// <param name="count"></param>
        /// <returns></returns>
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
        /// <param name="count"></param>
        /// <param name="items"></param>
        public void WriteBits(int count, bool items)
        {
            for (int i = 0; i < count; i++)
            {
                WriteBit(items);
            }
        }

        /// <summary>
        /// retrieves / sets the current bitposition.
        /// </summary>
        public long Position => (BaseStream.Position * 8) + position;

        /// <summary>
        /// retrieves the length in bits.
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
        /// Obtains the name of the class and the current state.
        /// </summary>
        /// <returns></returns>
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
        /// Obtains a hash code for this object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
