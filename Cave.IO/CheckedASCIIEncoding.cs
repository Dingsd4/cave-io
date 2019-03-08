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
        /// <summary>
        /// Gets the code page identifier of the current Encoding.
        /// </summary>
        public override int CodePage => ASCII.CodePage;

        /// <summary>
        /// Gets the human-readable description of the current encoding.
        /// </summary>
        public override string EncodingName => ASCII.EncodingName;

#if !NETSTANDARD13
        /// <summary>
        /// Gets a name for the current encoding that can be used with mail agent body tags.
        /// </summary>
        public override string BodyName => ASCII.BodyName;
#endif

        /// <summary>
        /// Gets a value indicating whether this is a single byte character encoding.
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
