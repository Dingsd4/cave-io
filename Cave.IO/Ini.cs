using System;

namespace Cave.IO
{
    internal static class Ini
    {
        internal static void CheckName(string value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
            for (int i = 0; i < value.Length; i++)
            {
                switch (value[i])
                {
                    case '#':
                    case '[':
                    case ']':
                        throw new ArgumentException($"Invalid name for {paramName} {value}!", paramName);
                    default:
                        if (value[i] < 32)
                        {
                            throw new ArgumentException($"Invalid name for {paramName} {value}!", paramName);
                        }
                        break;
                }
            }
        }

        internal static string Escape(string value, char boxChar)
        {
            bool box = value.IndexOfAny(new[] { boxChar, '#', ' ' }) > -1;
            value = value.EscapeUtf8();
            box |= value.IndexOf('\\') > -1 || value.Trim() != value;
            if (box)
            {
                value = value.Box(boxChar);
            }
            return value;
        }

        internal static string Unescape(string value, char boxChar)
        {
            if (value.IsBoxed(boxChar, boxChar))
            {
                value = value.Unbox(boxChar);
            }
            value = value.Unescape();
            return value;
        }
    }
}