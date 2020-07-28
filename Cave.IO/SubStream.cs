using System;
using System.IO;

namespace Cave.IO
{
    /// <summary>Provides a sub stream implementation.</summary>
    public class SubStream : Stream
    {
        long position;

        /// <summary>Initializes a new instance of the <see cref="SubStream" /> class.</summary>
        /// <remarks>Creates a new SubStream from the specified stream at its current read/write position.</remarks>
        /// <param name="stream">The stream to create the substream from.</param>
        public SubStream(Stream stream)
            : this(stream, 0)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SubStream" /> class.</summary>
        /// <remarks>Creates a new SubStream from the specified stream at the specified read/write position.</remarks>
        /// <param name="stream">The stream to create the substream from.</param>
        /// <param name="seek">The start position of the substream relative to the current stream position.</param>
        public SubStream(Stream stream, int seek)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (seek != 0)
            {
                if (stream.CanSeek)
                {
                    stream.Seek(seek, SeekOrigin.Current);
                }
                else if (seek < 0)
                {
                    throw new NotSupportedException("The stream does not support seeking!");
                }
                else
                {
                    if (seek != stream.Read(new byte[seek], 0, seek))
                    {
                        throw new EndOfStreamException();
                    }
                }
            }

            BaseStream = stream;
        }

        /// <summary>Gets the BaseStream.</summary>
        public Stream BaseStream { get; }

        /// <summary>Gets a value indicating whether the stream can be read or not.</summary>
        public override bool CanRead => BaseStream.CanRead;

        /// <summary>Gets a value indicating whether the stream can seek or not.</summary>
        public override bool CanSeek => BaseStream.CanSeek;

        /// <summary>Gets a value indicating whether the stream can be written or not.</summary>
        public override bool CanWrite => BaseStream.CanWrite;

        /// <summary>Gets the length of the stream.</summary>
        public override long Length => (BaseStream.Length - BaseStream.Position) + position;

        /// <summary>Gets or sets the current read/write position.</summary>
        public override long Position { get => position; set => Seek(value, SeekOrigin.Begin); }

        /// <summary>Flushes the stream.</summary>
        public override void Flush() { BaseStream.Flush(); }

        /// <summary>Reads a byte buffer from the stream at the current position.</summary>
        /// <param name="buffer">The buffer as byte array.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The number of bytes.</param>
        /// <returns>Number of bytes read.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var l_Read = BaseStream.Read(buffer, offset, count);
            position += l_Read;
            return l_Read;
        }

        /// <summary>seeks the stream.</summary>
        /// <param name="offset">The offset to start from.</param>
        /// <param name="origin">Specifies the position in a stream to use for seeking.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    if (offset < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(offset));
                    }

                    var result = Seek(offset - position, origin);
                    position = offset;
                    return result;
                case SeekOrigin.Current:
                    return Seek(position + offset, SeekOrigin.Begin);
                default: throw new NotSupportedException($"SeekOrigin {origin} not supported!");
            }
        }

        /// <summary>not supported.</summary>
        /// <param name="value">The new length.</param>
        public override void SetLength(long value) { throw new NotSupportedException(); }

        /// <summary>Writes a byte buffer to the stream at the current position.</summary>
        /// <param name="buffer">An array of bytes.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(buffer, offset, count);
            position += count;
        }
    }
}
