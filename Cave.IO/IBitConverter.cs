#region CopyRight 2018
/*
    Copyright (c) 2003-2018 Andreas Rohleder (andreas@rohleder.cc)
    All rights reserved
*/
#endregion
#region License LGPL-3
/*
    This program/library/sourcecode is free software; you can redistribute it
    and/or modify it under the terms of the GNU Lesser General Public License
    version 3 as published by the Free Software Foundation subsequent called
    the License.

    You may not use this program/library/sourcecode except in compliance
    with the License. The License is included in the LICENSE file
    found at the installation directory or the distribution package.

    Permission is hereby granted, free of charge, to any person obtaining
    a copy of this software and associated documentation files (the
    "Software"), to deal in the Software without restriction, including
    without limitation the rights to use, copy, modify, merge, publish,
    distribute, sublicense, and/or sell copies of the Software, and to
    permit persons to whom the Software is furnished to do so, subject to
    the following conditions:

    The above copyright notice and this permission notice shall be included
    in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
    LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
    OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion
#region Authors & Contributors
/*
   Author:
     Andreas Rohleder <andreas@rohleder.cc>

   Contributors:
 */
#endregion Authors & Contributors

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