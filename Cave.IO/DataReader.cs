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

		#region private string reader implementation for reading strings without buffering
		byte[] m_ReadUTF8(int charCount)
        {
            int readBOM = 0;
            List<byte> result = new List<byte>(charCount * 4);
            for (int i = 0; i < charCount; i++)
            {
                byte b = ReadByte();
                result.Add(b);
                #region BOM reader
                /* UTF8 BOM Quick Fix only for single chars.
                 * TODO: find solution for BOM detection
                 */
                if (charCount > 2)
                {
                    if (readBOM > 0)
                    {
                        if (readBOM == 1)
                        {
                            if (b != 0xBB)
                            {
                                throw new InvalidDataException("Invalid BOM at index 1");
                            }
                            readBOM++;
                        }
                        else if (readBOM == 2)
                        {
                            if (b != 0xBF)
                            {
                                throw new InvalidDataException("Invalid BOM at index 2");
                            }
                            readBOM = 0;
                        }
                        continue;
                    }
                    if (b == 0xEF)
                    {
                        readBOM = 1;
                        continue;
                    }
                }
                #endregion
                #region char reader
                if (b < 0x80) continue;
                if (b < 0xC2)
                {
                    throw new InvalidDataException();
                }
                if (b >= 0xC2)
                {
                    //2 byte char
                    result.Add(ReadByte());
                }
                if (b >= 0xE0)
                {
                    //3 byte char
                    result.Add(ReadByte());
                }
                if (b >= 0xF0)
                {
                    //4 byte char
                    result.Add(ReadByte());
                }
                if (b >= 0xF5)
                {
                    throw new InvalidDataException();
                }
                #endregion
            }
            return result.ToArray();
        }

        byte[] m_ReadUTF16(int charCount)
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

        byte[] m_ReadUTF32(int charCount)
        {
            return ReadBytes(4 * charCount);
        }
        #endregion

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
					case StringEncoding.ASCII: textDecoder = new CheckedASCIIEncoding(); break;
					case StringEncoding.UTF8: textDecoder = Encoding.UTF8; break;
					case StringEncoding.UTF16: textDecoder = Encoding.Unicode; break;
					case StringEncoding.UTF32: textDecoder = Encoding.UTF32; break;
                    default: textDecoder = Encoding.GetEncoding(stringEncoding.ToString().Replace('_', '-')); break;
				}
			}
		}

        /// <summary>
        /// Provides access to the base stream
        /// </summary>
        public Stream BaseStream { get; private set; }

        /// <summary>Creates a new binary writer using the specified encoding and writing to the specified stream</summary>
        /// <param name="input">The stream to read from</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="endian">The endian type.</param>
        /// <param name="newLineMode">Sets the newline mode used</param>
        public DataReader(Stream input, StringEncoding encoding, EndianType endian = EndianType.LittleEndian, NewLineMode newLineMode = NewLineMode.LF)
            : this(input, newLineMode, encoding, endian)
        {
        }

        /// <summary>Creates a new binary writer using the specified encoding and writing to the specified stream</summary>
        /// <param name="input">The stream to read from</param>
        /// <param name="endian">The endian type.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="newLineMode">Sets the newline mode used</param>
        public DataReader(Stream input, EndianType endian, StringEncoding encoding = StringEncoding.UTF8, NewLineMode newLineMode = NewLineMode.LF)
            : this(input, newLineMode, encoding, endian)
        {
        }

        /// <summary>Creates a new binary reader using the specified encoding and writing to the specified stream</summary>
        /// <param name="input">The stream to read from</param>
        /// <param name="newLineMode">Sets the newline mode used</param>
        /// <param name="stringEncoding">Encoding to use for characters and strings</param>
        /// <param name="endian">The endian type.</param>
        /// <exception cref="ArgumentNullException">input</exception>
        /// <exception cref="ArgumentException">Stream does not support reading or is already closed.;input</exception>
        /// <exception cref="NotSupportedException">StringEncoding {0} not supported!
        /// or EndianType {0} not supported!</exception>
        public DataReader(Stream input, NewLineMode newLineMode = NewLineMode.LF, StringEncoding stringEncoding = StringEncoding.UTF8, EndianType endian = EndianType.LittleEndian)
        {
            if (input == null) throw new ArgumentNullException("input");
            if (!input.CanRead) throw new ArgumentException("Stream does not support reading or is already closed.", "input");
            BaseStream = input;
            NewLineMode = newLineMode;
            StringEncoding = stringEncoding;
			EndianType = endian;
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
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (count == 0) return;
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
            if (b < 0) throw new EndOfStreamException();
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
                if (length == -1) return null;
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
                if (read == 0) throw new EndOfStreamException();
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
                    if (b > 127) throw new InvalidDataException(string.Format("Byte '{0}' is not a valid ASCII character!", b));
                    return (char)b;
                default: return ReadChars(1)[0];
            }
        }

        /// <summary>
        /// Reads characters from the stream
        /// </summary>
        /// <param name="count">Number of characters (not bytes) to read</param>
        public char[] ReadChars(int count)
        {
            switch (StringEncoding)
            {
                case StringEncoding.UTF8: return textDecoder.GetChars(m_ReadUTF8(count));
                case StringEncoding.UTF16: return textDecoder.GetChars(m_ReadUTF16(count));
                case StringEncoding.UTF32: return textDecoder.GetChars(m_ReadUTF32(count));
				default: return textDecoder.GetChars(ReadBytes(count));
			}
        }

        /// <summary>
        /// Reads a value from the stream
        /// </summary>
        public decimal ReadDecimal()
        {
            int[] bits = new int[4];
            for (int i = 0; i < 4; i++) bits[i] = ReadInt32();
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
            List<char> result = new List<char>();
            bool completed = false;
            while (!completed)
            {
                char c = ReadChar();
                switch (c)
                {
                    case '\r':
                        switch (NewLineMode)
                        {
                            case NewLineMode.CR: completed = true; continue;
                            // treat carriage return as regular char if NewLineMode is line feed
                            case NewLineMode.LF: break;
                            case NewLineMode.CRLF:
                                char c2 = ReadChar();
                                if (c2 == '\n') { completed = true; continue; }
                                result.Add(c);
                                result.Add(c2);
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
                            case NewLineMode.CRLF: break;
                            default: throw new NotImplementedException("NewLineMode not implemented: " + NewLineMode.ToString());
                        }
                        break;
                }
                result.Add(c);
                if (result.Count > 65536) throw new InvalidDataException("Refusing to read more than 64k characters at ReadLine()!");
            }
            return new string(result.ToArray());
        }

        /// <summary>
        /// Reads bytes ending with [CR]LF from the stream
        /// </summary>
        public byte[] ReadUntil(int maxCount, params byte[] endMark)
        {
            List<byte> result = new List<byte>();
            bool completed = false;
            int endMarkLast = endMark.Length - 1;
            while (!completed)
            {
                byte b = ReadByte();
                result.Add(b);
                if (maxCount > 0 && result.Count > maxCount) throw new InvalidDataException("Refusing to read more than " + maxCount + " bytes at ReadUntil()!");

                if (result.Count >= endMark.Length && b == endMark[endMarkLast])
                {
                    completed = true;
                    int i = endMarkLast;
                    int n = result.Count - 1;
                    while (i >= 0)
                    {
                        if (result[n--] != endMark[i--]) { completed = false; break; }
                    }
                }
            }
            result.RemoveRange(result.Count - endMark.Length, endMark.Length);
            return result.ToArray();
        }

        /// <summary>
        /// Reads a string of the specified byte count from the stream
        /// </summary>
        public string ReadString(int count)
        {
            switch (StringEncoding)
            {
                case StringEncoding.UTF8: return Encoding.UTF8.GetString(ReadBytes(count));
                case StringEncoding.UTF16: return Encoding.Unicode.GetString(ReadBytes(count));
                case StringEncoding.UTF32: return Encoding.UTF32.GetString(ReadBytes(count));
				default: return textDecoder.GetString(ReadBytes(count));
			}
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
                if (length == -1) return null;
                throw new InvalidDataException(string.Format("Invalid 7bit encoded value found!"));
            }
            return ReadString(length);
        }

        /// <summary>
        /// Reads a zero terminated string from the stream
        /// </summary>
        public string ReadZeroTerminatedString()
        {
            List<char> result = new List<char>();
            while (true)
            {
                char c = ReadChar();
                if (c == 0) break;
                result.Add(c);
            }
            return new string(result.ToArray());
        }

        /// <summary>
        /// Reads a zero terminated string from the stream
        /// </summary>
        /// <param name="byteCount">Fieldlength in bytes</param>
        public string ReadZeroTerminatedString(int byteCount)
        {
            string result = ReadString(byteCount);
            int i = result.IndexOf((char)0);
            if (i > -1) result = result.Substring(0, i);
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
            if (count < 0) throw new InvalidDataException("Invalid length prefix while reading array!");
            if (count == 0) return new T[0];

            int byteCount = Read7BitEncodedInt32();
            byte[] bytes = ReadBytes(byteCount);
            T[] result;
            if (typeof(T) == typeof(byte))
            {
                result = bytes as T[];
                if (result == null) throw new PlatformNotSupportedException("Byte array conversion bug! Please update your mono framework!");
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
