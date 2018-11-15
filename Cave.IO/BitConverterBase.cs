﻿#region CopyRight 2018
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

using Cave.IO;
using System;
using System.Collections.Generic;

namespace Cave.IO
{
    /// <summary>
    /// Provides a base class for bit converters
    /// </summary>
    /// <seealso cref="IBitConverter" />
    public abstract class BitConverterBase : IBitConverter
    {
        #region public GetBytes() members

        /// <summary>Obtains the bytes of a 7 bit encoded integer</summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
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
            } while (value != 0);
            Array.Resize(ref result, index);
            return result;
        }

        /// <summary>
        /// Obtains the bytes of a 7 bit encoded integer
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] Get7BitEncodedBytes(long value)
        {
            return Get7BitEncodedBytes(unchecked((ulong)value));
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
        /// Retrieves the specified value as byte array with the specified endiantype
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(byte value)
        {
            return unchecked(new byte[] { value });
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(sbyte value)
        {
            return unchecked(new byte[] { (byte)value });
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(short value)
        {
            return unchecked(GetBytes((ushort)value));
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(int value)
        {
            return unchecked(GetBytes((uint)value));
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(long value)
        {
            return unchecked(GetBytes((ulong)value));
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(float value)
        {
            return GetBytes(SingleStruct.ToUInt32(value));
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(double value)
        {
            return GetBytes(DoubleStruct.ToUInt64(value));
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(DateTime value)
        {
            return GetBytes(value.Ticks);
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] GetBytes(TimeSpan value)
        {
            return GetBytes(value.Ticks);
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
        /// Returns a value converted from the specified data at a specified index
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool ToBoolean(byte[] data, int index)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            return data[index] != 0;
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte ToByte(byte[] data, int index)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            return data[index];
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public sbyte ToSByte(byte[] data, int index)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            return unchecked((sbyte)data[index]);
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public short ToInt16(byte[] data, int index)
        {
            return unchecked((short)ToUInt16(data, index));
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public int ToInt32(byte[] data, int index)
        {
            return unchecked((int)ToUInt32(data, index));
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public long ToInt64(byte[] data, int index)
        {
            return unchecked((long)ToUInt64(data, index));
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public decimal ToDecimal(byte[] data, int index)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            int[] array = new int[4];
            for (int i = 0; i < 4; i++)
            {
                array[i] = ToInt32(data, index + i * 4);
            }
            return new decimal(array);
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public DateTime ToDateTime(byte[] data, int index)
        {
            return new DateTime(ToInt64(data, index));
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public TimeSpan ToTimeSpan(byte[] data, int index)
        {
            return new TimeSpan(ToInt64(data, index));
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public double ToDouble(byte[] data, int index)
        {
            return DoubleStruct.ToDouble(ToUInt64(data, index));
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public float ToSingle(byte[] data, int index)
        {
            return SingleStruct.ToSingle(ToUInt32(data, index));
        }
        #endregion

        #region abstract definitions
        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public abstract byte[] GetBytes(ushort value);

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public abstract byte[] GetBytes(uint value);

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public abstract byte[] GetBytes(ulong value);

        /// <summary>
        /// Returns a value converted from the specified data at a specified index
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract ushort ToUInt16(byte[] data, int index);

        /// <summary>
        /// Returns a value converted from the specified data at a specified index
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract uint ToUInt32(byte[] data, int index);

        /// <summary>
        /// Returns a value converted from the specified data at a specified index
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract ulong ToUInt64(byte[] data, int index);

        #endregion
    }
}