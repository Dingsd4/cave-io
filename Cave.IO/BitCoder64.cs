using System;
using System.IO;

namespace Cave.IO
{
    /// <summary>Provides 7bit encoding of 64bit values (ulong, ulong).</summary>
    public static class BitCoder64
    {
        /// <summary>Gets the number of bytes needed for the specified value.</summary>
        /// <param name="value">The value to encode.</param>
        /// <returns>number of bytes needed.</returns>
        public static int GetByteCount7BitEncoded(ulong value)
        {
            unchecked
            {
                var count = 0;
                do
                {
                    count++;
                    value >>= 7;
                }
                while (value != 0);

                return count;
            }
        }

        /// <summary>Gets the number of bytes needed for the specified value.</summary>
        /// <param name="value">The value to encode.</param>
        /// <returns>number of bytes needed.</returns>
        public static int GetByteCount7BitEncoded(long value)
        {
            unchecked
            {
                return GetByteCount7BitEncoded((ulong) value);
            }
        }

        /// <summary>Gets the data of a 7 bit encoded value.</summary>
        /// <param name="value">The value to encode.</param>
        /// <returns>The encoded value as byte array.</returns>
        public static byte[] Get7BitEncoded(ulong value)
        {
            using var stream = new MemoryStream();
            Write7BitEncoded(stream, value);
            return stream.ToArray();
        }

        /// <summary>Gets the data of a 7 bit encoded value.</summary>
        /// <param name="value">The value to encode.</param>
        /// <returns>The encoded value as byte array.</returns>
        public static byte[] Get7BitEncoded(long value)
        {
            unchecked
            {
                return Get7BitEncoded((ulong) value);
            }
        }

        /// <summary>Reads a 7 bit encoded value from the specified Stream.</summary>
        /// <param name="stream">The <see cref="Stream" /> to read from.</param>
        /// <returns>Returns the read value.</returns>
        public static ulong Read7BitEncodedUInt64(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            unchecked
            {
                var b = stream.ReadByte();
                var count = 1;
                if (b == -1)
                {
                    throw new EndOfStreamException();
                }

                var result = (ulong) (b & 0x7F);
                var bitPos = 7;
                while (b > 0x7F)
                {
                    b = stream.ReadByte();
                    if (++count > 10)
                    {
                        throw new InvalidDataException("7Bit encoded 64 bit integer may not exceed 10 bytes!");
                    }

                    if (b == -1)
                    {
                        throw new EndOfStreamException();
                    }

                    var value = (ulong) (b & 0x7F);
                    result = (value << bitPos) | result;
                    bitPos += 7;
                }

                return result;
            }
        }

        /// <summary>Reads a 7 bit encoded value from the specified Stream.</summary>
        /// <param name="stream">The <see cref="Stream" /> to read from.</param>
        /// <returns>Returns the read value.</returns>
        public static long Read7BitEncodedInt64(Stream stream)
        {
            unchecked
            {
                return (long) Read7BitEncodedUInt64(stream);
            }
        }

        /// <summary>Writes the specified value 7 bit encoded to the specified Stream.</summary>
        /// <param name="stream">The <see cref="Stream" /> to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns the number of bytes written.</returns>
        public static int Write7BitEncoded(Stream stream, ulong value)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            unchecked
            {
                var i = 1;
                var b = (byte) (value & 0x7F);
                var data = value >> 7;
                while (data != 0)
                {
                    stream.WriteByte((byte) (0x80 | b));
                    i++;
                    b = (byte) (data & 0x7F);
                    data >>= 7;
                }

                stream.WriteByte(b);
                return i;
            }
        }

        /// <summary>Writes the specified value 7 bit encoded to the specified Stream.</summary>
        /// <param name="stream">The <see cref="Stream" /> to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns the number of bytes written.</returns>
        public static int Write7BitEncoded(Stream stream, long value)
        {
            unchecked
            {
                return Write7BitEncoded(stream, (ulong) value);
            }
        }

        /// <summary>Writes the specified value 7 bit encoded to the specified Stream.</summary>
        /// <param name="writer">The <see cref="DataWriter" /> to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns the number of bytes written.</returns>
        public static int Write7BitEncoded(DataWriter writer, ulong value)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            unchecked
            {
                var i = 1;
                var b = (byte) (value & 0x7F);
                var data = value >> 7;
                while (data != 0)
                {
                    writer.Write((byte) (0x80 | b));
                    i++;
                    b = (byte) (data & 0x7F);
                    data >>= 7;
                }

                writer.Write(b);
                return i;
            }
        }

        /// <summary>Writes the specified value 7 bit encoded to the specified Stream.</summary>
        /// <param name="writer">The <see cref="DataWriter" /> to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns the number of bytes written.</returns>
        public static int Write7BitEncoded(DataWriter writer, long value)
        {
            unchecked
            {
                return Write7BitEncoded(writer, (ulong) value);
            }
        }
    }
}
