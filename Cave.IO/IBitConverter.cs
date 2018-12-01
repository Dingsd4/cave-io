using System;

namespace Cave.IO
{
    /// <summary>
    /// Provides an interface for bit converter implementations
    /// </summary>
    public interface IBitConverter
    {
        /// <summary>Obtains the bytes of a 7 bit encoded integer</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        byte[] Get7BitEncodedBytes(ulong value);

        /// <summary>Obtains the bytes of a 7 bit encoded integer</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        byte[] Get7BitEncodedBytes(long value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        byte[] GetBytes(double value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        byte[] GetBytes(int value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        byte[] GetBytes(sbyte value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        byte[] GetBytes(TimeSpan value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        byte[] GetBytes(ulong value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        byte[] GetBytes(ushort value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        byte[] GetBytes(uint value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        byte[] GetBytes(short value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        byte[] GetBytes(long value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        byte[] GetBytes(float value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        byte[] GetBytes(decimal value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        byte[] GetBytes(DateTime value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        byte[] GetBytes(byte value);

        /// <summary>Retrieves the specified value as byte array with the specified endiantype</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        byte[] GetBytes(bool value);

        /// <summary>Returns a value converted from the specified data at a specified index</summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        bool ToBoolean(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index</summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        byte ToByte(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index</summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        DateTime ToDateTime(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index</summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        decimal ToDecimal(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index</summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        double ToDouble(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index</summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        short ToInt16(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index</summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        int ToInt32(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index</summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        long ToInt64(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index</summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        sbyte ToSByte(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index</summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        float ToSingle(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index</summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        TimeSpan ToTimeSpan(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index</summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        ushort ToUInt16(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index</summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        uint ToUInt32(byte[] data, int index);

        /// <summary>Returns a value converted from the specified data at a specified index</summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        ulong ToUInt64(byte[] data, int index);
    }
}