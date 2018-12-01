using System;
using System.IO;

namespace Cave.IO
{
    /// <summary>
    /// Bit Stream Reader Class for reversed bitstreams of the form: byte0[bit7,bit6,bit5,bit4,bit3,bit2,bit1,bit0] byte1[bit15,bit14,bit13,bit12,...]
    /// </summary>
    public class BitStreamReaderReverse
    {
        int m_BufferedByte = 0;
        int m_Position = 8;
        Stream m_Stream;

        /// <summary>
        /// Obtains the BaseStream
        /// </summary>
        public Stream BaseStream => m_Stream;

        /// <summary>
        /// creates a new csBitStreamReader
        /// </summary>
        /// <param name="stream"></param>
        public BitStreamReaderReverse(Stream stream)
        {
            m_Stream = stream;
        }

        /// <summary>
        /// reads a bit from the buffer
        /// </summary>
        /// <returns></returns>
        public int ReadBit()
        {
            if (m_Position > 7)
            {
                m_BufferedByte = m_Stream.ReadByte();
                if (m_BufferedByte == -1)
                {
                    throw new EndOfStreamException();
                }

                m_Position = 0;
            }
            return (m_BufferedByte >> m_Position++) & 1;
        }

        /// <summary>
        /// reads some bits
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public long ReadBits64(int count)
        {
            if (Math.Abs(count) > 63)
            {
                throw new ArgumentException("count");
            }

            long result = 0;
            while (count-- > 0)
            {
                long bit = ReadBit();
                result = (result << 1) | bit;
            }
            return result;
        }

        /// <summary>
        /// reads some bits
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public int ReadBits32(int count)
        {
            if (Math.Abs(count) > 31)
            {
                throw new ArgumentException("count");
            }

            int result = 0;
            while (count-- > 0)
            {
                int bit = ReadBit();
                result = (result << 1) | bit;
            }
            return result;
        }

        /// <summary>
        /// retrieves / sets the current bitposition
        /// </summary>
        public long Position
        {
            get
            {
                long pos = m_Stream.Position * 8;
                if (m_Position < 8)
                {
                    pos += m_Position - 8;
                }
                return pos;
            }
            set
            {
                m_Stream.Position = value / 8;
                long diff = value % 8;
                m_Position = 8;
                if (diff == 0)
                {
                    return;
                }

                m_Position = (int)diff;
                m_BufferedByte = m_Stream.ReadByte();
                if (m_BufferedByte == -1)
                {
                    throw new EndOfStreamException();
                }
            }
        }

        /// <summary>
        /// retrieves the length in bits
        /// </summary>
        public long Length => m_Stream.Length * 8;

        /// <summary>
        /// Checks for end of stream during bit reading. This can always be called, even if the stream cannot seek.
        /// </summary>
        public bool EndOfStream
        {
            get
            {
                if (m_Position > 7)
                {
                    m_BufferedByte = m_Stream.ReadByte();
                    if (m_BufferedByte == -1)
                    {
                        return true;
                    }

                    m_Position = 0;
                }
                return false;
            }
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
