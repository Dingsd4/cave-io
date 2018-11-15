#region CopyRight 2018
/*
    Copyright (c) 2003-2018 Andreas Rohleder (andreas@rohleder.cc)
    All rights reserved
*/
#endregion
#region License LGPL-3
/*
    This program/library/sourcecode is free software; you can redistribute it
    and/or modify it under the terms of the GNU Lesser General Public License
    version 3 as published by the Free Software Foundation subsequent called
    the License.

    You may not use this program/library/sourcecode except in compliance
    with the License. The License is included in the LICENSE file
    found at the installation directory or the distribution package.

    Permission is hereby granted, free of charge, to any person obtaining
    a copy of this software and associated documentation files (the
    "Software"), to deal in the Software without restriction, including
    without limitation the rights to use, copy, modify, merge, publish,
    distribute, sublicense, and/or sell copies of the Software, and to
    permit persons to whom the Software is furnished to do so, subject to
    the following conditions:

    The above copyright notice and this permission notice shall be included
    in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
    OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion
#region Authors & Contributors
/*
   Author:
     Andreas Rohleder <andreas@rohleder.cc>

   Contributors:

 */
#endregion

using System;
using System.IO;

namespace Cave.IO
{
    /// <summary>
    /// Provides 7bit encoding of 64bit values (uint, uint)
    /// </summary>
    public static class BitCoder32
    {
        /// <summary>
        /// Obtains the number of bytes needed for the specified value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetByteCount7BitEncoded(uint value)
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
        /// Obtains the number of bytes needed for the specified value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetByteCount7BitEncoded(int value)
        {
            unchecked
            {
                return GetByteCount7BitEncoded((uint)value);
            }
        }

        /// <summary>
        /// Obtains the data of a 7 bit encoded value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Get7BitEncoded(uint value)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Write7BitEncoded(stream, value);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Obtains the data of a 7 bit encoded value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Get7BitEncoded(int value)
        {
            unchecked
            {
                return Get7BitEncoded((uint)value);
            }
        }

        /// <summary>
        /// Reads a 7 bit encoded value from the specified Stream
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read from</param>
        /// <returns>Returns the read value</returns>
        public static uint Read7BitEncodedUInt32(Stream stream)
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

                uint result = (uint)(b & 0x7F);
                int bitPos = 7;
                while (b > 0x7F)
                {
                    b = stream.ReadByte();
                    if (++count > 5)
                    {
                        throw new InvalidDataException(string.Format("7Bit encoded 32 bit integer may not exceed 5 bytes!"));
                    }

                    if (b == -1)
                    {
                        throw new EndOfStreamException();
                    }

                    uint value = (uint)(b & 0x7F);
                    result = value << bitPos | result;
                    bitPos += 7;
                }
                return result;
            }
        }

        /// <summary>
        /// Reads a 7 bit encoded value from the specified Stream
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read from</param>
        /// <returns>Returns the read value</returns>
        public static int Read7BitEncodedInt32(Stream stream)
        {
            unchecked
            {
                return (int)Read7BitEncodedUInt32(stream);
            }
        }

        /// <summary>
        /// Writes the specified value 7 bit encoded to the specified Stream
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write to</param>
        /// <param name="value">The value to write</param>
        /// <returns>Returns the number of bytes written</returns>
        public static int Write7BitEncoded(Stream stream, uint value)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            unchecked
            {
                int i = 1;
                byte b = (byte)(value & 0x7F);
                uint data = value >> 7;
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
        /// Writes the specified value 7 bit encoded to the specified Stream
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write to</param>
        /// <param name="value">The value to write</param>
        /// <returns>Returns the number of bytes written</returns>
        public static int Write7BitEncoded(Stream stream, int value)
        {
            unchecked
            {
                return Write7BitEncoded(stream, (uint)value);
            }
        }

        /// <summary>
        /// Writes the specified value 7 bit encoded to the specified Stream
        /// </summary>
        /// <param name="writer">The <see cref="DataWriter"/> to write to</param>
        /// <param name="value">The value to write</param>
        /// <returns>Returns the number of bytes written</returns>
        public static int Write7BitEncoded(DataWriter writer, uint value)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            unchecked
            {
                int i = 1;
                byte b = (byte)(value & 0x7F);
                uint data = value >> 7;
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
        /// Writes the specified value 7 bit encoded to the specified Stream
        /// </summary>
        /// <param name="writer">The <see cref="DataWriter"/> to write to</param>
        /// <param name="value">The value to write</param>
        /// <returns>Returns the number of bytes written</returns>
        public static int Write7BitEncoded(DataWriter writer, int value)
        {
            unchecked
            {
                return Write7BitEncoded(writer, (uint)value);
            }
        }
    }
}
