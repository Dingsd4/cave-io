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
#endregion Authors & Contributors

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Cave.IO
{
	/// <summary>
	/// Provides a new little endian binary writer implementation (a combination of streamwriter and binarywriter).
	/// This class is not threadsafe and does not buffer anything nor needs flushing.
	/// You can access the basestream at any time with any mode of operation (read, write, seek, ...).
	/// </summary>
	public sealed class DataWriter
    {
        Encoding textEncoder;
        IBitConverter endianEncoder;
		StringEncoding stringEncoding;
		EndianType endianType;
        bool lineFeedTested;
        bool zeroTested;

        /// <summary>
		/// Gets / sets the Encoding to use for characters and strings. 
        /// Setting this value directly sets <see cref="StringEncoding"/> to <see cref="StringEncoding.Undefined"/>.
		/// </summary>
        public Encoding Encoding
        {
            get => textEncoder;
            set
            {
                textEncoder = value;
                stringEncoding = StringEncoding.Undefined;
                lineFeedTested = false;
                zeroTested = false;
            }
        }

		/// <summary>Gets the endian encoder type.</summary>
		/// <value>The endian encoder type.</value>
		public EndianType EndianType
		{
			get => endianType;
            set
			{
				endianType = value;
				switch (endianType)
				{
					case EndianType.LittleEndian: endianEncoder = BitConverterLE.Instance; break;
					case EndianType.BigEndian: endianEncoder = BitConverterBE.Instance; break;
					default: throw new NotSupportedException(string.Format("EndianType {0} not supported!", endianType));
				}
			}
		}

		/// <summary>
		/// Encoding to use for characters and strings
		/// </summary>
		public StringEncoding StringEncoding
		{
			get => stringEncoding;
			set
			{
				stringEncoding = value;
				switch (stringEncoding)
				{
                    case StringEncoding.Undefined: break;
                    case StringEncoding.ASCII: textEncoder = new CheckedASCIIEncoding(); break;
					case StringEncoding.UTF8: textEncoder = Encoding.UTF8; break;
					case StringEncoding.UTF16: textEncoder = Encoding.Unicode; break;
					case StringEncoding.UTF32: textEncoder = Encoding.UTF32; break;
                    default: textEncoder = Encoding.GetEncoding((int)stringEncoding); break;
				}
                lineFeedTested = false;
                zeroTested = false;
            }
		}

        /// <summary>
        /// Provides the new line mode used
        /// </summary>
        public NewLineMode NewLineMode { get; set; }

        /// <summary>
        /// Provides access to the base stream
        /// </summary>
        public Stream BaseStream { get; private set; }

        /// <summary>Creates a new binary writer using the specified encoding and writing to the specified stream</summary>
        /// <param name="output">The stream to write to</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="endian">The endian type.</param>
        /// <param name="newLineMode">New line mode</param>
        /// <exception cref="ArgumentNullException">output</exception>
        /// <exception cref="ArgumentException">Stream does not support writing or is already closed.;output</exception>
        /// <exception cref="NotSupportedException">StringEncoding {0} not supported!
        /// or EndianType {0} not supported!</exception>
        public DataWriter(Stream output, StringEncoding encoding = StringEncoding.UTF8, NewLineMode newLineMode = NewLineMode.LF, EndianType endian = EndianType.LittleEndian)
        {
            BaseStream = output ?? throw new ArgumentNullException(nameof(output));
            NewLineMode = newLineMode;
            StringEncoding = encoding != StringEncoding.Undefined ? encoding : throw new ArgumentOutOfRangeException(nameof(encoding));
            EndianType = endian;
            if (!BaseStream.CanWrite)
            {
                throw new ArgumentException("Stream does not support writing or is already closed.", nameof(output));
            }
        }

        /// <summary>Creates a new binary writer using the specified encoding and writing to the specified stream</summary>
        /// <param name="output">The stream to write to</param>
        /// <param name="newLineMode">New line mode</param>
        /// <param name="encoding">Encoding to use for characters and strings</param>
        /// <param name="endian">The endian type.</param>
        /// <exception cref="ArgumentNullException">output</exception>
        /// <exception cref="ArgumentException">Stream does not support writing or is already closed.;output</exception>
        /// <exception cref="NotSupportedException">StringEncoding {0} not supported!
        /// or EndianType {0} not supported!</exception>
        public DataWriter(Stream output, Encoding encoding, NewLineMode newLineMode = NewLineMode.LF, EndianType endian = EndianType.LittleEndian)
        {
            BaseStream = output ?? throw new ArgumentNullException(nameof(output));
            NewLineMode = newLineMode;
            Encoding = encoding ?? throw new ArgumentOutOfRangeException(nameof(encoding));
            EndianType = endian;
            if (!BaseStream.CanWrite)
            {
                throw new ArgumentException("Stream does not support writing or is already closed.", nameof(output));
            }
        }

        /// <summary>
        /// Flushes the stream
        /// </summary>
        public void Flush()
        {
            BaseStream.Flush();
        }

        /// <summary>
        /// Seeks at the base stream (this requires the stream to be seekable)
        /// </summary>
        /// <param name="offset">Offset to seek to</param>
        /// <param name="origin">Origin to seek from</param>
        /// <returns></returns>
        public long Seek(int offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        /// <summary>
        /// Writes the specified value directly to the stream
        /// </summary>
        /// <param name="value"></param>
        public void Write(bool value)
        {
            BaseStream.WriteByte(value ? (byte)1 : (byte)0);
        }

        /// <summary>
        /// Writes the specified value directly to the stream
        /// </summary>
        /// <param name="value"></param>
        public void Write(byte value)
        {
            BaseStream.WriteByte(value);
        }

        /// <summary>
        /// Writes the specified buffer directly to the stream
        /// </summary>
        /// <param name="buffer"></param>
        public void Write(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            BaseStream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Writes the specified buffer to the stream with length prefix
        /// </summary>
        /// <param name="buffer"></param>
        public void WritePrefixed(byte[] buffer)
        {
            if (buffer == null)
            {
                Write7BitEncoded32(-1);
            }
            else
            {
                Write7BitEncoded32(buffer.Length);
                BaseStream.Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// Writes a part of the specified buffer directly to the stream
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public void Write(byte[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            BaseStream.Write(buffer, index, count);
        }

        /// <summary>
        /// Writes a part of the specified buffer to the stream with length prefix
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public void WritePrefixed(byte[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            Write7BitEncoded32(count);
            BaseStream.Write(buffer, index, count);
        }

        /// <summary>
        /// Writes the specified character directly to the stream
        /// </summary>
        /// <param name="c"></param>
        public int Write(char c)
        {
            return Write(new char[] { c });
        }

        /// <summary>
        /// Writes the specified characters directly to the stream
        /// </summary>
        /// <param name="chars"></param>
        public int Write(char[] chars)
        {
            if (chars == null)
            {
                throw new ArgumentNullException("chars");
            }

            byte[] data = textEncoder.GetBytes(chars);
            Write(data);
            return data.Length;
        }

        /// <summary>
        /// Writes a part of the specified character array directly to the stream
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public int Write(char[] chars, int index, int count)
        {
            if (chars == null)
            {
                throw new ArgumentNullException("chars");
            }

            byte[] data = textEncoder.GetBytes(chars, index, count);
            Write(data);
            return data.Length;
        }

        /// <summary>
        /// Writes the specified value directly to the stream
        /// </summary>
        /// <param name="value"></param>
        public void Write(decimal value)
        {
            foreach (int decimalData in decimal.GetBits(value))
            {
                Write(decimalData);
            }
        }

        /// <summary>
        /// Writes the specified value directly to the stream
        /// </summary>
        /// <param name="value"></param>
        public void Write(double value)
        {
            Write(endianEncoder.GetBytes(value));
        }

        /// <summary>
        /// Writes the specified value directly to the stream
        /// </summary>
        /// <param name="value"></param>
        public void Write(short value)
        {
            Write(endianEncoder.GetBytes(value));
        }

        /// <summary>
        /// Writes the specified value directly to the stream
        /// </summary>
        /// <param name="value"></param>
        public void Write(int value)
        {
            Write(endianEncoder.GetBytes(value));
        }

        /// <summary>
        /// Writes the specified value directly to the stream
        /// </summary>
        /// <param name="value"></param>
        public void Write(long value)
        {
            Write(endianEncoder.GetBytes(value));
        }

        /// <summary>
        /// Writes the specified value directly to the stream
        /// </summary>
        /// <param name="value"></param>
        public void Write(sbyte value)
        {
            //Bugfix: BitConverter.GetBytes(sbyte) returns 2 bytes...
            Write(unchecked((byte)value));
        }

        /// <summary>
        /// Writes the specified value directly to the stream
        /// </summary>
        /// <param name="value"></param>
        public void Write(float value)
        {
            Write(endianEncoder.GetBytes(value));
        }

        /// <summary>
        /// Writes the specified string directly to the stream
        /// </summary>
        /// <param name="text">String to write</param>
        public int Write(string text)
        {
            byte[] data = textEncoder.GetBytes(text);
            Write(data);
            return data.Length;
        }

        /// <summary>
        /// Writes the specified string (with/out length prefix) directly to the stream
        /// </summary>
        /// <param name="text">String to write</param>
        public int WritePrefixed(string text)
        {
            if (text == null)
            {
                return Write7BitEncoded32(-1);
            }
            byte[] data = textEncoder.GetBytes(text);
            int prefix = Write7BitEncoded32(data.Length);
            Write(data);
            return prefix + data.Length;
        }

        /// <summary>
        /// Writes the "new line" marking to the stream. This depends on the chosen <see cref="NewLineMode"/>.
        /// </summary>
        public int WriteLine()
        {
            if (!lineFeedTested)
            {
                if ("\r\n" != textEncoder.GetString(textEncoder.GetBytes("\r\n")))
                {
                    throw new InvalidOperationException($"Encoding {textEncoder.EncodingName} does not support WriteLine/ReadLine!");
                }
                lineFeedTested = true;
            }
            switch (NewLineMode)
            {
                case NewLineMode.CR: return Write('\r');
                case NewLineMode.LF: return Write('\n');
                case NewLineMode.CRLF: return Write(new char[] { '\r', '\n' });
                default: throw new NotImplementedException(string.Format("NewLineMode {0} undefined!", NewLineMode));
            }
        }

        /// <summary>
        /// Writes the specified string followed by a "new line" marking to the stream. This depends on the chosen <see cref="NewLineMode"/>.
        /// </summary>
        /// <param name="text"></param>
        public int WriteLine(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            byte[] data = textEncoder.GetBytes(text);
            Write(data);
            return data.Length + WriteLine();
        }

        /// <summary>
        /// Writes the specified string zero terminated directly to the stream
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fieldLength">Fixed field length to use (1..x)</param>
        public int WriteZeroTerminated(string text, int fieldLength = 0)
        {
            if (!zeroTested)
            {
                if ("\0" != textEncoder.GetString(textEncoder.GetBytes("\0")))
                {
                    throw new InvalidOperationException($"Encoding {textEncoder.EncodingName} does not support zero termination!");
                }
                zeroTested = true;
            }
            byte[] data = textEncoder.GetBytes(text + "\0");
            if (fieldLength > 0 && data.Length > fieldLength)
            {
                data[fieldLength - 1] = 0;
                Write(data, 0, fieldLength);
            }
            else
            {
                Write(data);
                if (fieldLength > 0)
                {
                    int zeroBytes = fieldLength - data.Length;
                    if (zeroBytes > 0)
                    {
                        Write(new byte[zeroBytes]);
                    }
                }
            }
            return Math.Min(data.Length, fieldLength);
        }

        /// <summary>
        /// Writes the specified value directly to the stream
        /// </summary>
        /// <param name="value"></param>
        public void Write(ushort value)
        {
            Write(endianEncoder.GetBytes(value));
        }

        /// <summary>
        /// Writes the specified value directly to the stream
        /// </summary>
        /// <param name="value"></param>
        public void Write(uint value)
        {
            Write(endianEncoder.GetBytes(value));
        }

        /// <summary>
        /// Writes the specified value directly to the stream
        /// </summary>
        /// <param name="value"></param>
        public void Write(ulong value)
        {
            Write(endianEncoder.GetBytes(value));
        }

        /// <summary>
        /// Writes the specified value directly to the stream
        /// </summary>
        /// <param name="value"></param>
        public void Write(TimeSpan value)
        {
            Write(value.Ticks);
        }

        /// <summary>
        /// Writes the specified datetime value with <see cref="DateTimeKind"/>
        /// </summary>
        /// <param name="value"></param>
        public void Write(DateTime value)
        {
            Write7BitEncoded32((int)value.Kind);
            Write(value.Ticks);
        }

        /// <summary>
        /// Writes a 32bit linux epoch value
        /// </summary>
        /// <param name="value"></param>
        public void WriteEpoch32(DateTime value)
        {
            Write((uint)(value - new DateTime(1970, 1, 1)).TotalSeconds);
        }

        /// <summary>
        /// Writes a 64bit linux epoch value
        /// </summary>
        /// <param name="value"></param>
        public void WriteEpoch64(DateTime value)
        {
            Write((ulong)(value - new DateTime(1970, 1, 1)).TotalSeconds);
        }

        /// <summary>
        /// Writes the specified struct directly to the stream using the default marshaller
        /// </summary>
        /// <param name="item"></param>
        public int WriteStruct<T>(T item) where T : struct
        {
            int len = Marshal.SizeOf(item);
            byte[] data = new byte[len];
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            Marshal.StructureToPtr(item, handle.AddrOfPinnedObject(), false);
            handle.Free();
            Write(data);
            return len;
        }

        /// <summary>
        /// Writes the specified 64 bit value to the stream 7 bit encoded (1-10 bytes)
        /// </summary>
        /// <param name="value"></param>
        public int Write7BitEncoded64(long value)
        {
            return BitCoder64.Write7BitEncoded(this, value);
        }

        /// <summary>
        /// Writes the specified 64 bit value to the stream 7 bit encoded (1-10 bytes)
        /// </summary>
        /// <param name="value"></param>
        public int Write7BitEncoded64(ulong value)
        {
            return BitCoder64.Write7BitEncoded(this, value);
        }

        /// <summary>
        /// Writes the specified 32 bit value to the stream 7 bit encoded (1-5 bytes)
        /// </summary>
        /// <param name="value"></param>
        public int Write7BitEncoded32(int value)
        {
            return BitCoder32.Write7BitEncoded(this, value);
        }

        /// <summary>
        /// Writes the specified 32 bit value to the stream 7 bit encoded (1-5 bytes)
        /// </summary>
        /// <param name="value"></param>
        public int Write7BitEncoded32(uint value)
        {
            return BitCoder32.Write7BitEncoded(this, value);
        }

        /// <summary>
        /// Writes an array of the specified struct type to the stream using the default marshaller prefixed by array length
        /// </summary>
        /// <typeparam name="T">Type of each element</typeparam>
        /// <param name="array">Array of elements</param>
        public int WriteArray<T>(T[] array) where T : struct
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (array.Length == 0)
            {
                Write7BitEncoded32(0);
                return 1;
            }
            Write7BitEncoded32(array.Length);
            byte[] bytes;
            if (array is byte[])
            {
                bytes = array as byte[];
                if (bytes == null)
                {
                    throw new PlatformNotSupportedException("Byte array conversion bug! Please update your mono framework!");
                }
            }
            else
            {
                bytes = new byte[array.Length * Marshal.SizeOf(array[0])];
                Buffer.BlockCopy(array, 0, bytes, 0, bytes.Length);
            }
            int headerSize = Write7BitEncoded32(bytes.Length);
            Write(bytes);
            return headerSize + bytes.Length;
        }

        /// <summary>Closes the writer and the stream.</summary>
        public void Close()
        {
            if (BaseStream != null)
            {
                BaseStream.Close();
                BaseStream = null;
            }
        }
    }
}
