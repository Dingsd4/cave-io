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
using System.Text;

namespace Cave.IO
{
    /// <summary>
    /// Provides a ASCIIEncoding replacement throwing errors on invalid data.
    /// The default .net class just ignores invalid characters and replaces them.
    /// </summary>
    public sealed class CheckedASCIIEncoding : Encoding
    {
        public override int CodePage => ASCII.CodePage;

        public override string EncodingName => ASCII.EncodingName;

#if !NETSTANDARD13
        public override string BodyName => ASCII.BodyName;
#endif

        /// <summary>
        /// This is a single byte character encoding
        /// </summary>
        public override bool IsSingleByte => true;

        /// <summary>
        /// Calculates the number of bytes produced by encoding a set of characters.
        /// </summary>
        /// <param name="chars">The character array containing the set of characters to encode. </param>
        /// <param name="index">The index of the first character to encode. </param>
        /// <param name="count">The number of characters to encode. </param>
        /// <returns>The number of bytes produced by encoding the specified characters.</returns>
        public override int GetByteCount(char[] chars, int index, int count)
        {
            return count;
        }

        /// <summary>
        /// Encodes a set of characters from the specified character array into the specified byte array.
        /// </summary>
        /// <param name="chars">The character array containing the set of characters to encode. </param>
        /// <param name="charIndex">The index of the first character to encode.</param>
        /// <param name="charCount">The number of characters to encode. </param>
        /// <param name="bytes">The byte array to contain the resulting sequence of bytes. </param>
        /// <param name="byteIndex">The index at which to start writing the resulting sequence of bytes. </param>
        /// <returns>The actual number of bytes written into bytes.</returns>
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            if (chars == null)
            {
                throw new ArgumentNullException("chars");
            }

            unchecked
            {
                int c = 0;
                for (; c < charCount; c++)
                {
                    uint value = chars[charIndex + c];
                    if (value > 127)
                    {
                        throw new InvalidDataException(string.Format("Character '{0}' at index '{1}' is not a valid ASCII character!", (char)value, c + charIndex));
                    }

                    bytes[byteIndex + c] = (byte)value;
                }
                return c;
            }
        }

        /// <summary>
        /// Calculates the number of characters produced by decoding a sequence of bytes from the specified byte array.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode. </param>
        /// <param name="index">The index of the first byte to decode. </param>
        /// <param name="count">The number of bytes to decode. </param>
        /// <returns>The number of characters produced by decoding the specified sequence of bytes.</returns>
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            return count;
        }

        /// <summary>
        /// Decodes a sequence of bytes from the specified byte array into the specified character array.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode. </param>
        /// <param name="byteIndex">The index of the first byte to decode. </param>
        /// <param name="byteCount">The number of bytes to decode. </param>
        /// <param name="chars">The character array to contain the resulting set of characters.</param>
        /// <param name="charIndex">The index at which to start writing the resulting set of characters. </param>
        /// <returns>The actual number of characters written into chars.</returns>
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            if (chars == null)
            {
                throw new ArgumentNullException("chars");
            }

            unchecked
            {
                int c = 0;
                for (; c < byteCount; c++)
                {
                    byte b = bytes[byteIndex + c];
                    if (b > 127)
                    {
                        throw new InvalidDataException(string.Format("Byte '{0}' at index '{1}' is not a valid ASCII character!", b, byteIndex + c));
                    }

                    chars[charIndex + c] = (char)b;
                }
                return c;
            }
        }

        /// <summary>
        /// Calculates the maximum number of bytes produced by encoding the specified number of characters.
        /// </summary>
        /// <param name="charCount">The number of characters to encode. </param>
        /// <returns>The maximum number of bytes produced by encoding the specified number of characters.</returns>
        public override int GetMaxByteCount(int charCount)
        {
            return charCount;
        }

        /// <summary>
        /// Calculates the maximum number of characters produced by decoding the specified number of bytes.
        /// </summary>
        /// <param name="byteCount">The number of bytes to decode. </param>
        /// <returns>The maximum number of characters produced by decoding the specified number of bytes.</returns>
        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount;
        }
    }
}
