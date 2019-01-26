using System;
using System.Collections.Generic;

namespace Cave.IO
{
    /// <summary>
    /// Provides binary conversion routines.
    /// </summary>
    public class Bits
    {
        /// <summary>
        /// Implicitly converts <see cref="Bits"/> data to an array.
        /// </summary>
        /// <param name="value">The binary data.</param>
        /// <returns></returns>
        public static implicit operator byte[](Bits value)
        {
            if (value == null)
            {
                return new byte[0];
            }

            return value.data;
        }

        /// <summary>
        /// Implicitly converts an array to <see cref="Bits"/> data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static implicit operator Bits(byte[] data)
        {
            if (data == null)
            {
                return new Bits(new byte[0]);
            }

            return new Bits(data);
        }

        /// <summary>Reflects 64 bits.</summary>
        /// <param name="x">The bits.</param>
        /// <returns>Returns a center reflection.</returns>
        public static ulong Reflect64(ulong x)
        {
            // move bits
            x = ((x & 0x5555555555555555) << 1) | ((x >> 1) & 0x5555555555555555);
            x = ((x & 0x3333333333333333) << 2) | ((x >> 2) & 0x3333333333333333);
            x = ((x & 0x0F0F0F0F0F0F0F0F) << 4) | ((x >> 4) & 0x0F0F0F0F0F0F0F0F);

            // move bytes
            x = (x << 56) | ((x & 0xFF00) << 40) | ((x & 0xFF0000) << 24) | ((x & 0xFF000000) << 8) | ((x >> 8) & 0xFF000000) | ((x >> 24) & 0xFF0000) | ((x >> 40) & 0xFF00) | (x >> 56);
            return x;
        }

        /// <summary>Reflects 32 bits.</summary>
        /// <param name="x">The bits.</param>
        /// <returns>Returns a center reflection.</returns>
        public static uint Reflect32(uint x)
        {
            // move bits
            x = ((x & 0x55555555) << 1) | ((x >> 1) & 0x55555555);
            x = ((x & 0x33333333) << 2) | ((x >> 2) & 0x33333333);
            x = ((x & 0x0F0F0F0F) << 4) | ((x >> 4) & 0x0F0F0F0F);

            // move bytes
            x = (x << 24) | ((x & 0xFF00) << 8) | ((x >> 8) & 0xFF00) | (x >> 24);
            return x;
        }

        /// <summary>Reflects 8 bits.</summary>
        /// <param name="b">The bits.</param>
        /// <returns>Returns a center reflection.</returns>
        public static byte Reflect8(byte b)
        {
            uint r = b;
            r = ((r & 0x55) << 1) | ((r >> 1) & 0x55);
            r = ((r & 0x33) << 2) | ((r >> 2) & 0x33);
            r = ((r & 0x0F) << 4) | ((r >> 4) & 0x0F);
            return (byte)r;
        }

        /// <summary>
        /// Converts a binary value (100110101) to a "normal" value (0x135 = 309).
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static byte ToByte(long binary)
        {
            return (byte)ToInt32(binary);
        }

        /// <summary>
        /// Converts a binary value (100110101) to a "normal" value (0x135 = 309).
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static sbyte ToSByte(long binary)
        {
            return (sbyte)ToInt32(binary);
        }

        /// <summary>
        /// Converts a binary value (100110101) to a "normal" value (0x135 = 309).
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static short ToInt16(long binary)
        {
            return (short)ToInt32(binary);
        }

        /// <summary>
        /// Converts a binary value (100110101) to a "normal" value (0x135 = 309).
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static ushort ToUInt16(long binary)
        {
            return (ushort)ToInt32(binary);
        }

        /// <summary>
        /// Converts a binary value (100110101) to a "normal" value (0x135 = 309).
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static uint ToUInt32(long binary)
        {
            return (uint)ToInt32(binary);
        }

        /// <summary>
        /// Converts a binary value (100110101) to a "normal" int (0x135 = 309).
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static int ToInt32(long binary)
        {
            int result = 0;
            int counter = 0;
            while (binary > 0)
            {
                int current = (int)(binary % 10);
                binary = binary / 10;
                if (current > 1)
                {
                    throw new ArgumentException("binary");
                }

                result |= current << counter++;
            }
            return result;
        }

        /// <summary>
        /// Converts a binary value (100110101) to a "normal" value (0x135 = 309).
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static byte ToByte(string binary)
        {
            return (byte)ToInt64(binary);
        }

        /// <summary>
        /// Converts a binary value (100110101) to a "normal" value (0x135 = 309).
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static sbyte ToSByte(string binary)
        {
            return (sbyte)ToInt64(binary);
        }

        /// <summary>
        /// Converts a binary value (100110101) to a "normal" value (0x135 = 309).
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static short ToInt16(string binary)
        {
            return (short)ToInt64(binary);
        }

        /// <summary>
        /// Converts a binary value (100110101) to a "normal" value (0x135 = 309).
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static ushort ToUInt16(string binary)
        {
            return (ushort)ToInt64(binary);
        }

        /// <summary>
        /// Converts a binary value (100110101) to a "normal" value (0x135 = 309).
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static ushort ToUInt32(string binary)
        {
            return (ushort)ToInt64(binary);
        }

        /// <summary>
        /// Converts a binary value (100110101) to a "normal" value (0x135 = 309).
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static int ToInt32(string binary)
        {
            return (int)ToInt64(binary);
        }

        /// <summary>
        /// Converts a binary string ("100110101") to a "normal" int (0x135 = 309).
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static long ToInt64(string binary)
        {
            if (binary == null)
            {
                throw new ArgumentNullException("binary");
            }

            if (binary.Length > 63)
            {
                throw new ArgumentException("binary");
            }

            long result = 0;
            foreach (char c in binary)
            {
                switch (c)
                {
                    case '0':
                        result = result << 1;
                        break;
                    case '1':
                        result = (result << 1) | 1;
                        break;
                    default: throw new ArgumentException("binary");
                }
            }
            return result;
        }

        /// <summary>
        /// Converts a value int (309 = 0x135) to a binary string ("100110101").
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToString(int value)
        {
            List<char> result = new List<char>();
            while (value != 0)
            {
                if ((value & 1) == 0)
                {
                    result.Add('0');
                }
                else
                {
                    result.Add('1');
                }

                value >>= 1;
            }
            result.Reverse();
            return new string(result.ToArray());
        }

        /// <summary>
        /// Converts a value int (309 = 0x135) to a binary long (100110101).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToBinary(int value)
        {
            long result = 0;
            int counter = 0;
            while (value != 0)
            {
                long bit = (value & 1) << counter++;
                result = result | bit;
                value >>= 1;
            }
            return result;
        }

        byte[] data;

        /// <summary>
        /// Creates a new instance with the specified data.
        /// </summary>
        /// <param name="data"></param>
        public Bits(byte[] data)
        {
            this.data = data;
        }

        /// <summary>
        /// Obtains a copy of all data.
        /// </summary>
        public byte[] Data => (byte[])data.Clone();
    }
}
