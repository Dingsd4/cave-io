using System;
using System.IO;

namespace Cave.IO
{
    /// <summary>Provides 7bit encoding of 64bit values (uint, uint).</summary>
    public static class BitCoder32
    {
        /// <summary>Gets the number of bytes needed for the specified value.</summary>
        /// <param name="value">The value to encode.</param>
        /// <returns>number of bytes needed.</returns>
        public static int GetByteCount7BitEncoded(uint value)
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
        public static int GetByteCount7BitEncoded(int value)
        {
            unchecked
            {
                return GetByteCount7BitEncoded((uint) value);
            }
        }

        /// <summary>Gets the data of a 7 bit encoded value.</summary>
        /// <param name="value">The value to encode.</param>
        /// <returns>The encoded value as byte array.</returns>
        public static byte[] Get7BitEncoded(uint value)
        {
            using var stream = new MemoryStream();
            Write7BitEncoded(stream, value);
            return stream.ToArray();
        }

        /// <summary>Gets the data of a 7 bit encoded value.</summary>
        /// <param name="value">The value to encode.</param>
        /// <returns>The encoded value as byte array.</returns>
        public static byte[] Get7BitEncoded(int value)
        {
            unchecked
            {
                return Get7BitEncoded((uint) value);
            }
        }

        /// <summary>Reads a 7 bit encoded value from the specified Stream.</summary>
        /// <param name="stream">The <see cref="Stream" /> to read from.</param>
        /// <returns>Returns the read value.</returns>
        public static uint Read7BitEncodedUInt32(Stream stream)
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

                var result = (uint) (b & 0x7F);
                var bitPos = 7;
                while (b > 0x7F)
                {
                    b = stream.ReadByte();
                    if (++count > 5)
                    {
                        throw new InvalidDataException("7Bit encoded 32 bit integer may not exceed 5 bytes!");
                    }

                    if (b == -1)
                    {
                        throw new EndOfStreamException();
                    }

                    var value = (uint) (b & 0x7F);
                    result = (value << bitPos) | result;
                    bitPos += 7;
                }

                return result;
            }
        }

        /// <summary>Reads a 7 bit encoded value from the specified Stream.</summary>
        /// <param name="stream">The <see cref="Stream" /> to read from.</param>
        /// <returns>Returns the read value.</returns>
        public static int Read7BitEncodedInt32(Stream stream)
        {
            unchecked
            {
                return (int) Read7BitEncodedUInt32(stream);
            }
        }

        /// <summary>Writes the specified value 7 bit encoded to the specified Stream.</summary>
        /// <param name="stream">The <see cref="Stream" /> to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns the number of bytes written.</returns>
        public static int Write7BitEncoded(Stream stream, uint value)
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
        public static int Write7BitEncoded(Stream stream, int value)
        {
            unchecked
            {
                return Write7BitEncoded(stream, (uint) value);
            }
        }

        /// <summary>Writes the specified value 7 bit encoded to the specified Stream.</summary>
        /// <param name="writer">The <see cref="DataWriter" /> to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>Returns the number of bytes written.</returns>
        public static int Write7BitEncoded(DataWriter writer, uint value)
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
        public static int Write7BitEncoded(DataWriter writer, int value)
        {
            unchecked
            {
                return Write7BitEncoded(writer, (uint) value);
            }
        }
    }
}
