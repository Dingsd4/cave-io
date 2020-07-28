using System.Text;

namespace Cave.IO
{
    /// <summary>Extensions to the <see cref="StringEncoding" /> enum.</summary>
    public static class StringEncodingExtensions
    {
        /// <summary>Returns whether the encoding is dead (true) or not (false).</summary>
        /// <param name="encoding">Encoding to check.</param>
        /// <returns>Returns true for dead encodings.</returns>
        public static bool IsDead(this Encoding encoding) => (encoding.CodePage >= 0xDEA0) && (encoding.CodePage < 0xDF00);

        /// <summary>Converts an encoding instance by codepage to the corresponding <see cref="StringEncoding" /> enum value.</summary>
        /// <param name="encoding">The encoding to convert.</param>
        /// <returns>Returns an enum value for the <see cref="Encoding.CodePage" />.</returns>
        public static StringEncoding ToStringEncoding(this Encoding encoding)
        {
            switch (encoding.CodePage)
            {
                case (int) StringEncoding.UTF_16: return StringEncoding.UTF16;
                case (int) StringEncoding.UTF_32: return StringEncoding.UTF32;
                case (int) StringEncoding.UTF_8: return StringEncoding.UTF8;
                case (int) StringEncoding.US_ASCII: return StringEncoding.ASCII;
                default: return (StringEncoding) encoding.CodePage;
            }
        }
    }
}
