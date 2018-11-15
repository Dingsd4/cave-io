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
#endregion

using Cave.IO;
using System;
using System.Collections.Generic;

namespace Cave.IO
{
    /// <summary>
    /// Provides an alternate <see cref="BitConverter" /> class providing additional functionality
    /// </summary>
    public class BitConverterBE : BitConverterBase
    {
        /// <summary>Gets the default instance.</summary>
        /// <value>The default instance.</value>
        public static BitConverterBE Instance { get; } = new BitConverterBE();

        private BitConverterBE() { }

        #region public GetBytes() members

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override byte[] GetBytes(ushort value)
        {
            return unchecked(new byte[] { (byte)(value / 256), (byte)(value % 256) });
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override byte[] GetBytes(uint value)
        {
            byte[] result = new byte[4];
            for (int i = 3; i >= 0; i--)
            {
                result[i] = (byte)(value % 256);
                value /= 256;
            }
            return result;
        }

        /// <summary>
        /// Retrieves the specified value as byte array with the specified endiantype
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override byte[] GetBytes(ulong value)
        {
            byte[] result = new byte[8];
            for (int i = 7; i >= 0; i--)
            {
                result[i] = (byte)(value % 256);
                value /= 256;
            }
            return result;
        }

        #endregion

        #region public ToXXX() members

        /// <summary>Returns a value converted from the specified data at a specified index</summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">data</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public override ushort ToUInt16(byte[] data, int index)
        {
            return unchecked((ushort)((data[index] * 256) + data[index + 1]));
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index
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

            uint result = 0;
            for (int i = 0; i < 4; i++, index++)
            {
                result = (result * 256) + data[index];
            }
            return result;
        }

        /// <summary>
        /// Returns a value converted from the specified data at a specified index
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

            ulong result = 0;
            for (int i = 0; i < 8; i++, index++)
            {
                result = (result * 256) + data[index];
            }
            return result;
        }

        #endregion
    }
}
