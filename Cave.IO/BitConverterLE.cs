using System;

namespace Cave.IO
{
    /// <summary>
    /// Provides an alternate <see cref="BitConverter" /> class providing additional functionality.
    /// </summary>
    public class BitConverterLE : BitConverterBase
    {
        /// <summary>Gets the default instance.</summary>
        /// <value>The default instance.</value>
        public static BitConverterLE Instance { get; } = new BitConverterLE();

        private BitConverterLE()
        {
        }

        #region public GetBytes() members

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override byte[] GetBytes(ushort value)
        {
            return unchecked(new byte[] { (byte)(value % 256), (byte)(value / 256) });
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override byte[] GetBytes(uint value)
        {
            byte[] result = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                result[i] = (byte)(value % 256);
                value /= 256;
            }
            return result;
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override byte[] GetBytes(ulong value)
        {
            byte[] result = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                result[i] = (byte)(value % 256);
                value /= 256;
            }
            return result;
        }

        #endregion

        #region public ToXXX() members

        /// <summary>
        /// Returns a value converted from the specified data at a specified index.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override ushort ToUInt16(byte[] data, int index)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (index < 0 || index >= data.Length - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return unchecked((ushort)(data[index] + (data[index + 1] * 256)));
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override uint ToUInt32(byte[] data, int index)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            uint result = data[index];
            uint multiplier = 1;
            for (int i = 1; i < 4; i++)
            {
                multiplier *= 256;
                result += data[index + i] * multiplier;
            }
            return result;
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override ulong ToUInt64(byte[] data, int index)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            ulong result = data[index];
            ulong multiplier = 1;
            for (int i = 1; i < 8; i++)
            {
                multiplier *= 256;
                result += data[index + i] * multiplier;
            }
            return result;
        }

        #endregion
    }
}
