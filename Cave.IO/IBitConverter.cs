using System;

namespace Cave.IO
{
    /// <summary>
    /// Provides an interface for bit converter implementations.
    /// </summary>
    public interface IBitConverter
    {
        /// <summary>Obtains the bytes of a 7 bit encoded integer.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        byte[] Get7BitEncodedBytes(ulong value);

        /// <summary>Obtains the bytes of a 7 bit encoded integer.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        byte[] Get7BitEncodedBytes(long value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        byte[] GetBytes(double value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        byte[] GetBytes(int value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        byte[] GetBytes(sbyte value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        byte[] GetBytes(TimeSpan value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        byte[] GetBytes(ulong value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        byte[] GetBytes(ushort value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        byte[] GetBytes(uint value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        byte[] GetBytes(short value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        byte[] GetBytes(long value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        byte[] GetBytes(float value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        byte[] GetBytes(decimal value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        byte[] GetBytes(DateTime value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        byte[] GetBytes(byte value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The value as encoded byte array.</returns>
        byte[] GetBytes(bool value);

        /// <summary>Returns a value converted from the specified data at a specified index.</summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">out of range.</exception>
        bool ToBoolean(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index.</summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">out of range.</exception>
        byte ToByte(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index.</summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">out of range.</exception>
        DateTime ToDateTime(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index.</summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">out of range.</exception>
        decimal ToDecimal(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index.</summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">out of range.</exception>
        double ToDouble(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index.</summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">out of range.</exception>
        short ToInt16(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index.</summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">out of range.</exception>
        int ToInt32(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index.</summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">out of range.</exception>
        long ToInt64(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index.</summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">out of range.</exception>
        sbyte ToSByte(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index.</summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">out of range.</exception>
        float ToSingle(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index.</summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">out of range.</exception>
        TimeSpan ToTimeSpan(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index.</summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">out of range.</exception>
        ushort ToUInt16(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index.</summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">out of range.</exception>
        uint ToUInt32(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index.</summary>
        /// <param name="data">The data as byte array.</param>
        /// <param name="index">The index.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">out of range.</exception>
        ulong ToUInt64(byte[] data, int index);
    }
}
