using System;
using System.IO;

namespace Cave.IO
{
    /// <summary>
    /// Provides a sub stream implementation.
    /// </summary>
    public class SubStream : Stream
    {
        long position = 0;

        /// <summary>
        /// Creates a new SubStream from the specified stream at its current read/write position.
        /// </summary>
        /// <param name="stream">The stream to create the substream from.</param>
        public SubStream(Stream stream)
            : this(stream, 0)
        {
        }

        /// <summary>
        /// Creates a new SubStream from the specified stream at the specified read/write position.
        /// </summary>
        /// <param name="stream">The stream to create the substream from.</param>
        /// <param name="seek">The start position of the substream relative to the current stream position.</param>
        public SubStream(Stream stream, int seek)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (seek != 0)
            {
                if (stream.CanSeek)
                {
                    stream.Seek(seek, SeekOrigin.Current);
                }
                else if (seek < 0)
                {
                    throw new NotSupportedException(string.Format("The stream does not support seeking!"));
                }
                else
                {
                    if (seek != stream.Read(new byte[seek], 0, seek))
                    {
                        throw new EndOfStreamException();
                    }
                }
            }
            this.BaseStream = stream;
        }

        /// <summary>
        /// Gets the BaseStream.
        /// </summary>
        public Stream BaseStream { get; private set; }

        /// <summary>
        /// Obtains whether the stream can be read or not.
        /// </summary>
        public override bool CanRead => BaseStream.CanRead;

        /// <summary>
        /// Obtains whether the stream can seek or not.
        /// </summary>
        public override bool CanSeek => BaseStream.CanSeek;

        /// <summary>
        /// Obtains whether the stream can be written or not.
        /// </summary>
        public override bool CanWrite => BaseStream.CanWrite;

        /// <summary>
        /// flushes the stream.
        /// </summary>
        public override void Flush()
        {
            BaseStream.Flush();
        }

        /// <summary>
        /// Obtains the length of the stream.
        /// </summary>
        public override long Length => BaseStream.Length - BaseStream.Position + position;

        /// <summary>
        /// Gets/sets the current read/write position.
        /// </summary>
        public override long Position
        {
            get => position;
            set => Seek(value, SeekOrigin.Begin);
        }

        /// <summary>
        /// Reads a byte buffer from the stream at the current position.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int l_Read = BaseStream.Read(buffer, offset, count);
            position += l_Read;
            return l_Read;
        }

        /// <summary>
        /// seeks the stream.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    if (offset < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(offset));
                    }

                    long result = Seek(offset - position, origin);
                    position = offset;
                    return result;

                case SeekOrigin.Current:
                    return Seek(position + offset, SeekOrigin.Begin);

                default: throw new NotSupportedException(string.Format("SeekOrigin {0} not supported!", origin));
            }
        }

        /// <summary>
        /// not supported.
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes a byte buffer to the stream at the current position.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(buffer, offset, count);
            position += count;
        }
    }
}
