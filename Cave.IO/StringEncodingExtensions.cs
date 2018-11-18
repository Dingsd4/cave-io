using System.Text;

namespace Cave.IO
{
    public static class StringEncodingExtensions
    {
        public static bool IsDead(this Encoding encoding)
        {
            return encoding.CodePage >= 0xDEA0 && encoding.CodePage < 0xDF00;
        }

        public static StringEncoding ToStringEncoding(this Encoding encoding)
        {
            switch(encoding.CodePage)
            {
                case (int)StringEncoding.UTF_16: return StringEncoding.UTF16;
                case (int)StringEncoding.UTF_32: return StringEncoding.UTF32;
                case (int)StringEncoding.UTF_8: return StringEncoding.UTF8;
                case (int)StringEncoding.US_ASCII: return StringEncoding.ASCII;
                default: return (StringEncoding)encoding.CodePage;
            }
        }
    }
}
