using System;
using System.Collections.Generic;

namespace Cave.IO
{
    /// <summary>
    /// Provides a base class for bit converters.
    /// </summary>
    /// <seealso cref="IBitConverter" />
    public abstract class BitConverterBase : IBitConverter
    {
        #region public GetBytes() members

        /// <summary>Gets the bytes of a 7 bit encoded integer.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        public byte[] Get7BitEncodedBytes(ulong value)
        {
            int index = 0;
            byte[] result = new byte[10];
            byte b = (byte)(value % 128);
            do
            {
                value /= 128;
                if (value != 0)
                {
                    b |= 0x80;
                }

                result[index++] = b;
                b = (byte)(value % 128);
            }
            while (value != 0);
            Array.Resize(ref result, index);
            return result;
        }

        /// <summary>
        /// Gets the bytes of a 7 bit encoded integer.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        public byte[] Get7BitEncodedBytes(long value)
        {
            return Get7BitEncodedBytes(unchecked((ulong)value));
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        public byte[] GetBytes(bool value)
        {
            if (value)
            {
                return new byte[] { 1 };
            }
            else
            {
                return new byte[] { 0 };
            }
        }

        /// <summary>
        /// Gets the specified value as byte array with the specified endiantype.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        public byte[] GetBytes(byte value)
        {
            return unchecked(new byte[] { value });
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        public byte[] GetBytes(sbyte value)
        {
            return unchecked(new byte[] { (byte)value });
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        public byte[] GetBytes(short value)
        {
            return unchecked(GetBytes((ushort)value));
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        public byte[] GetBytes(int value)
        {
            return unchecked(GetBytes((uint)value));
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        public byte[] GetBytes(long value)
        {
            return unchecked(GetBytes((ulong)value));
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        public byte[] GetBytes(float value)
        {
            return GetBytes(SingleStruct.ToUInt32(value));
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        public byte[] GetBytes(double value)
        {
            return GetBytes(DoubleStruct.ToUInt64(value));
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        public byte[] GetBytes(DateTime value)
        {
            return GetBytes(value.Ticks);
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        public byte[] GetBytes(TimeSpan value)
        {
            return GetBytes(value.Ticks);
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        public byte[] GetBytes(decimal value)
        {
            int[] array = decimal.GetBits(value);
            List<byte> result = new List<byte>(16);
            foreach (int i in array)
            {
                result.AddRange(GetBytes(i));
            }
            return result.ToArray();
        }

        #endregion

        #region public ToXXX() members

        /// <summary>
        /// Returns a value converted from the specified data at a specified index.
        /// </summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        public bool ToBoolean(byte[] data, int index)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            return data[index] != 0;
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index.
        /// </summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        public byte ToByte(byte[] data, int index)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            return data[index];
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index.
        /// </summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        public sbyte ToSByte(byte[] data, int index)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            return unchecked((sbyte)data[index]);
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index.
        /// </summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        public short ToInt16(byte[] data, int index)
        {
            return unchecked((short)ToUInt16(data, index));
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index.
        /// </summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        public int ToInt32(byte[] data, int index)
        {
            return unchecked((int)ToUInt32(data, index));
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index.
        /// </summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        public long ToInt64(byte[] data, int index)
        {
            return unchecked((long)ToUInt64(data, index));
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index.
        /// </summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        public decimal ToDecimal(byte[] data, int index)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            int[] array = new int[4];
            for (int i = 0; i < 4; i++)
            {
                array[i] = ToInt32(data, index + (i * 4));
            }
            return new decimal(array);
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index.
        /// </summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        public DateTime ToDateTime(byte[] data, int index)
        {
            return new DateTime(ToInt64(data, index));
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index.
        /// </summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        public TimeSpan ToTimeSpan(byte[] data, int index)
        {
            return new TimeSpan(ToInt64(data, index));
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index.
        /// </summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        public double ToDouble(byte[] data, int index)
        {
            return DoubleStruct.ToDouble(ToUInt64(data, index));
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index.
        /// </summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        public float ToSingle(byte[] data, int index)
        {
            return SingleStruct.ToSingle(ToUInt32(data, index));
        }
        #endregion

        #region abstract definitions

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The value as byte array.</returns>
        public abstract byte[] GetBytes(ushort value);

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The value as byte array.</returns>
        public abstract byte[] GetBytes(uint value);

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The value as byte array.</returns>
        public abstract byte[] GetBytes(ulong value);

        /// <summary>
        /// Returns a value converted from the specified data at a specified index.
        /// </summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The value as byte array.</returns>
        public abstract ushort ToUInt16(byte[] data, int index);

        /// <summary>
        /// Returns a value converted from the specified data at a specified index.
        /// </summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The value as byte array.</returns>
        public abstract uint ToUInt32(byte[] data, int index);

        /// <summary>
        /// Returns a value converted from the specified data at a specified index.
        /// </summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The value as byte array.</returns>
        public abstract ulong ToUInt64(byte[] data, int index);

        #endregion
    }
}
