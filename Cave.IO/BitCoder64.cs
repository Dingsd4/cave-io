using System;
using System.IO;

namespace Cave.IO
{
    /// <summary>
    /// Provides 7bit encoding of 64bit values (ulong, ulong).
    /// </summary>
    public static class BitCoder64
    {
        /// <summary>
        /// Obtains the number of bytes needed for the specified value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetByteCount7BitEncoded(ulong value)
        {
            unchecked
            {
                int count = 0;
                do
                {
                    count++;
                    value >>= 7;
                }
                while (value != 0);
                return count;
            }
        }

        /// <summary>
        /// Obtains the number of bytes needed for the specified value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetByteCount7BitEncoded(long value)
        {
            unchecked
            {
                return GetByteCount7BitEncoded((ulong)value);
            }
        }

        /// <summary>
        /// Obtains the data of a 7 bit encoded value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Get7BitEncoded(ulong value)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Write7BitEncoded(stream, value);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Obtains the data of a 7 bit encoded value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Get7BitEncoded(long value)
        {
            unchecked
            {
                return Get7BitEncoded((ulong)value);
            }
        }

        /// <summary>
        /// Reads a 7 bit encoded value from the specified Stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read from.</param>
        /// <returns>Returns the read value.</returns>
        public static ulong Read7BitEncodedUInt64(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            unchecked
            {
                int b = stream.ReadByte();
                int count = 1;
                if (b == -1)
                {
                    throw new EndOfStreamException();
                }

                ulong result = (ulong)(b & 0x7F);
                int bitPos = 7;
                while (b > 0x7F)
                {
                    b = stream.ReadByte();
                    if (++count > 10)
                    {
                        throw new InvalidDataException(string.Format("7Bit encoded 64 bit integer may not exceed 10 bytes!"));
                    }

                    if (b == -1)
                    {
                        throw new EndOfStreamException();
                    }

                    ulong value = (ulong)(b & 0x7F);
                    result = value << bitPos | result;
                    bitPos += 7;
                }
                return result;
            }
        }

        /// <summary>
        /// Reads a 7 bit encoded value from the specified Stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read from.</param>
        /// <returns>Returns the read value.</returns>
        public static long Read7BitEncodedInt64(Stream stream)
        {
            unchecked
            {
                return (long)Read7BitEncodedUInt64(stream);
            }
        }

        /// <summary>
        /// Writes the specified value 7 bit encoded to the specified Stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns the number of bytes written.</returns>
        public static int Write7BitEncoded(Stream stream, ulong value)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            unchecked
            {
                int i = 1;
                byte b = (byte)(value & 0x7F);
                ulong data = value >> 7;
                while (data != 0)
                {
                    stream.WriteByte((byte)(0x80 | b));
                    i++;
                    b = (byte)(data & 0x7F);
                    data >>= 7;
                }
                stream.WriteByte(b);
                return i;
            }
        }

        /// <summary>
        /// Writes the specified value 7 bit encoded to the specified Stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns the number of bytes written.</returns>
        public static int Write7BitEncoded(Stream stream, long value)
        {
            unchecked
            {
                return Write7BitEncoded(stream, (ulong)value);
            }
        }

        /// <summary>
        /// Writes the specified value 7 bit encoded to the specified Stream.
        /// </summary>
        /// <param name="writer">The <see cref="DataWriter"/> to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns the number of bytes written.</returns>
        public static int Write7BitEncoded(DataWriter writer, ulong value)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            unchecked
            {
                int i = 1;
                byte b = (byte)(value & 0x7F);
                ulong data = value >> 7;
                while (data != 0)
                {
                    writer.Write((byte)(0x80 | b));
                    i++;
                    b = (byte)(data & 0x7F);
                    data >>= 7;
                }
                writer.Write(b);
                return i;
            }
        }

        /// <summary>
        /// Writes the specified value 7 bit encoded to the specified Stream.
        /// </summary>
        /// <param name="writer">The <see cref="DataWriter"/> to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns the number of bytes written.</returns>
        public static int Write7BitEncoded(DataWriter writer, long value)
        {
            unchecked
            {
                return Write7BitEncoded(writer, (ulong)value);
            }
        }
    }
}
