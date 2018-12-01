using System;
using System.IO;

namespace Cave.IO
{
    /// <summary>
    /// Bit Stream Reader Class for Bitstreams of the form: byte0[bit0,bit1,bit2,bit3,bit4,bit5,bit6,bit7] byte1[bit8,bit9,bit10,bit11,...]
    /// </summary>
    public class BitStreamReader
    {
        int m_BufferedByte = 0;
        int m_Position = -1;
        Stream m_Stream;

        /// <summary>
        /// Obtains the BaseStream
        /// </summary>
        public Stream BaseStream => m_Stream;

        /// <summary>
        /// creates a new BitStreamReader
        /// </summary>
        /// <param name="stream"></param>
        public BitStreamReader(Stream stream)
        {
            m_Stream = stream;
        }

        /// <summary>
        /// reads a bit from the buffer
        /// </summary>
        /// <returns></returns>
        public uint ReadBit()
        {
            if (m_Position < 0)
            {
                m_BufferedByte = m_Stream.ReadByte();
                if (m_BufferedByte == -1)
                {
                    throw new EndOfStreamException();
                }

                m_Position = 7;
            }
            return (uint)((m_BufferedByte >> m_Position--) & 1);
        }

        /// <summary>
        /// Checks for end of stream during bit reading. This can always be called, even if the stream cannot seek.
        /// </summary>
        public bool EndOfStream
        {
            get
            {
                if (m_Position < 0)
                {
                    m_BufferedByte = m_Stream.ReadByte();
                    if (m_BufferedByte == -1)
                    {
                        return true;
                    }

                    m_Position = 7;
                }
                return false;
            }
        }

        /// <summary>
        /// reads some bits
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public ulong ReadBits64(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return ReadBits64((uint)count);
        }

        /// <summary>
        /// reads some bits
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public ulong ReadBits64(uint count)
        {
            if (Math.Abs(count) > 64)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            ulong result = 0;
            while (count-- > 0)
            {
                ulong bit = ReadBit();
                result = (result << 1) | bit;
            }
            return result;
        }

        /// <summary>
        /// reads some bits
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public uint ReadBits32(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return ReadBits32((uint)count);
        }

        /// <summary>
        /// reads some bits
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public uint ReadBits32(uint count)
        {
            if (Math.Abs(count) > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            uint result = 0;
            while (count-- > 0)
            {
                uint bit = ReadBit();
                result = (result << 1) | bit;
            }
            return result;
        }

        /// <summary>
        /// Retrieves / Sets the current bitposition (Stream needs to provide Position getter and setter)
        /// </summary>
        public long Position
        {
            get
            {
                long pos = m_Stream.Position * 8;
                if (m_Position > -1)
                {
                    pos += (7 - m_Position) - 8;
                }
                return pos;
            }
            set
            {
                m_Stream.Position = value / 8;
                long diff = value % 8;
                m_Position = -1;
                if (diff == 0)
                {
                    return;
                }

                m_Position = 7 - (int)diff;
                m_BufferedByte = m_Stream.ReadByte();
                if (m_BufferedByte == -1)
                {
                    throw new EndOfStreamException();
                }
            }
        }

        /// <summary>
        /// Obtains the number of bits available (Stream needs to provide Position and Length getters!)
        /// </summary>
        public long Available => Length - Position;

        /// <summary>
        /// Retrieves the length in bits (Stream needs to provide Length getter!)
        /// </summary>
        public long Length => m_Stream.Length * 8;

        /// <summary>
        /// Closes the reader and the underlying stream
        /// </summary>
        public void Close()
        {
#if NETSTANDARD13
            m_Stream?.Dispose();
#else
            m_Stream?.Close();
#endif
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
