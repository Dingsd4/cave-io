using System;
using System.IO;

namespace Cave.IO
{
    /// <summary>
    /// Bit Stream Reader Class for Bitstreams of the form: byte0[bit0,bit1,bit2,bit3,bit4,bit5,bit6,bit7] byte1[bit8,bit9,bit10,bit11,...]
    /// </summary>
    public class BitStreamWriter
    {
        int m_BufferedByte = 0;
        int m_Position = 0;
        Stream m_Stream;

        /// <summary>
        /// Obtains the BaseStream
        /// </summary>
        public Stream BaseStream => m_Stream;

        /// <summary>
        /// creates a new csBitStreamWriter
        /// </summary>
        /// <param name="stream"></param>
        public BitStreamWriter(Stream stream)
        {
            m_Stream = stream;
        }

        /// <summary>
        /// writes a bit to the buffer
        /// </summary>
        /// <returns></returns>
        public void WriteBit(bool items)
        {
            if (items)
            {
                int bitmask = (1 << (7 - m_Position));
                m_BufferedByte = m_BufferedByte | bitmask;
            }
            if (++m_Position > 7)
            {
                m_Stream.WriteByte((byte)m_BufferedByte);
                m_BufferedByte = 0;
                m_Position = 0;
            }
        }

        /// <summary>
        /// writes some bits
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
        /// writes some bits
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
        /// writes some bits (todo: optimize me)
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
        /// retrieves / sets the current bitposition
        /// </summary>
        public long Position => m_Stream.Position * 8 + m_Position;

        /// <summary>
        /// retrieves the length in bits
        /// </summary>
        public long Length => m_Stream.Length * 8 + m_Position;

        /// <summary>
        /// Closes the writer and the underlying stream
        /// </summary>
        public void Close()
        {
            Flush();
#if NETSTANDARD13
            m_Stream?.Dispose();
#else
            m_Stream?.Close();
#endif
            m_Stream = null;
        }

        /// <summary>
        /// Flushes the buffered bits to the stream and closes the writer (not the underlying stream)
        /// </summary>
        public void Flush()
        {
            if (m_Position > 0)
            {
                m_Stream.WriteByte((byte)m_BufferedByte);
            }
            m_Stream = null;
        }

        #region overrides
        /// <summary>
        /// Obtains the name of the class and the current state
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = base.ToString();
            if (m_Stream != null)
            {
                if (m_Stream.CanSeek)
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
        /// Obtains a hash code for this object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
