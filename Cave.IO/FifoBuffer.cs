using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Cave.IO
{
    /// <summary>
    /// Provides a simple byte[] buffer queue able to work with buffers of any size
    /// </summary>
    public sealed class FifoBuffer
    {
        /// <summary>
        /// Reads a byte array of specified length from the source pointer starting at the specified byte offset
        /// </summary>
        /// <param name="source">The source pointer</param>
        /// <param name="offset">The byte offset for the read position</param>
        /// <param name="count">The number of bytes to read</param>
        /// <returns></returns>
        public static byte[] Read(IntPtr source, int offset, int count)
        {
            IntPtr ptr = (offset == 0) ? source : new IntPtr(source.ToInt64() + offset);
            byte[] buffer = new byte[count];
            Marshal.Copy(ptr, buffer, 0, count);
            return buffer;
        }

        LinkedList<byte[]> m_Buffer = new LinkedList<byte[]>();
        int m_Length = 0;

        /// <summary>
        /// Directly prepends the specified byte buffer (without copying)
        /// </summary>
        /// <param name="buffer">The buffer to add (will not be copied)</param>
        public void Prepend(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            m_Buffer.AddFirst(buffer);
            m_Length += buffer.Length;
        }

        /// <summary>
        /// Enqueues a number of bytes from the specified stream
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        /// <param name="count">The number of bytes to enqueue</param>
        public void Enqueue(Stream stream, int count)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            byte[] buffer = new byte[count];
            int len = stream.Read(buffer, 0, count);
            if (len == count)
            {
                Enqueue(buffer);
            }
            else
            {
                Enqueue(buffer, 0, len);
            }
        }

        /// <summary>
        /// Directly enqueues the specified byte buffer (without copying)
        /// </summary>
        /// <param name="buffer">The buffer to add (will not be copied)</param>
        public void Enqueue(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            m_Buffer.AddLast(buffer);
            m_Length += buffer.Length;
        }

        /// <summary>
        /// Enqueues datas from the specified buffer (will be copied)
        /// </summary>
        /// <param name="buffer">The buffer</param>
        /// <param name="offset">The byte offset to start reading from</param>
        /// <param name="count">The number of bytes to copy</param>
        public void Enqueue(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            byte[] newBuffer = new byte[count];
            Array.Copy(buffer, offset, newBuffer, 0, count);
            Enqueue(newBuffer);
        }

        /// <summary>
        /// Enqueues data from the specified pointer
        /// </summary>
        /// <param name="ptr">The location to start reading at</param>
        /// <param name="count">The number of bytes to copy</param>
        public void Enqueue(IntPtr ptr, int count)
        {
            Enqueue(Read(ptr, 0, count));
        }

        /// <summary>
        /// Enqueues data from the specified pointer
        /// </summary>
        /// <param name="ptr">The location to start reading at</param>
        /// <param name="offset">The byte offset to start reading from</param>
        /// <param name="count">The number of bytes to copy</param>
        public void Enqueue(IntPtr ptr, int offset, int count)
        {
            Enqueue(Read(ptr, offset, count));
        }

        /// <summary>
        /// Peeks at the first buffer (may be of any size &gt; 0)
        /// </summary>
        /// <returns>Returns the first buffer (may be of any size &gt; 0)</returns>
        public byte[] Peek()
        {
            return m_Buffer.First.Value;
        }

        /// <summary>
        /// Peeks at the buffer returning the specified number of bytes as new byte[] buffer
        /// </summary>
        /// <param name="size">The number of bytes to peek at</param>
        /// <returns>Returns a new buffer of the specified size</returns>
        public byte[] Peek(int size)
        {
            if (Length < size)
            {
                throw new EndOfStreamException();
            }

            byte[] result = new byte[size];
            int pos = 0;
            LinkedListNode<byte[]> node = m_Buffer.First;
            while (pos < size)
            {
                byte[] current = node.Value;
                node = node.Next;
                int len = Math.Min(current.Length, size - pos);
                Array.Copy(current, 0, result, pos, len);
                pos += len;
            }
            return result;
        }

        /// <summary>
        /// Peeks at the buffer and writes the data to the specified location
        /// </summary>
        /// <param name="size">The number of bytes to peek at</param>
        /// <param name="ptr">The location to start writing at</param>
        public void Peek(int size, IntPtr ptr)
        {
            if (Length < size)
            {
                throw new EndOfStreamException();
            }

            int pos = 0;
            LinkedListNode<byte[]> node = m_Buffer.First;
            while (pos < size)
            {
                byte[] current = node.Value;
                node = node.Next;
                int len = Math.Min(current.Length, size - pos);
                Marshal.Copy(current, 0, ptr, len);
                ptr = new IntPtr(len + ptr.ToInt64());
                pos += len;
            }
        }

        /// <summary>
        /// Dequeues the first buffer (may be of any size &gt; 0)
        /// </summary>
        /// <returns>Returns a dequeued buffer (may be of any size &gt; 0)</returns>
        public byte[] Dequeue()
        {
            byte[] buffer = m_Buffer.First.Value;
            m_Buffer.RemoveFirst();
            m_Length -= buffer.Length;
            return buffer;
        }

        /// <summary>
        /// Dequeues the specified number of bytes as new byte[] buffer
        /// </summary>
        /// <param name="size">The number of bytes to dequeue</param>
        /// <returns>Returns a dequeued buffer of the specified size</returns>
        public byte[] Dequeue(int size)
        {
            if (Length < size)
            {
                throw new EndOfStreamException();
            }

            byte[] result = new byte[size];
            int pos = 0;
            while (pos < size)
            {
                byte[] current = Dequeue();
                int len = Math.Min(current.Length, size - pos);
                Array.Copy(current, 0, result, pos, len);
                pos += len;
                if (len < current.Length)
                {
                    byte[] remainder = new byte[current.Length - len];
                    Array.Copy(current, len, remainder, 0, remainder.Length);
                    m_Buffer.AddFirst(remainder);
                    m_Length += remainder.Length;
                }
            }
            return result;
        }

        /// <summary>
        /// Dequeues the specified number of bytes and writes them to the specified location
        /// </summary>
        /// <param name="size">The number of bytes to dequeue</param>
        /// <param name="ptr">The location to start writing at</param>
        public void Dequeue(int size, IntPtr ptr)
        {
            if (Length < size)
            {
                throw new EndOfStreamException();
            }

            int pos = 0;
            while (pos < size)
            {
                byte[] current = Dequeue();
                int len = Math.Min(current.Length, size - pos);
                Marshal.Copy(current, 0, ptr, len);
                ptr = new IntPtr(len + ptr.ToInt64());
                pos += len;
                if (len < current.Length)
                {
                    byte[] remainder = new byte[current.Length - len];
                    Array.Copy(current, len, remainder, 0, remainder.Length);
                    m_Buffer.AddFirst(remainder);
                    m_Length += remainder.Length;
                }
            }
        }

        /// <summary>
        /// Obtains a byte[] array containing all currently buffered bytes
        /// </summary>
        /// <returns>Returns a byte[] array size = <see cref="Length"/></returns>
        public byte[] ToArray()
        {
            byte[] result = new byte[Length];
            int pos = 0;
            foreach (byte[] buffer in m_Buffer)
            {
                Array.Copy(buffer, 0, result, pos, buffer.Length);
                pos += buffer.Length;
            }
            return result;
        }

        /// <summary>
        /// Clears the buffer
        /// </summary>
        public void Clear()
        {
            m_Buffer.Clear();
            m_Length = 0;
        }

        /// <summary>
        /// Obtains number of bytes currently buffered
        /// </summary>
        public int Length => m_Length;
    }
}
