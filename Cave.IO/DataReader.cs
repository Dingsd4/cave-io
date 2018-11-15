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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Cave;

namespace Cave.IO
{
	/// <summary>
	/// Provides a new little endian binary reader implementation (a combination of streamreader and binaryreader).
	/// This class is not threadsafe and does not buffer anything nor needs flushing.
	/// You can access the basestream at any time with any mode of operation (read, write, seek, ...).
	/// </summary>
	public sealed class DataReader
    {
        IBitConverter endianDecoder;
        Encoding textDecoder;
		StringEncoding stringEncoding;
		EndianType endianType;
        bool slowTextDecodingWarningSent;
        bool lineFeedTested;
        bool zeroTested;

        #region private string reader implementation for reading strings without buffering
        byte[] ReadCharsUTF8(int charCount)
        {
            int chars = 0;
            List<byte> result = new List<byte>(charCount * 4);
            while (true)
            {
                for (; chars < charCount; chars++)
                {
                    byte b = ReadByte();
                    result.Add(b);

                    #region char reader

                    if (b < 0x80)
                    {
                        continue;
                    }

                    if (b < 0xC2)
                    {
                        throw new InvalidDataException();
                    }

                    if (b >= 0xF5)
                    {
                        throw new InvalidDataException();
                    }

                    // if (b >= 0xC2)
                    {
                        //2nd byte
                        result.Add(ReadByte());
                    }
                    if (b >= 0xE0)
                    {
                        //3rd byte
                        result.Add(ReadByte());
                    }
                    if (b >= 0xF0)
                    {
                        //4th byte
                        result.Add(ReadByte());
                    }
                    
                    #endregion
                }

                //remove bom at beginning
                if (result.Count > 2 && result[0] == 0xEF && result[1] == 0xBB && result[2] == 0xBF)
                {
                    result.RemoveRange(0, 3);
                    chars--;
                    continue;
                }
                break;
            }
            return result.ToArray();
        }

        byte[] ReadCharsUTF16LE(int charCount)
        {
            List<byte> result = new List<byte>(charCount * 4);
            for (int i = 0; i < charCount; i++)
            {
                byte b1 = ReadByte();
                byte b2 = ReadByte();
                result.Add(b1);
                result.Add(b2);
                if ((b2 > 0xD7) && (b2 < 0xDC))
                {
                    //add low surrogate
                    result.Add(ReadByte());
                    result.Add(ReadByte());
                }
            }
            return result.ToArray();
        }

        byte[] ReadCharsUTF16BE(int charCount)
        {
            List<byte> result = new List<byte>(charCount * 4);
            for (int i = 0; i < charCount; i++)
            {
                byte b1 = ReadByte();
                byte b2 = ReadByte();
                result.Add(b1);
                result.Add(b2);
                if ((b1 > 0xD7) && (b1 < 0xDC))
                {
                    //add low surrogate
                    result.Add(ReadByte());
                    result.Add(ReadByte());
                }
            }
            return result.ToArray();
        }

        byte[] ReadCharsUTF32(int charCount)
        {
            return ReadBytes(4 * charCount);
        }
        #endregion

        /// <summary>
		/// Gets / sets the Encoding to use for characters and strings. 
        /// Setting this value directly sets <see cref="StringEncoding"/> to <see cref="StringEncoding.Undefined"/>.
		/// </summary>
        public Encoding Encoding
        {
            get => textDecoder;
            set
            {
                textDecoder = value;
                stringEncoding = StringEncoding.Undefined;
                lineFeedTested = false;
                zeroTested = false;
            }
        }

        /// <summary>
        /// Provides the new line mode used
        /// </summary>
        public NewLineMode NewLineMode { get; set; } 

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
					case EndianType.LittleEndian: endianDecoder = BitConverterLE.Instance; break;
					case EndianType.BigEndian: endianDecoder = BitConverterBE.Instance; break;
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
					case StringEncoding.ASCII: textDecoder = new CheckedASCIIEncoding(); break;
					case StringEncoding.UTF8: textDecoder = Encoding.UTF8; break;
					case StringEncoding.UTF16: textDecoder = Encoding.Unicode; break;
					case StringEncoding.UTF32: textDecoder = Encoding.UTF32; break;
                    default: textDecoder = Encoding.GetEncoding((int)stringEncoding); break;
                }
                lineFeedTested = false;
                zeroTested = false;
            }
		}

        /// <summary>
        /// Provides access to the base stream
        /// </summary>
        public Stream BaseStream { get; private set; }

        /// <summary>Creates a new binary reader using the specified encoding and writing to the specified stream</summary>
        /// <param name="input">The stream to read from</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="endian">The endian type.</param>
        /// <param name="newLineMode">New line mode</param>
        /// <exception cref="ArgumentNullException">output</exception>
        /// <exception cref="ArgumentException">Stream does not support writing or is already closed.;output</exception>
        /// <exception cref="NotSupportedException">StringEncoding {0} not supported!
        /// or EndianType {0} not supported!</exception>
        public DataReader(Stream input, StringEncoding encoding = StringEncoding.UTF8, NewLineMode newLineMode = NewLineMode.LF, EndianType endian = EndianType.LittleEndian)
        {
            BaseStream = input ?? throw new ArgumentNullException("output");
            NewLineMode = newLineMode;
            StringEncoding = encoding != StringEncoding.Undefined ? encoding : throw new ArgumentOutOfRangeException(nameof(encoding));
            EndianType = endian;
            if (!BaseStream.CanRead)
            {
                throw new ArgumentException("Stream does not support reading or is already closed.", nameof(input));
            }
        }

        /// <summary>Creates a new binary reader using the specified encoding and writing to the specified stream</summary>
        /// <param name="input">The stream to read from</param>
        /// <param name="newLineMode">New line mode</param>
        /// <param name="encoding">Encoding to use for characters and strings</param>
        /// <param name="endian">The endian type.</param>
        /// <exception cref="ArgumentNullException">output</exception>
        /// <exception cref="ArgumentException">Stream does not support writing or is already closed.;output</exception>
        /// <exception cref="NotSupportedException">StringEncoding {0} not supported!
        /// or EndianType {0} not supported!</exception>
        public DataReader(Stream input, Encoding encoding, NewLineMode newLineMode = NewLineMode.LF, EndianType endian = EndianType.LittleEndian)
        {
            BaseStream = input ?? throw new ArgumentNullException("output");
            NewLineMode = newLineMode;
            Encoding = encoding ?? throw new ArgumentOutOfRangeException(nameof(encoding));
            EndianType = endian;
            if (!BaseStream.CanRead)
            {
                throw new ArgumentException("Stream does not support reading or is already closed.", nameof(input));
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
        public long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        /// <summary>
        /// Skips some bytes at the base stream
        /// </summary>
        /// <param name="count">Length to skip in bytes</param>
        /// <returns></returns>
        public void Skip(long count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (count == 0)
            {
                return;
            }

            Seek(count, SeekOrigin.Current);
        }

        /// <summary>
        /// Obtains the available bytes for reading.
        /// Attention: the BaseStream has to support the Length and Position properties.
        /// </summary>
        public long Available
        {
            get
            {
                return BaseStream.Length - BaseStream.Position;
            }
        }

        /// <summary>
        /// Writes the specified value directly to the stream
        /// </summary>
        public bool ReadBool()
        {
            return ReadByte() != 0;
        }

        /// <summary>
        /// Writes the specified value directly to the stream
        /// </summary>
        public byte ReadByte()
        {
            int b = BaseStream.ReadByte();
            if (b < 0)
            {
                throw new EndOfStreamException();
            }

            return (byte)b;
        }

        /// <summary>
        /// Reads a byte buffer with length prefix from the stream
        /// </summary>
        /// <exception cref="InvalidDataException"></exception>
        public byte[] ReadBytes()
        {
            int length = Read7BitEncodedInt32();
            if (length < 0)
            {
                if (length == -1)
                {
                    return null;
                }

                throw new InvalidDataException(string.Format("Invalid 7bit encoded value found!"));
            }
            return ReadBytes(length);
        }

        /// <summary>
        /// Reads a buffer from the stream
        /// </summary>
        /// <param name="count"></param>
        public byte[] ReadBytes(int count)
        {
            byte[] result = new byte[count];
            int done = 0;
            while (done < count)
            {
                int read = BaseStream.Read(result, done, count - done);
                if (read == 0)
                {
                    throw new EndOfStreamException();
                }

                done += read;
            }
            return result;
        }

        /// <summary>
        /// Reads a character from the stream
        /// </summary>
        public char ReadChar()
        {
            switch (StringEncoding)
            {
                case StringEncoding.ASCII:
                    byte b = ReadByte();
                    if (b > 127)
                    {
                        throw new InvalidDataException(string.Format("Byte '{0}' is not a valid ASCII character!", b));
                    }

                    return (char)b;
                default:
                    var result = ReadChars(1);
                    if (result.Length > 1) throw new InvalidDataException("Decoded characters do not match single character read!");
                    return result[0];
            }
        }

        /// <summary>
        /// Reads characters from the stream
        /// </summary>
        /// <param name="count">Number of characters (not bytes) to read</param>
        public char[] ReadChars(int count)
        {
            if (textDecoder.IsSingleByte)
            {
                return textDecoder.GetChars(ReadBytes(count));
            }
            switch (StringEncoding)
            {
                case StringEncoding.UTF_8:
                case StringEncoding.UTF8: return textDecoder.GetChars(ReadCharsUTF8(count));

                case StringEncoding.UTF_16:
                case StringEncoding.UTF16: return textDecoder.GetChars(ReadCharsUTF16LE(count));

                case StringEncoding.UTF_16BE: return textDecoder.GetChars(ReadCharsUTF16BE(count));

                case StringEncoding.UTF_32:
                case StringEncoding.UTF32: return textDecoder.GetChars(ReadCharsUTF32(count));

                default:
                    if (!slowTextDecodingWarningSent)
                    {
                        Trace.TraceWarning("Using slow text decoding.");
                        slowTextDecodingWarningSent = true;
                    }
                    break;
			}

            var buf = ReadBytes(count);
            var dec = textDecoder.GetDecoder();
            var result = new char[count];
            dec.Convert(buf, 0, count, result, 0, count, false, out int bytesUsed, out int resultCount, out bool complete);
            while (!complete || resultCount < count)
            {
                int charCountLeft = count - resultCount;
                buf = ReadBytes(charCountLeft);
                dec.Convert(buf, 0, charCountLeft, result, resultCount, charCountLeft, false, out bytesUsed, out int charsUsed, out complete);
                resultCount += charsUsed;
            }
            return result;
        }

        /// <summary>
        /// Reads a value from the stream
        /// </summary>
        public decimal ReadDecimal()
        {
            int[] bits = new int[4];
            for (int i = 0; i < 4; i++)
            {
                bits[i] = ReadInt32();
            }

            return new decimal(bits);
        }

        /// <summary>
        /// Reads a value from the stream
        /// </summary>
        public double ReadDouble()
        {
            byte[] bytes = ReadBytes(8);
            return endianDecoder.ToDouble(bytes, 0);
        }

        /// <summary>
        /// Writes the specified value directly to the stream
        /// </summary>
        public float ReadSingle()
        {
            byte[] bytes = ReadBytes(4);
            return endianDecoder.ToSingle(bytes, 0);
        }

        /// <summary>
        /// Reads a value directly from the stream
        /// </summary>
        public sbyte ReadInt8()
        {
            unchecked { return (sbyte)ReadByte(); }
        }

        /// <summary>
        /// Reads a value directly from the stream
        /// </summary>
        public short ReadInt16()
        {
            byte[] bytes = ReadBytes(2);
            return endianDecoder.ToInt16(bytes, 0);
        }

        /// <summary>
        /// Reads a value directly from the stream
        /// </summary>
        public int ReadInt32()
        {
            byte[] bytes = ReadBytes(4);
            return endianDecoder.ToInt32(bytes, 0);
        }

        /// <summary>
        /// Reads a value directly from the stream
        /// </summary>
        public long ReadInt64()
        {
            byte[] bytes = ReadBytes(8);
            return endianDecoder.ToInt64(bytes, 0);
        }

        /// <summary>
        /// Reads a value directly from the stream
        /// </summary>
        public byte ReadUInt8()
        {
            return ReadByte();
        }

        /// <summary>
        /// Reads a value directly from the stream
        /// </summary>
        public ushort ReadUInt16()
        {
            byte[] bytes = ReadBytes(2);
            return endianDecoder.ToUInt16(bytes, 0);
        }

        /// <summary>
        /// Reads a value directly from the stream
        /// </summary>
        public uint ReadUInt32()
        {
            byte[] bytes = ReadBytes(4);
            return endianDecoder.ToUInt32(bytes, 0);
        }

        /// <summary>
        /// Reads a value directly from the stream
        /// </summary>
        public ulong ReadUInt64()
        {
            byte[] bytes = ReadBytes(8);
            return endianDecoder.ToUInt64(bytes, 0);
        }

        /// <summary>
        /// Reads a string ending with [CR]LF from the stream
        /// </summary>
        public string ReadLine()
        {
            if (!lineFeedTested)
            {
                if ("\r\n" != textDecoder.GetString(textDecoder.GetBytes("\r\n")))
                {
                    throw new InvalidOperationException($"Encoding {textDecoder.EncodingName} does not support WriteLine/ReadLine!");
                }
                lineFeedTested = true;
            }
            List<char> result = new List<char>();
            bool completed = false;
            bool waitLF = false;
            while (!completed)
            {
                var chars = ReadChars(1);
                for (int i = 0; i < chars.Length; i++)
                {
                    if (completed)
                    {
                        throw new InvalidDataException("Decoding got additional chars after newline!");
                    }
                    char c = chars[i];
                    switch (c)
                    {
                        case '\r':
                            switch (NewLineMode)
                            {
                                case NewLineMode.CR: completed = true; continue;
                                // treat carriage return as regular char if NewLineMode is line feed
                                case NewLineMode.LF: break;
                                case NewLineMode.CRLF:
                                    waitLF = true;
                                    continue;
                                default: throw new NotImplementedException("NewLineMode not implemented: " + NewLineMode.ToString());
                            }
                            break;
                        case '\n':
                            switch (NewLineMode)
                            {
                                case NewLineMode.LF: completed = true; continue;
                                // treat line feed as regular char if NewLineMode is carriage return
                                case NewLineMode.CR: break;
                                case NewLineMode.CRLF:
                                    if (waitLF)
                                    {
                                        completed = true;
                                        continue;
                                    }
                                    break;
                                default: throw new NotImplementedException("NewLineMode not implemented: " + NewLineMode.ToString());
                            }
                            break;
                    }
                    result.Add(c);
                    if (result.Count > 65536)
                    {
                        throw new InvalidDataException("Refusing to read more than 64k characters at ReadLine()!");
                    }
                }
            }
            return new string(result.ToArray());
        }

        /// <summary>
        /// Reads bytes from the stream until one of the specified end markers are found or max count is reached.
        /// </summary>
        public void ReadUntil(byte[] data, ref int offset, bool removeMarker, params byte[] endMark)
        {
            int maxCount = data.Length;
            bool completed = false;
            int endMarkLast = endMark.Length - 1;
            while (!completed)
            {
                if (offset > data.Length)
                {
                    throw new InvalidDataException($"Refusing to read more than {maxCount} bytes at ReadUntil()!");
                }

                var b = data[offset] = ReadByte();
                if (offset >= endMark.Length && b == endMark[endMarkLast])
                {
                    completed = true;
                    int i = endMarkLast - 1;
                    int n = offset - 1;
                    while (i >= 0)
                    {
                        if (data[n--] != endMark[i--]) { completed = false; break; }
                    }
                }
                offset += 1;
            }
            if (removeMarker)
            {
                offset -= endMark.Length;
            }
        }

        /// <summary>
        /// Reads a string of the specified byte count from the stream
        /// </summary>
        public string ReadString(int count)
        {
            return textDecoder.GetString(ReadBytes(count));
        }

        /// <summary>
        /// Reads a string with length prefix from the stream
        /// </summary>
        /// <exception cref="InvalidDataException"></exception>
        public string ReadString()
        {
            int length = Read7BitEncodedInt32();
            if (length < 0)
            {
                if (length == -1)
                {
                    return null;
                }

                throw new InvalidDataException(string.Format("Invalid 7bit encoded value found!"));
            }
            return ReadString(length);
        }

        /// <summary>
        /// Reads a zero terminated string from the stream
        /// </summary>
        public string ReadZeroTerminatedString(int maximumBytes)
        {
            if (!zeroTested)
            {
                if ("\0" != textDecoder.GetString(textDecoder.GetBytes("\0")))
                {
                    throw new InvalidOperationException($"Encoding {textDecoder.EncodingName} does not support zero termination!");
                }
                zeroTested = true;
            }

            var termination = textDecoder.GetBytes("\0");
            var buf = new byte[maximumBytes];
            int offset = 0;
            while (true)
            {
                ReadUntil(buf, ref offset, false, termination);
                var s = textDecoder.GetString(buf, 0, offset);
                int i = s.IndexOf('\0');
                if (i > -1) return s.Substring(0, i);
            }
        }

        /// <summary>
        /// Reads a zero terminated string from the stream
        /// </summary>
        /// <param name="byteCount">Fieldlength in bytes</param>
        public string ReadZeroTerminatedFixedLengthString(int byteCount)
        {
            string result = ReadString(byteCount);
            int i = result.IndexOf((char)0);
            if (i > -1)
            {
                result = result.Substring(0, i);
            }

            return result;
        }

        /// <summary>
        /// Reads a value from the stream
        /// </summary>
        public TimeSpan ReadTimeSpan()
        {
            return new TimeSpan(ReadInt64());
        }

        /// <summary>
        /// Reads a DateTime value from the stream with <see cref="DateTimeKind"/>
        /// </summary>
        public DateTime ReadDateTime()
        {
            DateTimeKind kind = (DateTimeKind)Read7BitEncodedInt32();
            switch(kind)
            {
                case DateTimeKind.Local:
                case DateTimeKind.Unspecified:
                case DateTimeKind.Utc:
                    break;
                default: throw new InvalidDataException("Invalid DateTimeKind!");
            }
            return new DateTime(ReadInt64(), kind);
        }

        /// <summary>
        /// Reads a 32bit linux epoch value
        /// </summary>
        /// <returns></returns>
        public DateTime ReadEpoch32()
        {
            return new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(ReadUInt32());
        }

        /// <summary>
        /// Reads a 64bit linux epoch value
        /// </summary>
        /// <returns></returns>
        public DateTime ReadEpoch64()
        {
            return new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(ReadUInt64());
        }

        /// <summary>
        /// Reads the specified struct from the stream using the default marshaller
        /// </summary>
        public T ReadStruct<T>() where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = ReadBytes(size);
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            T result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return result;
        }

        /// <summary>
        /// Reads a 7 bit encoded 32 bit value from the stream
        /// </summary>
        public int Read7BitEncodedInt32()
        {
            return BitCoder32.Read7BitEncodedInt32(BaseStream);
        }

        /// <summary>
        /// Reads a 7 bit encoded 32 bit value from the stream
        /// </summary>
        public uint Read7BitEncodedUInt32()
        {
            return BitCoder32.Read7BitEncodedUInt32(BaseStream);
        }

        /// <summary>
        /// Reads a 7 bit encoded 64 bit value from the stream
        /// </summary>
        public long Read7BitEncodedInt64()
        {
            return BitCoder64.Read7BitEncodedInt64(BaseStream);
        }

        /// <summary>
        /// Reads a 7 bit encoded 64 bit value from the stream
        /// </summary>
        public ulong Read7BitEncodedUInt64()
        {
            return BitCoder64.Read7BitEncodedUInt64(BaseStream);
        }

        /// <summary>
        /// Reads an array of the specified struct type from the stream using the default marshaller
        /// </summary>
        /// <typeparam name="T">Type of each element</typeparam>
        public T[] ReadArray<T>() where T : struct
        {
            int count = Read7BitEncodedInt32();
            if (count < 0)
            {
                throw new InvalidDataException("Invalid length prefix while reading array!");
            }

            if (count == 0)
            {
                return new T[0];
            }

            int byteCount = Read7BitEncodedInt32();
            byte[] bytes = ReadBytes(byteCount);
            T[] result;
            if (typeof(T) == typeof(byte))
            {
                result = bytes as T[];
                if (result == null)
                {
                    throw new PlatformNotSupportedException("Byte array conversion bug! Please update your mono framework!");
                }
            }
            else
            {
                result = new T[count];
                Buffer.BlockCopy(bytes, 0, result, 0, byteCount);
            }
            return result;
        }

        /// <summary>Closes the reade and the stream.</summary>
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
