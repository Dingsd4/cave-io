using System;
using System.IO;

namespace Cave.IO
{
    /// <summary>
    /// Bit Stream Reader Class for Bitstreams of the form: byte0[bit0,bit1,bit2,bit3,bit4,bit5,bit6,bit7] byte1[bit8,bit9,bit10,bit11,...].
    /// </summary>
    public class BitStreamReader
    {
        int bufferedByte = 0;
        int position = -1;

        /// <summary>
        /// Gets the BaseStream.
        /// </summary>
        public Stream BaseStream { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitStreamReader"/> class.
        /// </summary>
        /// <param name="stream"></param>
        public BitStreamReader(Stream stream)
        {
            BaseStream = stream;
        }

        /// <summary>
        /// reads a bit from the buffer.
        /// </summary>
        /// <returns></returns>
        public uint ReadBit()
        {
            if (position < 0)
            {
                bufferedByte = BaseStream.ReadByte();
                if (bufferedByte == -1)
                {
                    throw new EndOfStreamException();
                }

                position = 7;
            }
            return (uint)((bufferedByte >> position--) & 1);
        }

        /// <summary>
        /// Checks for end of stream during bit reading. This can always be called, even if the stream cannot seek.
        /// </summary>
        public bool EndOfStream
        {
            get
            {
                if (position < 0)
                {
                    bufferedByte = BaseStream.ReadByte();
                    if (bufferedByte == -1)
                    {
                        return true;
                    }

                    position = 7;
                }
                return false;
            }
        }

        /// <summary>
        /// reads some bits.
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
        /// reads some bits.
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
        /// reads some bits.
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
        /// reads some bits.
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
        /// Gets or sets the current bitposition (Stream needs to provide Position getter and setter).
        /// </summary>
        public long Position
        {
            get
            {
                long pos = BaseStream.Position * 8;
                if (position > -1)
                {
                    pos += 7 - position - 8;
                }
                return pos;
            }
            set
            {
                BaseStream.Position = value / 8;
                long diff = value % 8;
                position = -1;
                if (diff == 0)
                {
                    return;
                }

                position = 7 - (int)diff;
                bufferedByte = BaseStream.ReadByte();
                if (bufferedByte == -1)
                {
                    throw new EndOfStreamException();
                }
            }
        }

        /// <summary>
        /// Gets the number of bits available (Stream needs to provide Position and Length getters!).
        /// </summary>
        public long Available => Length - Position;

        /// <summary>
        /// Gets the length in bits (Stream needs to provide Length getter!).
        /// </summary>
        public long Length => BaseStream.Length * 8;

        /// <summary>
        /// Closes the reader and the underlying stream.
        /// </summary>
        public void Close()
        {
#if NETSTANDARD13
            BaseStream?.Dispose();
#else
            BaseStream?.Close();
#endif
            BaseStream = null;
        }

        #region overrides

        /// <summary>
        /// Gets the name of the class and the current state.
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
        /// Gets a hash code for this object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
