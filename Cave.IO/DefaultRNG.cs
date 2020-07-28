using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

#pragma warning disable CA1720

namespace Cave.IO
{
    /// <summary>Provides a globally used random number generator instance.</summary>
    [ComVisible(false)]
    public static class DefaultRNG
    {
        static RandomNumberGenerator generator = RandomNumberGenerator.Create();

        /// <summary>Gets or sets the currently used generator.</summary>
        public static RandomNumberGenerator Generator { get => generator; set => generator = value ?? throw new ArgumentNullException(nameof(value)); }

        /// <summary>Gets a random 8 bit signed integer.</summary>
        public static sbyte Int8 => (sbyte) Get(1)[0];

        /// <summary>Gets a random 8 bit unsigned integer.</summary>
        public static byte UInt8 => Get(1)[0];

        /// <summary>Gets a random 16 bit signed integer.</summary>
        public static short Int16 => BitConverter.ToInt16(Get(2), 0);

        /// <summary>Gets a random 16 bit unsigned integer.</summary>
        public static ushort UInt16 => BitConverter.ToUInt16(Get(2), 0);

        /// <summary>Gets a random 32 bit signed integer.</summary>
        public static int Int32 => BitConverter.ToInt32(Get(4), 0);

        /// <summary>Gets a random 32 bit unsigned integer.</summary>
        public static uint UInt32 => BitConverter.ToUInt32(Get(4), 0);

        /// <summary>Gets a random 64 bit signed integer.</summary>
        public static long Int64 => BitConverter.ToInt64(Get(8), 0);

        /// <summary>Gets a random 64 bit unsigned integer.</summary>
        public static ulong UInt64 => BitConverter.ToUInt64(Get(8), 0);

        /// <summary>Fills the specified array with random data.</summary>
        /// <param name="array">The byte array to fill.</param>
        public static void Fill(byte[] array) { Generator.GetBytes(array); }

        /// <summary>Gets a byte array containing secure random bytes with the specified size.</summary>
        /// <param name="size">The size in bytes.</param>
        /// <returns>Returns a new randomized byte array.</returns>
        public static byte[] Get(int size)
        {
            var array = new byte[size];
            Fill(array);
            return array;
        }

        /// <summary>Creates a random password using ascii printable characters.</summary>
        /// <param name="count">Length of the desired password.</param>
        /// <param name="characters">The characters.</param>
        /// <returns>The password string.</returns>
        public static string GetPassword(int count, string characters = null)
        {
            var result = new char[count];
            var value = UInt32;
            char[] chars;
            if (characters != null)
            {
                chars = characters.ToCharArray();
            }
            else
            {
                chars = new char[126 - 32 - 2];
                for (int x = 33, n = 0; n < chars.Length; x++)
                {
                    var c = (char) x;
                    if (c == '"')
                    {
                        continue;
                    }

                    if (c == '\'')
                    {
                        continue;
                    }

                    chars[n++] = c;
                }
            }

            var charsCount = (uint) chars.Length;
            var i = 0;
            while (i < count)
            {
                if (value < chars.Length)
                {
                    value ^= UInt32;
                }

                var index = value % charsCount;
                result[i++] = chars[index];
                value /= charsCount;
            }

            return new string(result);
        }
    }
}

#pragma warning restore CA1720
