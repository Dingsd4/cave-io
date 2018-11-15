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
using System.Collections.Generic;
using System.IO;

namespace Cave.IO
{
	/// <summary>
	/// Provides a fifo buffer for byte[] blocks readable as stream.
	/// New buffers can be appended to the end of the stream and read like a stream.
	/// The performance of this class is best with medium sized buffers (1kiB - 64kiB)
	/// </summary>
	public class FifoStream : Stream
	{
        int m_RealLength = 0;
        int m_RealPosition = 0;
        int m_CurrentBufferPosition = 0;
        LinkedListNode<byte[]> m_CurrentBuffer = null;
        LinkedList<byte[]> m_Buffers = new LinkedList<byte[]>();

        /// <summary>
        /// Creates a new empty FifoByteBuffer
        /// </summary>
        public FifoStream() { }

        /// <summary>
        /// This stream can always be read
        /// </summary>
        public override bool CanRead { get { return true; } }

        /// <summary>
        /// This stream can seek
        /// </summary>
        public override bool CanSeek { get { return true; } }

        /// <summary>
        /// This stream can not be written
        /// </summary>
        public override bool CanWrite { get { return false; } }

        /// <summary>
        /// Does nothing
        /// </summary>
        public override void Flush() { }

        /// <summary>
        /// provides the current length of the stream
        /// </summary>
        public override long Length { get { return m_RealLength; } }

        /// <summary>
        /// provides the current read/write position
        /// </summary>
        public override long Position
        {
            get
            {
                return m_RealPosition;
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Provides the number of bytes available from the current read position to the end of the stream.
        /// </summary>
        public virtual int Available
        {
            get
            {
                lock (this)
                {
                    return m_RealLength - m_RealPosition;
                }
            }
        }

        /// <summary>Determines whether the buffer contains the specified byte.</summary>
		/// <param name="b">The byte.</param>
		/// <returns>the index (a value &gt;=0) if the buffer contains the specified byte; otherwise, -1.</returns>
		public int IndexOf(byte b)
        {
            lock (this)
            {
                int index = 0;
                var node = m_CurrentBuffer;
                int pos = m_CurrentBufferPosition;
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
                var node = m_CurrentBuffer;
                int pos = m_CurrentBufferPosition;
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
            lock (this)
            {
                int index = 0;
                int checkIndex = 0;
                var node = m_CurrentBuffer;
                int pos = m_CurrentBufferPosition;
                while (node != null)
                {
                    for (; pos < node.Value.Length; pos++, index++)
                    {
                        if (node.Value[pos] == data[checkIndex])
                        {
                            if (++checkIndex == data.Length)
                            {
                                return index - checkIndex + 1;
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
            lock (this)
            {
                int checkIndex = 0;
                var node = m_CurrentBuffer;
                int pos = m_CurrentBufferPosition;
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

		/// <summary>
		/// Peeks at the next byte in the buffer. Returns -1 if no more data available.
		/// </summary>
		/// <returns></returns>
		public virtual int PeekByte()
        {
            lock (this)
            {
                if (m_CurrentBuffer == null)
                {
                    return -1;
                }

                return m_CurrentBuffer.Value[m_CurrentBufferPosition];
            }
        }

        /// <summary>
        /// Reads the next byte in the buffer (much faster than <see cref="Read"/>). Returns -1 if no more data available.
        /// </summary>
        /// <returns></returns>
        public override int ReadByte()
        {
            lock (this)
            {
                int result = PeekByte();
                if (result > -1)
                {
                    Seek(1, SeekOrigin.Current);
                }

                return result;
            }
        }

        /// <summary>
        /// Reads some bytes at the current position from the stream
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (this)
            {
                count = Math.Min(count, Available);
                int resultSize = 0;
                while (count > 0 && m_CurrentBuffer != null)
                {
                    byte[] currentBuffer = m_CurrentBuffer.Value;
                    int blockSize = Math.Min(currentBuffer.Length - m_CurrentBufferPosition, count);
                    Array.Copy(currentBuffer, m_CurrentBufferPosition, buffer, offset, blockSize);
                    resultSize += blockSize;
                    count -= blockSize;
                    offset += blockSize;
                    m_CurrentBufferPosition += blockSize;
                    m_RealPosition += blockSize;
                    if (m_CurrentBufferPosition == currentBuffer.Length)
                    {
                        m_CurrentBufferPosition = 0;
                        m_CurrentBuffer = m_CurrentBuffer.Next;
                    }
                }
                return resultSize;
            }
        }

        /// <summary>
        /// Throws a NotSupportedException
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Moves the read / write position in the stream
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            lock (this)
            {
                try
                {
                    switch (origin)
                    {
                        case SeekOrigin.Current:
                            if ((m_RealPosition + offset > m_RealLength) ||
                                (m_RealPosition + offset < 0) ||
                                (m_CurrentBuffer == null))
                            {
                                throw new ArgumentOutOfRangeException(nameof(offset));
                            }

                            m_RealPosition += (int)offset;
                            offset += m_CurrentBufferPosition;
                            m_CurrentBufferPosition = 0;
                            while (offset < 0)
                            {
                                m_CurrentBuffer = m_CurrentBuffer.Previous;
                                offset += m_CurrentBuffer.Value.Length;
                            }
                            if (offset > 0)
                            {
                                while ((m_CurrentBuffer != null) && (offset >= m_CurrentBuffer.Value.Length))
                                {
                                    offset -= m_CurrentBuffer.Value.Length;
                                    m_CurrentBuffer = m_CurrentBuffer.Next;
                                }
                                m_CurrentBufferPosition = (int)offset;
                                if ((m_CurrentBufferPosition > 0) && (m_CurrentBuffer == null))
                                {
                                    throw new EndOfStreamException();
                                }
                            }
                            return Position;
                        case SeekOrigin.Begin:
                            m_CurrentBuffer = m_Buffers.First;
                            m_CurrentBufferPosition = 0;
                            m_RealPosition = 0;
                            if (offset != 0)
                            {
                                return Seek(offset, SeekOrigin.Current);
                            }

                            return 0;
                        case SeekOrigin.End:
                            m_RealPosition = m_RealLength;
                            m_CurrentBuffer = m_Buffers.Last;
                            m_CurrentBufferPosition = m_CurrentBuffer.Value.Length;
                            if (offset != 0)
                            {
                                return Seek(offset, SeekOrigin.Current);
                            }

                            return m_RealLength;
                        default: throw new NotImplementedException(string.Format("SeekOrigin {0} undefined!", origin));
                    }
                }
                catch
                {
                    throw new EndOfStreamException();
                }
            }
        }

        /// <summary>
        /// Throws new NotSupportedException()
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Obtains the number of buffers in the stream
        /// </summary>
        public int BufferCount
        {
            get
            {
                lock (this)
                {
                    return m_Buffers.Count;
                }
            }
        }

        /// <summary>
        /// Removes all buffers in front of the current position
        /// </summary>
        public virtual int FreeBuffers()
        {
            lock (this)
            {
                int bytesFreed = 0;
                while ((m_Buffers.First != null) && (m_Buffers.First.Value.Length <= m_RealPosition))
                {
                    int len = m_Buffers.First.Value.Length;
                    m_RealPosition -= len;
                    m_RealLength -= len;
                    m_Buffers.RemoveFirst();
                    bytesFreed += len;
                }
                if (m_Buffers.Count == 0)
                {
                    m_CurrentBufferPosition = 0;
                    m_CurrentBuffer = null;
                }
                else
                {
                    //check first buffer
                }
                return bytesFreed;
            }
        }

        /// <summary>
        /// removes all buffers in front of the current position but keeps at least the specified number of bytes
        /// </summary>
        /// <param name="sizeToKeep">The number of bytes to keep at the buffer</param>
        public virtual void FreeBuffers(int sizeToKeep)
        {
            lock (this)
            {
                while ((m_Buffers.First != null) && (m_Buffers.First.Value.Length <= m_RealPosition))
                {
                    int len = m_Buffers.First.Value.Length;
                    if (Available - len >= sizeToKeep)
                    {
                        m_RealPosition -= len;
                        m_RealLength -= len;
                        m_Buffers.RemoveFirst();
                    }
                    else
                    {
                        break;
                    }
                }
                if (m_Buffers.Count == 0)
                {
                    m_CurrentBufferPosition = 0;
                    m_CurrentBuffer = null;
                }
            }
        }

        /// <summary>
        /// Appends a byte buffer of the specified length from the specified Source stream to the end of the stream
        /// </summary>
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int AppendStream(Stream source, int count)
        {
            lock (this)
            {
                if (source == null)
                {
                    throw new ArgumentNullException("source");
                }

                byte[] buffer = new byte[count];
                int result = source.Read(buffer, 0, count);
                if (result != count)
                {
                    Array.Resize(ref buffer, result);
                }
                PutBuffer(buffer);
                return result;
            }
        }

        /// <summary>
        /// Appends a whole stream to the end of the stream
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public long AppendStream(Stream source)
        {
            lock (this)
            {
                if (source == null)
                {
                    throw new ArgumentNullException("source");
                }

                const int BufferSize = 1024 * 1024;
                long result = 0;
                while (true)
                {
                    byte[] buffer = new byte[BufferSize];
                    int count = source.Read(buffer, 0, BufferSize);
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

        /// <summary>
        /// Puts a buffer to the end of the stream without copying
        /// </summary>
        /// <param name="buffer"></param>
        public virtual void PutBuffer(byte[] buffer)
        {
            lock (this)
            {
                if (buffer == null)
                {
                    throw new ArgumentNullException("buffer");
                }

                m_Buffers.AddLast(buffer);
                m_RealLength += buffer.Length;
                if (m_CurrentBuffer == null)
                {
                    Seek(m_RealPosition, SeekOrigin.Begin);
                }
            }
        }

        /// <summary>
        /// appends a buffer at the end of the stream (always copies the buffer)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public virtual void AppendBuffer(byte[] buffer, int offset, int count)
        {
            lock (this)
            {
                if (buffer == null)
                {
                    throw new ArgumentNullException("buffer");
                }

                if (count == 0)
                {
                    return;
                }

                byte[] newBuffer = new byte[count];
                Array.Copy(buffer, offset, newBuffer, 0, count);
                PutBuffer(newBuffer);
            }
        }

		/// <summary>Retrieves all data at the buffer as array (peek).</summary>
		/// <returns></returns>
		public byte[] ToArray()
		{
            lock (this)
            {
                byte[] result = new byte[Available];
                {
                    int start = 0;
                    var node = m_CurrentBuffer;
                    if (node != null)
                    {
                        int count = node.Value.Length - m_CurrentBufferPosition;
                        Array.Copy(node.Value, m_CurrentBufferPosition, result, start, count);
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

        /// <summary>
        /// Clears the buffer
        /// </summary>
        public void Clear()
        {
            lock (this)
            {
                m_Buffers.Clear();
                m_RealLength = 0;
                m_RealPosition = 0;
                m_CurrentBuffer = null;
                m_CurrentBufferPosition = 0;
            }
        }
    }
}
