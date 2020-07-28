using System;
using System.Collections.Generic;

namespace Cave.IO
{
    /// <summary>Provides binary conversion routines.</summary>
    public class Bits
    {
        /// <summary>Initializes a new instance of the <see cref="Bits" /> class.</summary>
        /// <param name="data">Binary data to initialize.</param>
        public Bits(byte[] data) => Data = data;

        /// <summary>Gets a copy of all data.</summary>
        public IList<byte> Data { get; }

        /// <summary>Implicitly converts <see cref="Bits" /> data to an array.</summary>
        /// <param name="value">The binary data.</param>
        public static implicit operator byte[](Bits value) => value?.Data as byte[] ?? new byte[0];

        /// <summary>Implicitly converts an array to <see cref="Bits" /> data.</summary>
        /// <param name="data">The binary data.</param>
        public static implicit operator Bits(byte[] data) => data == null ? new Bits(new byte[0]) : new Bits(data);

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
            x = (x << 56) | ((x & 0xFF00) << 40) | ((x & 0xFF0000) << 24) | ((x & 0xFF000000) << 8) | ((x >> 8) & 0xFF000000) | ((x >> 24) & 0xFF0000) |
                ((x >> 40) & 0xFF00) | (x >> 56);
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
            return (byte) r;
        }

        /// <summary>Converts a binary value (100110101) to a "normal" value (0x135 = 309).</summary>
        /// <param name="binary">The binary value.</param>
        /// <returns>The value as Byte.</returns>
        public static byte ToByte(long binary) => (byte) ToInt32(binary);

        /// <summary>Converts a binary value (100110101) to a "normal" value (0x135 = 309).</summary>
        /// <param name="binary">The binary value.</param>
        /// <returns>The value as Sbyte.</returns>
        public static sbyte ToSByte(long binary) => (sbyte) ToInt32(binary);

        /// <summary>Converts a binary value (100110101) to a "normal" value (0x135 = 309).</summary>
        /// <param name="binary">The binary value.</param>
        /// <returns>The value as Int16.</returns>
        public static short ToInt16(long binary) => (short) ToInt32(binary);

        /// <summary>Converts a binary value (100110101) to a "normal" value (0x135 = 309).</summary>
        /// <param name="binary">The binary value.</param>
        /// <returns>The value as UInt16.</returns>
        public static ushort ToUInt16(long binary) => (ushort) ToInt32(binary);

        /// <summary>Converts a binary value (100110101) to a "normal" value (0x135 = 309).</summary>
        /// <param name="binary">The binary value.</param>
        /// <returns>The value as UInt32.</returns>
        public static uint ToUInt32(long binary) => (uint) ToInt32(binary);

        /// <summary>Converts a binary value (100110101) to a "normal" int (0x135 = 309).</summary>
        /// <param name="binary">The binary value.</param>
        /// <returns>The value as Int32.</returns>
        public static int ToInt32(long binary)
        {
            var result = 0;
            var counter = 0;
            while (binary > 0)
            {
                var current = (int) (binary % 10);
                binary /= 10;
                if (current > 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(binary));
                }

                result |= current << counter++;
            }

            return result;
        }

        /// <summary>Converts a binary value (100110101) to a "normal" value (0x135 = 309).</summary>
        /// <param name="binary">The binary value.</param>
        /// <returns>The value as Byte.</returns>
        public static byte ToByte(string binary) => (byte) ToInt64(binary);

        /// <summary>Converts a binary value (100110101) to a "normal" value (0x135 = 309).</summary>
        /// <param name="binary">The binary value.</param>
        /// <returns>The value as SByte.</returns>
        public static sbyte ToSByte(string binary) => (sbyte) ToInt64(binary);

        /// <summary>Converts a binary value (100110101) to a "normal" value (0x135 = 309).</summary>
        /// <param name="binary">The binary value.</param>
        /// <returns>The value as Int16.</returns>
        public static short ToInt16(string binary) => (short) ToInt64(binary);

        /// <summary>Converts a binary value (100110101) to a "normal" value (0x135 = 309).</summary>
        /// <param name="binary">The binary value.</param>
        /// <returns>The value as UInt16.</returns>
        public static ushort ToUInt16(string binary) => (ushort) ToInt64(binary);

        /// <summary>Converts a binary value (100110101) to a "normal" value (0x135 = 309).</summary>
        /// <param name="binary">The binary value.</param>
        /// <returns>The value as UInt32.</returns>
        public static ushort ToUInt32(string binary) => (ushort) ToInt64(binary);

        /// <summary>Converts a binary value (100110101) to a "normal" value (0x135 = 309).</summary>
        /// <param name="binary">The binary value.</param>
        /// <returns>The value as UInt32.</returns>
        public static int ToInt32(string binary) => (int) ToInt64(binary);

        /// <summary>Converts a binary string ("100110101") to a "normal" int (0x135 = 309).</summary>
        /// <param name="binary">The binary value.</param>
        /// <returns>The value as Int64.</returns>
        public static long ToInt64(string binary)
        {
            if (binary == null)
            {
                throw new ArgumentNullException(nameof(binary));
            }

            if (binary.Length > 63)
            {
                throw new ArgumentOutOfRangeException(nameof(binary));
            }

            long result = 0;
            foreach (var c in binary)
            {
                switch (c)
                {
                    case '0':
                        result <<= 1;
                        break;
                    case '1':
                        result = (result << 1) | 1;
                        break;
                    default: throw new ArgumentOutOfRangeException(nameof(binary));
                }
            }

            return result;
        }

        /// <summary>Converts a value int (309 = 0x135) to a binary string ("100110101").</summary>
        /// <param name="value">The binary value.</param>
        /// <returns>The value as binary string.</returns>
        public static string ToString(int value)
        {
            var result = new List<char>();
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

        /// <summary>Converts a value int (309 = 0x135) to a binary long (100110101).</summary>
        /// <param name="value">The binary value as int.</param>
        /// <returns>The value as binary long.</returns>
        public static long ToBinary(int value)
        {
            long result = 0;
            var counter = 0;
            while (value != 0)
            {
                long bit = (value & 1) << counter++;
                result |= bit;
                value >>= 1;
            }

            return result;
        }
    }
}
