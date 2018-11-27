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
using System.IO;

namespace Cave.IO
{
    /// <summary>
    /// Provides a sub stream implementation
    /// </summary>
    public class SubStream : Stream
    {
        Stream m_Stream;
        long m_Position = 0;

        /// <summary>
        /// Creates a new SubStream from the specified stream at its current read/write position
        /// </summary>
        /// <param name="stream">The stream to create the substream from</param>
        public SubStream(Stream stream) : this(stream, 0) { }

        /// <summary>
        /// Creates a new SubStream from the specified stream at the specified read/write position
        /// </summary>
        /// <param name="stream">The stream to create the substream from</param>
        /// <param name="seek">The start position of the substream relative to the current stream position</param>
        public SubStream(Stream stream, int seek)
        {
            if (stream == null) throw new ArgumentNullException("stream");
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
                    if (seek != stream.Read(new byte[seek], 0, seek)) throw new EndOfStreamException();
                }
            }
            m_Stream = stream;
        }

        /// <summary>
        /// Obtains whether the stream can be read or not
        /// </summary>
        public override bool CanRead
        {
            get { return m_Stream.CanRead; }
        }

        /// <summary>
        /// Obtains whether the stream can seek or not
        /// </summary>
        public override bool CanSeek
        {
            get { return m_Stream.CanSeek; }
        }

        /// <summary>
        /// Obtains whether the stream can be written or not
        /// </summary>
        public override bool CanWrite
        {
            get { return m_Stream.CanWrite; }
        }

        /// <summary>
        /// flushes the stream
        /// </summary>
        public override void Flush()
        {
            m_Stream.Flush();
        }

        /// <summary>
        /// Obtains the length of the stream
        /// </summary>
        public override long Length
        {
            get { return m_Stream.Length - m_Stream.Position + m_Position; }
        }

        /// <summary>
        /// Gets/sets the current read/write position
        /// </summary>
        public override long Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Reads a byte buffer from the stream at the current position
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int l_Read = m_Stream.Read(buffer, offset, count);
            m_Position += l_Read;
            return l_Read;
        }

        /// <summary>
        /// seeks the stream
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
                    long result = Seek(offset - m_Position, origin);
                    m_Position = offset;
                    return result;

                case SeekOrigin.Current:
                    return Seek(m_Position + offset, SeekOrigin.Begin);

                default: throw new NotSupportedException(string.Format("SeekOrigin {0} not supported!", origin));
            }
        }

        /// <summary>
        /// not supported
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes a byte buffer to the stream at the current position
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            m_Stream.Write(buffer, offset, count);
            m_Position += count;
        }
    }
}
