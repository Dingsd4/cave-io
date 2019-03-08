using System;
using System.Diagnostics.CodeAnalysis;

namespace Cave.IO
{
    /// <summary>
    /// Provides Endian Tools.
    /// </summary>
    public static class Endian
    {
        /// <summary>Swaps the endian type of the specified data.</summary>
        /// <param name="data">The data.</param>
        /// <param name="bytes">The bytes to swap (2..x).</param>
        /// <returns>The swapped data.</returns>
        public static byte[] Swap(byte[] data, int bytes)
        {
            if (bytes < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes));
            }

            byte[] result = new byte[data.Length];
            bytes--;
            for (int i = 0; i < data.Length;)
            {
                int e = i + bytes;
                for (int n = 0; n <= bytes; n++, i++, e--)
                {
                    result[e] = data[i];
                }
            }
            return result;
        }

        /// <summary>Swaps the byte order of a value.</summary>
        /// <param name="value">Value to swap the byte order of.</param>
        /// <returns>Byte order-swapped value.</returns>
        public static ushort Swap(ushort value)
        {
            return (ushort)((value >> 8) | ((value & 0xFF) << 8));
        }

        /// <summary>Swaps the byte order of a value.</summary>
        /// <param name="value">Value to swap the byte order of.</param>
        /// <returns>Byte order-swapped value.</returns>
        public static uint Swap(uint value)
        {
            return (value >> 24) | ((value & 0xFF00) << 8) | ((value >> 8) & 0xFF00) | (value << 24);
        }

        /// <summary>Swaps the byte order of a value.</summary>
        /// <param name="value">Value to swap the byte order of.</param>
        /// <returns>Byte order-swapped value.</returns>
        public static ulong Swap(ulong value)
        {
            return (value >> 56) | (0xFF00 & (value >> 40)) | (0xFF0000 & (value >> 24)) | (0xFF000000 & (value >> 8)) |
                ((value & 0xFF000000) << 8) | ((value & 0xFF0000) << 24) | ((value & 0xFF00) << 40) | (value << 56);
        }

        /// <summary>
        /// Gets the machine endian type.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public static EndianType MachineType
        {
            get
            {
                byte[] bytes = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 };
                const ulong BigEndianValue = 0x123456789ABCDEF0;
                const ulong LittleEndianValue = 0xF0DEBC9A78563412;
                ulong value;
                unsafe
                {
                    fixed (byte* ptr = &bytes[0])
                    {
                        value = *(ulong*)ptr;
                    }
                }
                if (value == LittleEndianValue)
                {
                    return EndianType.LittleEndian;
                }

                if (value == BigEndianValue)
                {
                    return EndianType.BigEndian;
                }

                return EndianType.None;
            }
        }
    }
}
