using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Cave.IO
{
    /// <summary>
    ///     Provides a new little endian binary writer implementation (a combination of streamwriter and binarywriter).
    ///     This class is not threadsafe and does not buffer anything nor needs flushing. You can access the basestream at any
    ///     time with any mode of operation (read, write, seek, ...).
    /// </summary>
    public sealed class DataWriter
    {
        IBitConverter endianEncoder;
        EndianType endianType;
        bool lineFeedTested;
        Encoding textEncoder;
        bool zeroTested;

        /// <summary>Initializes a new instance of the <see cref="DataWriter" /> class.</summary>
        /// <param name="output">The stream to write to.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="endian">The endian type.</param>
        /// <param name="newLineMode">New line mode.</param>
        /// <exception cref="ArgumentNullException">output.</exception>
        /// <exception cref="ArgumentException">Stream does not support writing or is already closed.;output.</exception>
        /// <exception cref="NotSupportedException">StringEncoding {0} not supported! or EndianType {0} not supported!.</exception>
        public DataWriter(Stream output, StringEncoding encoding = StringEncoding.UTF8, NewLineMode newLineMode = NewLineMode.LF,
            EndianType endian = EndianType.LittleEndian)
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

        /// <summary>Initializes a new instance of the <see cref="DataWriter" /> class.</summary>
        /// <param name="output">The stream to write to.</param>
        /// <param name="newLineMode">New line mode.</param>
        /// <param name="encoding">Encoding to use for characters and strings.</param>
        /// <param name="endian">The endian type.</param>
        /// <exception cref="ArgumentNullException">output.</exception>
        /// <exception cref="ArgumentException">Stream does not support writing or is already closed.;output.</exception>
        /// <exception cref="NotSupportedException">StringEncoding {0} not supported! or EndianType {0} not supported!.</exception>
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
        ///     Gets or sets the Encoding to use for characters and strings. Setting this value directly sets
        ///     <see cref="StringEncoding" /> to <see cref="StringEncoding.Undefined" />.
        /// </summary>
        public Encoding Encoding
        {
            get => textEncoder;
            set
            {
                textEncoder = value;
                lineFeedTested = false;
                zeroTested = false;
            }
        }

        /// <summary>Gets or sets the endian encoder type.</summary>
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
                    default: throw new NotImplementedException($"EndianType {endianType} not implemented!");
                }
            }
        }

        /// <summary>Gets or sets encoding to use for characters and strings.</summary>
        public StringEncoding StringEncoding
        {
            get => textEncoder.ToStringEncoding();
            set
            {
                switch (value)
                {
                    case StringEncoding.Undefined: break;
                    case StringEncoding.ASCII:
                        textEncoder = new CheckedASCIIEncoding();
                        break;
                    case StringEncoding.UTF8:
                        textEncoder = Encoding.UTF8;
                        break;
                    case StringEncoding.UTF16:
                        textEncoder = Encoding.Unicode;
                        break;
                    case StringEncoding.UTF32:
                        textEncoder = Encoding.UTF32;
                        break;
                    default:
                        textEncoder = Encoding.GetEncoding((int) value);
                        break;
                }

                lineFeedTested = false;
                zeroTested = false;
            }
        }

        /// <summary>Gets or sets the new line mode used.</summary>
        public NewLineMode NewLineMode { get; set; }

        /// <summary>Gets access to the base stream.</summary>
        public Stream BaseStream { get; private set; }

        /// <summary>Flushes the stream.</summary>
        public void Flush() { BaseStream.Flush(); }

        /// <summary>Seeks at the base stream (this requires the stream to be seekable).</summary>
        /// <param name="offset">Offset to seek to.</param>
        /// <param name="origin">Origin to seek from.</param>
        /// <returns>A value of type SeekOrigin indicating the reference point used to obtain the new position.</returns>
        public long Seek(int offset, SeekOrigin origin) => BaseStream.Seek(offset, origin);

        /// <summary>Writes the specified value directly to the stream.</summary>
        /// <param name="value">The value to write.</param>
        public void Write(bool value) { BaseStream.WriteByte(value ? (byte) 1 : (byte) 0); }

        /// <summary>Writes the specified value directly to the stream.</summary>
        /// <param name="value">The value to write.</param>
        public void Write(byte value) { BaseStream.WriteByte(value); }

        /// <summary>Writes the specified buffer directly to the stream.</summary>
        /// <param name="buffer">The buffer to write.</param>
        public void Write(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            BaseStream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>Writes the specified buffer to the stream with length prefix.</summary>
        /// <param name="buffer">The buffer to write.</param>
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

        /// <summary>Writes a part of the specified buffer directly to the stream.</summary>
        /// <param name="buffer">The buffer to write.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            BaseStream.Write(buffer, offset, count);
        }

        /// <summary>Writes a part of the specified buffer to the stream with length prefix.</summary>
        /// <param name="buffer">The buffer to write.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public void WritePrefixed(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            Write7BitEncoded32(count);
            BaseStream.Write(buffer, offset, count);
        }

        /// <summary>Writes the specified character directly to the stream.</summary>
        /// <param name="c">The character to write.</param>
        /// <returns>The number of bytes written.</returns>
        public int Write(char c) { return Write(new[] { c }); }

        /// <summary>Writes the specified characters directly to the stream.</summary>
        /// <param name="chars">Array of characters to write.</param>
        /// <returns>The number of bytes written.</returns>
        public int Write(char[] chars)
        {
            if (chars == null)
            {
                throw new ArgumentNullException(nameof(chars));
            }

            if (textEncoder.IsDead())
            {
                throw new NotSupportedException($"Encoding {StringEncoding} does not support direct char writing!");
            }

            var data = textEncoder.GetBytes(chars);
            Write(data);
            return data.Length;
        }

        /// <summary>Writes a part of the specified character array directly to the stream.</summary>
        /// <param name="chars">Array of characters to write.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <returns>Number of bytes written.</returns>
        public int Write(char[] chars, int offset, int count)
        {
            if (chars == null)
            {
                throw new ArgumentNullException(nameof(chars));
            }

            if (textEncoder.IsDead())
            {
                throw new NotSupportedException($"Encoding {StringEncoding} does not support direct char writing!");
            }

            var data = textEncoder.GetBytes(chars, offset, count);
            Write(data);
            return data.Length;
        }

        /// <summary>Writes the specified value directly to the stream.</summary>
        /// <param name="value">The value to write.</param>
        public void Write(decimal value)
        {
            foreach (var decimalData in decimal.GetBits(value))
            {
                Write(decimalData);
            }
        }

        /// <summary>Writes the specified value directly to the stream.</summary>
        /// <param name="value">The value to write.</param>
        public void Write(double value) { Write(endianEncoder.GetBytes(value)); }

        /// <summary>Writes the specified value directly to the stream.</summary>
        /// <param name="value">The value to write.</param>
        public void Write(short value) { Write(endianEncoder.GetBytes(value)); }

        /// <summary>Writes the specified value directly to the stream.</summary>
        /// <param name="value">The value to write.</param>
        public void Write(int value) { Write(endianEncoder.GetBytes(value)); }

        /// <summary>Writes the specified value directly to the stream.</summary>
        /// <param name="value">The value to write.</param>
        public void Write(long value) { Write(endianEncoder.GetBytes(value)); }

        /// <summary>Writes the specified value directly to the stream.</summary>
        /// <param name="value">The value to write.</param>
        public void Write(sbyte value)
        {
            // Bugfix: BitConverter.GetBytes(sbyte) returns 2 bytes...
            Write(unchecked((byte) value));
        }

        /// <summary>Writes the specified value directly to the stream.</summary>
        /// <param name="value">The value to write.</param>
        public void Write(float value) { Write(endianEncoder.GetBytes(value)); }

        /// <summary>Writes the specified string directly to the stream.</summary>
        /// <param name="text">String to write.</param>
        /// <returns>Number of bytes written.</returns>
        public int Write(string text)
        {
            if (textEncoder.IsDead())
            {
                throw new NotSupportedException($"Encoding {StringEncoding} does not support direct char writing!");
            }

            var data = textEncoder.GetBytes(text);
            Write(data);
            return data.Length;
        }

        /// <summary>Writes the specified string (with/out length prefix) directly to the stream.</summary>
        /// <param name="text">String to write.</param>
        /// <returns>Number of bytes written.</returns>
        public int WritePrefixed(string text)
        {
            if (text == null)
            {
                return Write7BitEncoded32(-1);
            }

            var data = textEncoder.GetBytes(text);
            var prefix = Write7BitEncoded32(data.Length);
            Write(data);
            return prefix + data.Length;
        }

        /// <summary>Writes the "new line" marking to the stream. This depends on the chosen <see cref="NewLineMode" />.</summary>
        /// <returns>Number of bytes written.</returns>
        public int WriteLine()
        {
            if (!lineFeedTested)
            {
                if (textEncoder.IsDead() || (textEncoder.GetString(textEncoder.GetBytes("\r\n")) != "\r\n"))
                {
                    throw new NotSupportedException($"Encoding {textEncoder.EncodingName} does not support WriteLine/ReadLine!");
                }

                lineFeedTested = true;
            }

            switch (NewLineMode)
            {
                case NewLineMode.CR: return Write('\r');
                case NewLineMode.LF: return Write('\n');
                case NewLineMode.CRLF: return Write("\r\n");
                default: throw new NotImplementedException($"NewLineMode {NewLineMode} not implemented!");
            }
        }

        /// <summary>
        ///     Writes the specified string followed by a "new line" marking to the stream. This depends on the chosen
        ///     <see cref="NewLineMode" />.
        /// </summary>
        /// <param name="text">String to write.</param>
        /// <returns>Number of bytes written.</returns>
        public int WriteLine(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            var data = textEncoder.GetBytes(text);
            Write(data);
            return data.Length + WriteLine();
        }

        /// <summary>Writes the specified string zero terminated directly to the stream.</summary>
        /// <param name="text">String to write.</param>
        /// <param name="fieldLength">Fixed field length to use (1..x).</param>
        /// <returns>Number of bytes written.</returns>
        public int WriteZeroTerminated(string text, int fieldLength = 0)
        {
            if (!zeroTested)
            {
                if (textEncoder.IsDead() || (textEncoder.GetString(textEncoder.GetBytes("\0")) != "\0"))
                {
                    throw new NotSupportedException($"Encoding {textEncoder.EncodingName} does not support WriteZeroTerminated!");
                }

                zeroTested = true;
            }

            var data = textEncoder.GetBytes(text + "\0");
            if ((fieldLength > 0) && (data.Length > fieldLength))
            {
                data[fieldLength - 1] = 0;
                Write(data, 0, fieldLength);
            }
            else
            {
                Write(data);
                if (fieldLength > 0)
                {
                    var zeroBytes = fieldLength - data.Length;
                    if (zeroBytes > 0)
                    {
                        Write(new byte[zeroBytes]);
                    }
                }
            }

            return Math.Min(data.Length, fieldLength);
        }

        /// <summary>Writes the specified value directly to the stream.</summary>
        /// <param name="value">The value to write.</param>
        public void Write(ushort value) { Write(endianEncoder.GetBytes(value)); }

        /// <summary>Writes the specified value directly to the stream.</summary>
        /// <param name="value">The value to write.</param>
        public void Write(uint value) { Write(endianEncoder.GetBytes(value)); }

        /// <summary>Writes the specified value directly to the stream.</summary>
        /// <param name="value">The value to write.</param>
        public void Write(ulong value) { Write(endianEncoder.GetBytes(value)); }

        /// <summary>Writes the specified value directly to the stream.</summary>
        /// <param name="value">The value to write.</param>
        public void Write(TimeSpan value) { Write(value.Ticks); }

        /// <summary>Writes the specified datetime value with <see cref="DateTimeKind" />.</summary>
        /// <param name="value">The value to write.</param>
        public void Write(DateTime value)
        {
            Write7BitEncoded32((int) value.Kind);
            Write(value.Ticks);
        }

        /// <summary>Writes a 32bit linux epoch value.</summary>
        /// <param name="value">The value to write.</param>
        public void WriteEpoch32(DateTime value) { Write((uint) (value - new DateTime(1970, 1, 1)).TotalSeconds); }

        /// <summary>Writes a 64bit linux epoch value.</summary>
        /// <param name="value">The value to write.</param>
        public void WriteEpoch64(DateTime value) { Write((ulong) (value - new DateTime(1970, 1, 1)).TotalSeconds); }

        /// <summary>Writes the specified struct directly to the stream using the default marshaller.</summary>
        /// <typeparam name="T">the struct.</typeparam>
        /// <param name="item">The value to write.</param>
        /// <returns>Number of bytes written.</returns>
        public int WriteStruct<T>(T item)
            where T : struct
        {
            var len = Marshal.SizeOf(item);
            var data = new byte[len];
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            Marshal.StructureToPtr(item, handle.AddrOfPinnedObject(), false);
            handle.Free();
            Write(data);
            return len;
        }

        /// <summary>Writes the specified 64 bit value to the stream 7 bit encoded (1-10 bytes).</summary>
        /// <param name="value">The value to write.</param>
        /// <returns>Number of bytes written.</returns>
        public int Write7BitEncoded64(long value) => BitCoder64.Write7BitEncoded(this, value);

        /// <summary>Writes the specified 64 bit value to the stream 7 bit encoded (1-10 bytes).</summary>
        /// <param name="value">The value to write.</param>
        /// <returns>Number of bytes written.</returns>
        public int Write7BitEncoded64(ulong value) => BitCoder64.Write7BitEncoded(this, value);

        /// <summary>Writes the specified 32 bit value to the stream 7 bit encoded (1-5 bytes).</summary>
        /// <param name="value">The value to write.</param>
        /// <returns>Number of bytes written.</returns>
        public int Write7BitEncoded32(int value) => BitCoder32.Write7BitEncoded(this, value);

        /// <summary>Writes the specified 32 bit value to the stream 7 bit encoded (1-5 bytes).</summary>
        /// <param name="value">The value to write.</param>
        /// <returns>Number of bytes written.</returns>
        public int Write7BitEncoded32(uint value) => BitCoder32.Write7BitEncoded(this, value);

        /// <summary>
        ///     Writes an array of the specified struct type to the stream using the default marshaller prefixed by array
        ///     length.
        /// </summary>
        /// <typeparam name="T">Type of each element.</typeparam>
        /// <param name="array">Array of elements.</param>
        /// <returns>Number of bytes written.</returns>
        public int WriteArray<T>(T[] array)
            where T : struct
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
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

            var headerSize = Write7BitEncoded32(bytes.Length);
            Write(bytes);
            return headerSize + bytes.Length;
        }

        /// <summary>Closes the writer and the stream.</summary>
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
    }
}
