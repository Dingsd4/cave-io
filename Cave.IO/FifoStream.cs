using System;
using System.Collections.Generic;
using System.IO;

namespace Cave.IO
{
    /// <summary>
    ///     Provides a fifo buffer for byte[] blocks readable as stream. New buffers can be appended to the end of the
    ///     stream and read like a stream. The performance of this class is best with medium sized buffers (1kiB - 64kiB).
    /// </summary>
    public class FifoStream : Stream
    {
        readonly LinkedList<byte[]> buffers = new LinkedList<byte[]>();
        LinkedListNode<byte[]> currentBuffer;
        int currentBufferPosition;
        int realLength;
        int realPosition;

        /// <summary>Gets a value indicating whether this stream can always be read.</summary>
        public override bool CanRead => true;

        /// <summary>Gets a value indicating whether this stream can seek.</summary>
        public override bool CanSeek => true;

        /// <summary>Gets a value indicating whether this stream can not be written.</summary>
        public override bool CanWrite => false;

        /// <summary>Gets provides the current length of the stream.</summary>
        public override long Length => realLength;

        /// <summary>Gets or sets the current read/write position.</summary>
        public override long Position { get => realPosition; set => Seek(value, SeekOrigin.Begin); }

        /// <summary>Gets the number of bytes available from the current read position to the end of the stream.</summary>
        public virtual int Available
        {
            get
            {
                lock (this)
                {
                    return realLength - realPosition;
                }
            }
        }

        /// <summary>Gets the number of buffers in the stream.</summary>
        public int BufferCount
        {
            get
            {
                lock (this)
                {
                    return buffers.Count;
                }
            }
        }

        /// <summary>Does nothing.</summary>
        public override void Flush() { }

        /// <summary>Determines whether the buffer contains the specified byte.</summary>
        /// <param name="b">The byte.</param>
        /// <returns>the index (a value &gt;=0) if the buffer contains the specified byte; otherwise, -1.</returns>
        public int IndexOf(byte b)
        {
            lock (this)
            {
                var index = 0;
                var node = currentBuffer;
                var pos = currentBufferPosition;
                while (node != null)
                {
                    for (; pos < node.Value.Length; pos++, index++)
                    {
                        if (node.Value[pos] == b)
                        {
                            return index;
                        }
                    }

                    node = node.Next;
                    pos = 0;
                }

                return -1;
            }
        }

        /// <summary>Determines whether the buffer contains the specified byte.</summary>
        /// <param name="b">The byte.</param>
        /// <returns><c>true</c> if the buffer contains the specified byte; otherwise, <c>false</c>.</returns>
        public bool Contains(byte b)
        {
            lock (this)
            {
                var node = currentBuffer;
                var pos = currentBufferPosition;
                while (node != null)
                {
                    for (; pos < node.Value.Length; pos++)
                    {
                        if (node.Value[pos] == b)
                        {
                            return true;
                        }
                    }

                    node = node.Next;
                    pos = 0;
                }

                return false;
            }
        }

        /// <summary>Determines whether the buffer contains the specified data.</summary>
        /// <param name="data">The data.</param>
        /// <returns>the index (a value &gt;=0) if the buffer contains the specified bytes; otherwise, -1.</returns>
        public int IndexOf(byte[] data)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));
            lock (this)
            {
                var index = 0;
                var checkIndex = 0;
                var node = currentBuffer;
                var pos = currentBufferPosition;
                while (node != null)
                {
                    for (; pos < node.Value.Length; pos++, index++)
                    {
                        if (node.Value[pos] == data[checkIndex])
                        {
                            if (++checkIndex == data.Length)
                            {
                                return (index - checkIndex) + 1;
                            }
                        }
                        else
                        {
                            checkIndex = 0;
                        }
                    }

                    node = node.Next;
                    pos = 0;
                }

                return -1;
            }
        }

        /// <summary>Determines whether the buffer contains the specified data.</summary>
        /// <param name="data">The data.</param>
        /// <returns><c>true</c> if the buffer contains the specified data; otherwise, <c>false</c>.</returns>
        public bool Contains(byte[] data)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));
            lock (this)
            {
                var checkIndex = 0;
                var node = currentBuffer;
                var pos = currentBufferPosition;
                while (node != null)
                {
                    for (; pos < node.Value.Length; pos++)
                    {
                        if (node.Value[pos] == data[checkIndex])
                        {
                            if (++checkIndex == data.Length)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            checkIndex = 0;
                        }
                    }

                    node = node.Next;
                    pos = 0;
                }

                return false;
            }
        }

        /// <summary>Peeks at the next byte in the buffer. Returns -1 if no more data available.</summary>
        /// <returns>The next byte if available.</returns>
        public virtual int PeekByte()
        {
            lock (this)
            {
                if (currentBuffer == null)
                {
                    return -1;
                }

                return currentBuffer.Value[currentBufferPosition];
            }
        }

        /// <summary>
        ///     Reads the next byte in the buffer (much faster than <see cref="Read" />). Returns -1 if no more data
        ///     available.
        /// </summary>
        /// <returns>The next byte if available.</returns>
        public override int ReadByte()
        {
            lock (this)
            {
                var result = PeekByte();
                if (result > -1)
                {
                    Seek(1, SeekOrigin.Current);
                }

                return result;
            }
        }

        /// <summary>Reads some bytes at the current position from the stream.</summary>
        /// <param name="buffer">An array of bytes.</param>
        /// <param name="offset">
        ///     The zero-based byte offset in buffer at which to begin storing the data read from the current
        ///     stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (this)
            {
                count = Math.Min(count, Available);
                var resultSize = 0;
                while ((count > 0) && (currentBuffer != null))
                {
                    var currentBuffer = this.currentBuffer.Value;
                    var blockSize = Math.Min(currentBuffer.Length - currentBufferPosition, count);
                    Array.Copy(currentBuffer, currentBufferPosition, buffer, offset, blockSize);
                    resultSize += blockSize;
                    count -= blockSize;
                    offset += blockSize;
                    currentBufferPosition += blockSize;
                    realPosition += blockSize;
                    if (currentBufferPosition == currentBuffer.Length)
                    {
                        currentBufferPosition = 0;
                        this.currentBuffer = this.currentBuffer.Next;
                    }
                }

                return resultSize;
            }
        }

        /// <summary>Throws a NotSupportedException.</summary>
        /// <param name="buffer">An array of bytes.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }

        /// <summary>Moves the read / write position in the stream.</summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            lock (this)
            {
                try
                {
                    switch (origin)
                    {
                        case SeekOrigin.Current:
                            if (((realPosition + offset) > realLength) ||
                                ((realPosition + offset) < 0) ||
                                (currentBuffer == null))
                            {
                                throw new ArgumentOutOfRangeException(nameof(offset));
                            }

                            realPosition += (int) offset;
                            offset += currentBufferPosition;
                            currentBufferPosition = 0;
                            while (offset < 0)
                            {
                                currentBuffer = currentBuffer.Previous;
                                offset += currentBuffer.Value.Length;
                            }

                            if (offset > 0)
                            {
                                while ((currentBuffer != null) && (offset >= currentBuffer.Value.Length))
                                {
                                    offset -= currentBuffer.Value.Length;
                                    currentBuffer = currentBuffer.Next;
                                }

                                currentBufferPosition = (int) offset;
                                if ((currentBufferPosition > 0) && (currentBuffer == null))
                                {
                                    throw new EndOfStreamException();
                                }
                            }

                            return Position;
                        case SeekOrigin.Begin:
                            currentBuffer = buffers.First;
                            currentBufferPosition = 0;
                            realPosition = 0;
                            if (offset != 0)
                            {
                                return Seek(offset, SeekOrigin.Current);
                            }

                            return 0;
                        case SeekOrigin.End:
                            realPosition = realLength;
                            currentBuffer = buffers.Last;
                            currentBufferPosition = currentBuffer.Value.Length;
                            if (offset != 0)
                            {
                                return Seek(offset, SeekOrigin.Current);
                            }

                            return realLength;
                        default: throw new NotImplementedException($"SeekOrigin {origin} undefined!");
                    }
                }
                catch
                {
                    throw new EndOfStreamException();
                }
            }
        }

        /// <summary>Throws new NotSupportedException().</summary>
        /// <param name="value">Not supported.</param>
        public override void SetLength(long value) { throw new NotSupportedException(); }

        /// <summary>Removes all buffers in front of the current position.</summary>
        /// <returns>Bytes freed.</returns>
        public virtual int FreeBuffers()
        {
            lock (this)
            {
                var bytesFreed = 0;
                while ((buffers.First != null) && (buffers.First.Value.Length <= realPosition))
                {
                    var len = buffers.First.Value.Length;
                    realPosition -= len;
                    realLength -= len;
                    buffers.RemoveFirst();
                    bytesFreed += len;
                }

                if (buffers.Count == 0)
                {
                    currentBufferPosition = 0;
                    currentBuffer = null;
                }

                return bytesFreed;
            }
        }

        /// <summary>removes all buffers in front of the current position but keeps at least the specified number of bytes.</summary>
        /// <param name="sizeToKeep">The number of bytes to keep at the buffer.</param>
        public virtual void FreeBuffers(int sizeToKeep)
        {
            lock (this)
            {
                while ((buffers.First != null) && (buffers.First.Value.Length <= realPosition))
                {
                    var len = buffers.First.Value.Length;
                    if ((Available - len) >= sizeToKeep)
                    {
                        realPosition -= len;
                        realLength -= len;
                        buffers.RemoveFirst();
                    }
                    else
                    {
                        break;
                    }
                }

                if (buffers.Count == 0)
                {
                    currentBufferPosition = 0;
                    currentBuffer = null;
                }
            }
        }

        /// <summary>Appends a byte buffer of the specified length from the specified Source stream to the end of the stream.</summary>
        /// <param name="source">The source stream.</param>
        /// <param name="count">The number of bytes to append.</param>
        /// <returns>The number of bytes written.</returns>
        public int AppendStream(Stream source, int count)
        {
            lock (this)
            {
                if (source == null)
                {
                    throw new ArgumentNullException(nameof(source));
                }

                var buffer = new byte[count];
                var result = source.Read(buffer, 0, count);
                if (result != count)
                {
                    Array.Resize(ref buffer, result);
                }

                PutBuffer(buffer);
                return result;
            }
        }

        /// <summary>Appends a whole stream to the end of the stream.</summary>
        /// <param name="source">The source stream.</param>
        /// <returns>The number of bytes written.</returns>
        public long AppendStream(Stream source)
        {
            lock (this)
            {
                if (source == null)
                {
                    throw new ArgumentNullException(nameof(source));
                }

                const int BufferSize = 1024 * 1024;
                long result = 0;
                while (true)
                {
                    var buffer = new byte[BufferSize];
                    var count = source.Read(buffer, 0, BufferSize);
                    if (count == 0)
                    {
                        break;
                    }

                    result += count;
                    if (count != BufferSize)
                    {
                        Array.Resize(ref buffer, count);
                    }

                    PutBuffer(buffer);
                }

                return result;
            }
        }

        /// <summary>Puts a buffer to the end of the stream without copying.</summary>
        /// <param name="buffer">The byte buffer to add.</param>
        public virtual void PutBuffer(byte[] buffer)
        {
            lock (this)
            {
                if (buffer == null)
                {
                    throw new ArgumentNullException(nameof(buffer));
                }

                buffers.AddLast(buffer);
                realLength += buffer.Length;
                if (currentBuffer == null)
                {
                    Seek(realPosition, SeekOrigin.Begin);
                }
            }
        }

        /// <summary>appends a buffer at the end of the stream (always copies the buffer).</summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public virtual void AppendBuffer(byte[] buffer, int offset, int count)
        {
            lock (this)
            {
                if (buffer == null)
                {
                    throw new ArgumentNullException(nameof(buffer));
                }

                if (count == 0)
                {
                    return;
                }

                var newBuffer = new byte[count];
                Array.Copy(buffer, offset, newBuffer, 0, count);
                PutBuffer(newBuffer);
            }
        }

        /// <summary>Retrieves all data at the buffer as array (peek).</summary>
        /// <returns>An array of bytes.</returns>
        public byte[] ToArray()
        {
            lock (this)
            {
                var result = new byte[Available];
                {
                    var start = 0;
                    var node = currentBuffer;
                    if (node != null)
                    {
                        var count = node.Value.Length - currentBufferPosition;
                        Array.Copy(node.Value, currentBufferPosition, result, start, count);
                        start += count;
                        node = node.Next;
                    }

                    while (node != null)
                    {
                        node.Value.CopyTo(result, start);
                        start += node.Value.Length;
                        node = node.Next;
                    }
                }
                return result;
            }
        }

        /// <summary>Clears the buffer.</summary>
        public void Clear()
        {
            lock (this)
            {
                buffers.Clear();
                realLength = 0;
                realPosition = 0;
                currentBuffer = null;
                currentBufferPosition = 0;
            }
        }
    }
}
