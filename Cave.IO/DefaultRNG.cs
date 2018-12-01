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

using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Cave.IO
{
    /// <summary>
    /// Provides a globally used random number generator instance.
    /// </summary>
    [ComVisible(false)]
    public static class DefaultRNG
    {
        static RandomNumberGenerator generator = RandomNumberGenerator.Create();

        public static RandomNumberGenerator Generator { get=> generator; set => generator = value ?? throw new ArgumentNullException(nameof(value)); }

        /// <summary>
        ///
        /// </summary>
        /// <param name="array"></param>
        public static void Fill(byte[] array)
        {
            Generator.GetBytes(array);
        }

        /// <summary>
        /// Obtains a byte array containing secure random bytes with the specified size
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] Get(int size)
        {
            byte[] array = new byte[size];
            Fill(array);
            return array;
        }

        /// <summary>
        /// Obtains a random 8 bit signed integer
        /// </summary>
        public static sbyte Int8
        {
            get
            {
                return (sbyte)Get(1)[0];
            }
        }

        /// <summary>
        /// Obtains a random 8 bit unsigned integer
        /// </summary>
        public static byte UInt8
        {
            get
            {
                return Get(1)[0];
            }
        }

        /// <summary>
        /// Obtains a random 16 bit signed integer
        /// </summary>
        public static short Int16
        {
            get
            {
                return BitConverter.ToInt16(Get(2), 0);
            }
        }

        /// <summary>
        /// Obtains a random 16 bit unsigned integer
        /// </summary>
        public static ushort UInt16
        {
            get
            {
                return BitConverter.ToUInt16(Get(2), 0);
            }
        }

        /// <summary>
        /// Obtains a random 32 bit signed integer
        /// </summary>
        public static int Int32
        {
            get
            {
                return BitConverter.ToInt32(Get(4), 0);
            }
        }

        /// <summary>
        /// Obtains a random 32 bit unsigned integer
        /// </summary>
        public static uint UInt32
        {
            get
            {
                return BitConverter.ToUInt32(Get(4), 0);
            }
        }

        /// <summary>
        /// Obtains a random 64 bit signed integer
        /// </summary>
        public static long Int64
        {
            get
            {
                return BitConverter.ToInt64(Get(8), 0);
            }
        }

        /// <summary>
        /// Obtains a random 64 bit unsigned integer
        /// </summary>
        public static ulong UInt64
        {
            get
            {
                return BitConverter.ToUInt64(Get(8), 0);
            }
        }

        /// <summary>Creates a random password using ascii printable characters.</summary>
        /// <param name="count">Length of the desired password.</param>
        /// <param name="characters">The characters.</param>
        /// <returns></returns>
        public static string GetPassword(int count, string characters = null)
        {
            char[] result = new char[count];
            uint value = UInt32;
            char[] chars;
            if (characters != null)
            {
                chars = characters.ToCharArray();
            }
            else
            {
                chars = new char[126 - 32 - 2];
                for (int x = 33, n = 0; n < chars.Length; x++)
                {
                    var c = (char)x;
                    if (c == '"') continue;
                    if (c == '\'') continue;
                    chars[n++] = c;
                }
            };
            uint charsCount = (uint)chars.Length;
            int i = 0;
            while (i < count)
            {
                if (value < chars.Length)
                {
                    value ^= UInt32;
                }
                uint index = value % charsCount;
                result[i++] = chars[index];
                value /= charsCount;
            }
            return new string(result);
        }
    }
}
