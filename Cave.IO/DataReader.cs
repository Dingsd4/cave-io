using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cave.IO
{
    /// <summary>
    ///     Provides a new little endian binary reader implementation (a combination of streamreader and binaryreader).
    ///     This class is not threadsafe and does not buffer anything nor needs flushing. You can access the basestream at any
    ///     time with any mode of operation (read, write, seek, ...).
    /// </summary>
    public sealed class DataReader
    {
        IBitConverter endianDecoder;
        EndianType endianType;
        byte[] newLineBytes;
        string newLineChars;
        Encoding textDecoder;
        byte[] zeroBytes;

        /// <summary>Initializes a new instance of the <see cref="DataReader" /> class.</summary>
        /// <param name="input">The stream to read from.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="endian">The endian type.</param>
        /// <param name="newLineMode">New line mode.</param>
        /// <exception cref="ArgumentNullException">output.</exception>
        /// <exception cref="ArgumentException">Stream does not support writing or is already closed.;output.</exception>
        /// <exception cref="NotSupportedException">StringEncoding {0} not supported! or EndianType {0} not supported!.</exception>
        public DataReader(Stream input, StringEncoding encoding = StringEncoding.UTF8, NewLineMode newLineMode = NewLineMode.LF,
            EndianType endian = EndianType.LittleEndian)
        {
            BaseStream = input ?? throw new ArgumentNullException(nameof(input));
            NewLineMode = newLineMode;
            StringEncoding = encoding != StringEncoding.Undefined ? encoding : throw new ArgumentOutOfRangeException(nameof(encoding));
            EndianType = endian;
            if (!BaseStream.CanRead)
            {
                throw new ArgumentException("Stream does not support reading or is already closed.", nameof(input));
            }
        }

        /// <summary>Initializes a new instance of the <see cref="DataReader" /> class.</summary>
        /// <param name="input">The stream to read from.</param>
        /// <param name="newLineMode">New line mode.</param>
        /// <param name="encoding">Encoding to use for characters and strings.</param>
        /// <param name="endian">The endian type.</param>
        /// <exception cref="ArgumentNullException">output.</exception>
        /// <exception cref="ArgumentException">Stream does not support writing or is already closed.;output.</exception>
        /// <exception cref="NotSupportedException">StringEncoding {0} not supported! or EndianType {0} not supported!.</exception>
        public DataReader(Stream input, Encoding encoding, NewLineMode newLineMode = NewLineMode.LF, EndianType endian = EndianType.LittleEndian)
        {
            BaseStream = input ?? throw new ArgumentNullException(nameof(input));
            NewLineMode = newLineMode;
            Encoding = encoding ?? throw new ArgumentOutOfRangeException(nameof(encoding));
            EndianType = endian;
            if (!BaseStream.CanRead)
            {
                throw new ArgumentException("Stream does not support reading or is already closed.", nameof(input));
            }
        }

        /// <summary>
        ///     Gets or sets the Encoding to use for characters and strings. Setting this value directly sets
        ///     <see cref="StringEncoding" /> to <see cref="StringEncoding.Undefined" />.
        /// </summary>
        public Encoding Encoding
        {
            get => textDecoder;
            set
            {
                textDecoder = value;
                newLineBytes = null;
                zeroBytes = null;
            }
        }

        /// <summary>Gets or sets the new line mode used.</summary>
        public NewLineMode NewLineMode { get; set; }

        /// <summary>Gets or sets the endian encoder type.</summary>
        /// <value>The endian encoder type.</value>
        public EndianType EndianType
        {
            get => endianType;
            set
            {
                endianType = value;
                endianDecoder = endianType switch
                {
                    EndianType.LittleEndian => BitConverterLE.Instance,
                    EndianType.BigEndian => BitConverterBE.Instance,
                    _ => throw new NotImplementedException($"EndianType {endianType} not implemented!")
                };
            }
        }

        /// <summary>Gets or sets encoding to use for characters and strings.</summary>
        public StringEncoding StringEncoding
        {
            get => textDecoder.ToStringEncoding();
            set
            {
                switch (value)
                {
                    case StringEncoding.Undefined: break;
                    case StringEncoding.ASCII:
                        textDecoder = new CheckedASCIIEncoding();
                        break;
                    case StringEncoding.UTF8:
                        textDecoder = Encoding.UTF8;
                        break;
                    case StringEncoding.UTF16:
                        textDecoder = Encoding.Unicode;
                        break;
                    case StringEncoding.UTF32:
                        textDecoder = Encoding.UTF32;
                        break;
                    default:
                        textDecoder = Encoding.GetEncoding((int) value);
                        break;
                }

                newLineBytes = null;
                newLineChars = null;
                zeroBytes = null;
            }
        }

        /// <summary>Gets access to the base stream.</summary>
        public Stream BaseStream { get; private set; }

        /// <summary>
        ///     Gets the available bytes for reading. Attention: the BaseStream has to support the Length and Position
        ///     properties.
        /// </summary>
        public long Available => BaseStream.Length - BaseStream.Position;

        /// <summary>Flushes the stream.</summary>
        public void Flush() { BaseStream.Flush(); }

        /// <summary>Seeks at the base stream (this requires the stream to be seekable).</summary>
        /// <param name="offset">Offset to seek to.</param>
        /// <param name="origin">Origin to seek from.</param>
        /// <returns>A value of type SeekOrigin indicating the reference point used to obtain the new position.</returns>
        public long Seek(long offset, SeekOrigin origin) => BaseStream.Seek(offset, origin);

        /// <summary>Skips some bytes at the base stream.</summary>
        /// <param name="count">Length to skip in bytes.</param>
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

        /// <summary>Reads the specified value from the stream.</summary>
        /// <returns>The value.</returns>
        public bool ReadBool() => ReadByte() != 0;

        /// <summary>Reads the specified value from the stream.</summary>
        /// <returns>The value.</returns>
        public byte ReadByte()
        {
            var b = BaseStream.ReadByte();
            if (b < 0)
            {
                throw new EndOfStreamException();
            }

            return (byte) b;
        }

        /// <summary>Reads a byte buffer with length prefix from the stream.</summary>
        /// <exception cref="InvalidDataException">Thrown if a invalid 7bit encoded value is found.</exception>
        /// <returns>The value.</returns>
        public byte[] ReadBytes()
        {
            var length = Read7BitEncodedInt32();
            if (length < 0)
            {
                if (length == -1)
                {
                    return null;
                }

                throw new InvalidDataException("Invalid 7bit encoded value found!");
            }

            return ReadBytes(length);
        }

        /// <summary>Reads a buffer from the stream.</summary>
        /// <param name="count">Number of bytes to read.</param>
        /// <returns>The value.</returns>
        public byte[] ReadBytes(int count)
        {
            var result = new byte[count];
            var done = 0;
            while (done < count)
            {
                var read = BaseStream.Read(result, done, count - done);
                if (read == 0)
                {
                    throw new EndOfStreamException();
                }

                done += read;
            }

            return result;
        }

        /// <summary>Reads a character from the stream.</summary>
        /// <returns>The value.</returns>
        public char ReadChar()
        {
            if (textDecoder.CodePage == (int) StringEncoding.UTF_7)
            {
                return ReadCharUTF7();
            }

            if (textDecoder.CodePage == (int) StringEncoding.US_ASCII)
            {
                var b = ReadByte();
                if (b > 127)
                {
                    throw new InvalidDataException($"Byte '{b}' is not a valid ASCII character!");
                }

                return (char) b;
            }

            var result = ReadChars(1);
            if (result.Length > 1)
            {
                throw new InvalidDataException("Decoded characters do not match single character read!");
            }

            return result[0];
        }

        /// <summary>Reads characters from the stream.</summary>
        /// <param name="count">Number of characters (not bytes) to read.</param>
        /// <returns>The value.</returns>
        public char[] ReadChars(int count)
        {
            if (textDecoder.IsSingleByte)
            {
                return textDecoder.GetChars(ReadBytes(count));
            }

            switch (textDecoder.CodePage)
            {
                case (int) StringEncoding.UTF_8: return textDecoder.GetChars(ReadCharsUTF8(count));
                case (int) StringEncoding.UTF_16: return textDecoder.GetChars(ReadCharsUTF16LE(count));
                case (int) StringEncoding.UTF_16BE: return textDecoder.GetChars(ReadCharsUTF16BE(count));
                case (int) StringEncoding.UTF_32: return textDecoder.GetChars(ReadCharsUTF32(count));
                case (int) StringEncoding.UTF_7: return ReadCharsUTF7(count);
            }

            if (textDecoder.IsDead())
            {
                throw new NotSupportedException($"Encoding {StringEncoding} does not support direct char reading!");
            }

            var buf = ReadBytes(count);
            var dec = textDecoder.GetDecoder();
            var result = new char[count];
            dec.Convert(buf, 0, count, result, 0, count, false, out var bytesUsed, out var resultCount, out var complete);
            if (bytesUsed != buf.Length)
            {
                throw new InvalidDataException("Invalid additional data!");
            }

            while (!complete || (resultCount < count))
            {
                var charCountLeft = count - resultCount;
                buf = ReadBytes(charCountLeft);
                dec.Convert(buf, 0, charCountLeft, result, resultCount, charCountLeft, false, out bytesUsed, out var charsUsed, out complete);
                if (bytesUsed != buf.Length)
                {
                    throw new InvalidDataException("Invalid additional data!");
                }

                resultCount += charsUsed;
            }

            return result;
        }

        /// <summary>Reads a value from the stream.</summary>
        /// <returns>The value.</returns>
        public decimal ReadDecimal()
        {
            var bits = new int[4];
            for (var i = 0; i < 4; i++)
            {
                bits[i] = ReadInt32();
            }

            return new decimal(bits);
        }

        /// <summary>Reads a value from the stream.</summary>
        /// <returns>The value.</returns>
        public double ReadDouble()
        {
            var bytes = ReadBytes(8);
            return endianDecoder.ToDouble(bytes, 0);
        }

        /// <summary>Writes the specified value directly to the stream.</summary>
        /// <returns>The value.</returns>
        public float ReadSingle()
        {
            var bytes = ReadBytes(4);
            return endianDecoder.ToSingle(bytes, 0);
        }

        /// <summary>Reads a value directly from the stream.</summary>
        /// <returns>The value.</returns>
        public sbyte ReadInt8()
        {
            unchecked
            {
                return (sbyte) ReadByte();
            }
        }

        /// <summary>Reads a value directly from the stream.</summary>
        /// <returns>The value.</returns>
        public short ReadInt16()
        {
            var bytes = ReadBytes(2);
            return endianDecoder.ToInt16(bytes, 0);
        }

        /// <summary>Reads a value directly from the stream.</summary>
        /// <returns>The value.</returns>
        public int ReadInt32()
        {
            var bytes = ReadBytes(4);
            return endianDecoder.ToInt32(bytes, 0);
        }

        /// <summary>Reads a value directly from the stream.</summary>
        /// <returns>The value.</returns>
        public long ReadInt64()
        {
            var bytes = ReadBytes(8);
            return endianDecoder.ToInt64(bytes, 0);
        }

        /// <summary>Reads a value directly from the stream.</summary>
        /// <returns>The value.</returns>
        public byte ReadUInt8() => ReadByte();

        /// <summary>Reads a value directly from the stream.</summary>
        /// <returns>The value.</returns>
        public ushort ReadUInt16()
        {
            var bytes = ReadBytes(2);
            return endianDecoder.ToUInt16(bytes, 0);
        }

        /// <summary>Reads a value directly from the stream.</summary>
        /// <returns>The value.</returns>
        public uint ReadUInt32()
        {
            var bytes = ReadBytes(4);
            return endianDecoder.ToUInt32(bytes, 0);
        }

        /// <summary>Reads a value directly from the stream.</summary>
        /// <returns>The value.</returns>
        public ulong ReadUInt64()
        {
            var bytes = ReadBytes(8);
            return endianDecoder.ToUInt64(bytes, 0);
        }

        /// <summary>Reads a string ending with [CR]LF from the stream.</summary>
        /// <param name="maximumBytes">The maximum number of bytes to read.</param>
        /// <returns>The string.</returns>
        public string ReadLine(int maximumBytes = 64 * 1024)
        {
            if (newLineBytes == null)
            {
                if (textDecoder.IsDead() || (textDecoder.GetString(textDecoder.GetBytes("\r\n")) != "\r\n"))
                {
                    throw new NotSupportedException($"Encoding {textDecoder.EncodingName} does not support WriteLine/ReadLine!");
                }

                newLineChars = NewLineMode switch
                {
                    NewLineMode.CR => "\r",
                    NewLineMode.LF => "\n",
                    NewLineMode.CRLF => "\r\n",
                    _ => throw new NotImplementedException($"NewLineMode {NewLineMode} not implemented!")
                };
                newLineBytes = textDecoder.GetBytes(newLineChars);
            }

            if (textDecoder.CodePage == (int) StringEncoding.UTF_7)
            {
                return ReadCharsUTF7(maximumBytes, newLineChars);
            }

            var buf = new byte[maximumBytes];
            var offset = 0;
            while (true)
            {
                ReadUntil(buf, ref offset, false, newLineBytes);
                var s = textDecoder.GetString(buf, 0, offset);
                var i = s.IndexOf(newLineChars, StringComparison.Ordinal);
                if (i > -1)
                {
                    if (s.Length > (i + newLineChars.Length))
                    {
                        throw new InvalidDataException("Decoded data does not match!");
                    }

                    return s.Substring(0, i);
                }
            }
        }

        /// <summary>Reads bytes from the stream until one of the specified end markers are found or max count is reached.</summary>
        /// <param name="data">An array of bytes.</param>
        /// <param name="offset">
        ///     The zero-based byte offset in buffer at which to begin storing the data read from the current
        ///     stream.
        /// </param>
        /// <param name="removeMarker">If set, removes found mark from endMark array.</param>
        /// <param name="endMark">Array of ending markers.</param>
        public void ReadUntil(byte[] data, ref int offset, bool removeMarker, params byte[] endMark)
        {
            var maxCount = data.Length;
            var completed = false;
            var endMarkLast = endMark.Length - 1;
            while (!completed)
            {
                if (offset >= data.Length)
                {
                    throw new InvalidDataException($"Refusing to read more than {maxCount} bytes at ReadUntil()!");
                }

                var b = data[offset] = ReadByte();
                if ((offset >= endMarkLast) && (b == endMark[endMarkLast]))
                {
                    completed = true;
                    var i = endMarkLast - 1;
                    var n = offset - 1;
                    while (i >= 0)
                    {
                        if (data[n--] != endMark[i--])
                        {
                            completed = false;
                            break;
                        }
                    }
                }

                offset += 1;
            }

            if (removeMarker)
            {
                offset -= endMark.Length;
            }
        }

        /// <summary>Reads a string of the specified byte count from the stream.</summary>
        /// <param name="count">Number of bytes to read.</param>
        /// <returns>The string.</returns>
        public string ReadString(int count) => textDecoder.GetString(ReadBytes(count));

        /// <summary>Reads a string with length prefix from the stream.</summary>
        /// <exception cref="InvalidDataException">Thrown if invalid 7bit encoded value found.</exception>
        /// <returns>The string.</returns>
        public string ReadString()
        {
            var length = Read7BitEncodedInt32();
            if (length < 0)
            {
                if (length == -1)
                {
                    return null;
                }

                throw new InvalidDataException("Invalid 7bit encoded value found!");
            }

            return ReadString(length);
        }

        /// <summary>Reads a zero terminated string from the stream.</summary>
        /// <param name="maximumBytes">The number of bytes to write at maximum.</param>
        /// <returns>The string.</returns>
        public string ReadZeroTerminatedString(int maximumBytes)
        {
            if (textDecoder.CodePage == (int) StringEncoding.UTF_7)
            {
                return ReadCharsUTF7(maximumBytes, "\0");
            }

            if (zeroBytes == null)
            {
                if (textDecoder.IsDead() || (textDecoder.GetString(textDecoder.GetBytes("\0")) != "\0"))
                {
                    throw new NotSupportedException($"Encoding {textDecoder.EncodingName} does not support ReadZeroTerminatedString!");
                }

                zeroBytes = textDecoder.GetBytes("\0");
            }

            var buf = new byte[maximumBytes];
            var offset = 0;
            while (true)
            {
                ReadUntil(buf, ref offset, false, zeroBytes);
                var s = textDecoder.GetString(buf, 0, offset);
                var i = s.IndexOf('\0');
                if (i > -1)
                {
                    return s.Substring(0, i);
                }
            }
        }

        /// <summary>Reads a zero terminated string from the stream.</summary>
        /// <param name="byteCount">Fieldlength in bytes.</param>
        /// <returns>The string.</returns>
        public string ReadZeroTerminatedFixedLengthString(int byteCount)
        {
            var result = ReadString(byteCount);
            var i = result.IndexOf((char) 0);
            if (i > -1)
            {
                result = result.Substring(0, i);
            }

            return result;
        }

        /// <summary>Reads a value from the stream.</summary>
        /// <returns>The value.</returns>
        public TimeSpan ReadTimeSpan() => new TimeSpan(ReadInt64());

        /// <summary>Reads a DateTime value from the stream with <see cref="DateTimeKind" />.</summary>
        /// <returns>The value.</returns>
        public DateTime ReadDateTime()
        {
            var kind = (DateTimeKind) Read7BitEncodedInt32();
            switch (kind)
            {
                case DateTimeKind.Local:
                case DateTimeKind.Unspecified:
                case DateTimeKind.Utc:
                    break;
                default: throw new InvalidDataException("Invalid DateTimeKind!");
            }

            return new DateTime(ReadInt64(), kind);
        }

        /// <summary>Reads a 32bit linux epoch value.</summary>
        /// <returns>The value.</returns>
        public DateTime ReadEpoch32() => new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(ReadUInt32());

        /// <summary>Reads a 64bit linux epoch value.</summary>
        /// <returns>The value.</returns>
        public DateTime ReadEpoch64() => new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(ReadUInt64());

        /// <summary>Reads the specified struct from the stream using the default marshaller.</summary>
        /// <typeparam name="T">the struct.</typeparam>
        /// <returns>The struct.</returns>
        public T ReadStruct<T>()
            where T : struct
        {
            var size = MarshalStruct.SizeOf<T>();
            var buffer = ReadBytes(size);
            MarshalStruct.Copy(buffer, out T result);
            return result;
        }

        /// <summary>Reads a 7 bit encoded 32 bit value from the stream.</summary>
        /// <returns>The value.</returns>
        public int Read7BitEncodedInt32() => BitCoder32.Read7BitEncodedInt32(BaseStream);

        /// <summary>Reads a 7 bit encoded 32 bit value from the stream.</summary>
        /// <returns>The value.</returns>
        public uint Read7BitEncodedUInt32() => BitCoder32.Read7BitEncodedUInt32(BaseStream);

        /// <summary>Reads a 7 bit encoded 64 bit value from the stream.</summary>
        /// <returns>The value.</returns>
        public long Read7BitEncodedInt64() => BitCoder64.Read7BitEncodedInt64(BaseStream);

        /// <summary>Reads a 7 bit encoded 64 bit value from the stream.</summary>
        /// <returns>The value.</returns>
        public ulong Read7BitEncodedUInt64() => BitCoder64.Read7BitEncodedUInt64(BaseStream);

        /// <summary>Reads an array of the specified struct type from the stream using the default marshaller.</summary>
        /// <typeparam name="T">Type of each element.</typeparam>
        /// <returns>The struct array.</returns>
        public T[] ReadArray<T>()
            where T : struct
        {
            var count = Read7BitEncodedInt32();
            if (count < 0)
            {
                throw new InvalidDataException("Invalid length prefix while reading array!");
            }

            if (count == 0)
            {
                return new T[0];
            }

            var byteCount = Read7BitEncodedInt32();
            var bytes = ReadBytes(byteCount);
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

        /// <summary>Closes the reader and the stream.</summary>
        public void Close()
        {
            if (BaseStream != null)
            {
#if NETSTANDARD13
                BaseStream.Dispose();
#else
                BaseStream.Close();
#endif
                BaseStream = null;
            }
        }

        #region private string reader implementation for reading strings without buffering

        byte[] ReadCharsUTF8(int charCount)
        {
            var chars = 0;
            var result = new List<byte>(charCount * 4);
            while (true)
            {
                for (; chars < charCount; chars++)
                {
                    var b = ReadByte();
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
                        // 2nd byte
                        result.Add(ReadByte());
                    }
                    if (b >= 0xE0)
                    {
                        // 3rd byte
                        result.Add(ReadByte());
                    }

                    if (b >= 0xF0)
                    {
                        // 4th byte
                        result.Add(ReadByte());
                    }

                    #endregion
                }

                // remove bom at beginning
                if ((result.Count > 2) && (result[0] == 0xEF) && (result[1] == 0xBB) && (result[2] == 0xBF))
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
            var result = new List<byte>(charCount * 4);
            for (var i = 0; i < charCount; i++)
            {
                var b1 = ReadByte();
                var b2 = ReadByte();
                result.Add(b1);
                result.Add(b2);
                if ((b2 > 0xD7) && (b2 < 0xDC))
                {
                    // add low surrogate
                    result.Add(ReadByte());
                    result.Add(ReadByte());
                }
            }

            return result.ToArray();
        }

        byte[] ReadCharsUTF16BE(int charCount)
        {
            var result = new List<byte>(charCount * 4);
            for (var i = 0; i < charCount; i++)
            {
                var b1 = ReadByte();
                var b2 = ReadByte();
                result.Add(b1);
                result.Add(b2);
                if ((b1 > 0xD7) && (b1 < 0xDC))
                {
                    // add low surrogate
                    result.Add(ReadByte());
                    result.Add(ReadByte());
                }
            }

            return result.ToArray();
        }

        byte[] ReadCharsUTF32(int charCount) => ReadBytes(4 * charCount);

        char ReadCharUTF7()
        {
            var b = ReadByte();
            if (b == '+')
            {
                var i = 0;
                var buf = new byte[8];
                buf[i++] = b;
                do
                {
                    buf[i++] = b = ReadByte();
                }
                while (b != '-');

                var chars = textDecoder.GetChars(buf, 0, i);
                if (chars.Length > 1)
                {
                    throw new InvalidDataException("Cannot parse utf8 stateless with unencoded '+' character!");
                }

                return chars[0];
            }

            return (char) b;
        }

        char[] ReadCharsUTF7(int charCount)
        {
            byte[] buf = null;
            var result = new char[charCount];
            var i = 0;
            while (i < charCount)
            {
                var b = ReadByte();
                if (b == '+')
                {
                    if (buf == null)
                    {
                        buf = new byte[128];
                    }

                    var n = 0;
                    buf[n++] = b;
                    while (b != '-')
                    {
                        if (n == buf.Length)
                        {
                            Array.Resize(ref buf, buf.Length << 1);
                        }

                        buf[n++] = b = ReadByte();
                    }

                    var chars = textDecoder.GetChars(buf, 0, n);
                    if ((i + chars.Length) > charCount)
                    {
                        throw new InvalidDataException("Encoded character length does not match!");
                    }

                    chars.CopyTo(result, i);
                    i += chars.Length;
                }
                else
                {
                    result[i++] = (char) b;
                }
            }

            return result;
        }

        string ReadCharsUTF7(int maxBytes, string endMarker)
        {
            var result = string.Empty;
            byte[] buf = null;
            while (!result.EndsWith(endMarker, StringComparison.Ordinal))
            {
                if (--maxBytes < 0)
                {
                    throw new InvalidDataException("Exceeded maximum byte count during read.");
                }

                var b = ReadByte();
                if (b == '+')
                {
                    if (buf == null)
                    {
                        buf = new byte[128];
                    }

                    var n = 0;
                    buf[n++] = b;
                    while (b != '-')
                    {
                        if (n == buf.Length)
                        {
                            Array.Resize(ref buf, buf.Length << 1);
                        }

                        if (--maxBytes < 0)
                        {
                            throw new InvalidDataException("Exceeded maximum byte count during read.");
                        }

                        buf[n++] = b = ReadByte();
                    }

                    var chars = textDecoder.GetChars(buf, 0, n);
                    result += new string(chars);
                }
                else
                {
                    result += (char) b;
                }
            }

            return result.Substring(0, result.Length - endMarker.Length);
        }

        #endregion
    }
}
