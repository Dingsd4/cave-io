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

namespace Cave.IO
{
    /// <summary>
    /// Provides an easy way to access the bits of a single value
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct SingleStruct : IEquatable<SingleStruct>
    {
        /// <summary>Implements the operator ==.</summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(SingleStruct value1, SingleStruct value2)
        {
            return value1.UInt32 == value2.UInt32;
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(SingleStruct value1, SingleStruct value2)
        {
            return value1.UInt32 != value2.UInt32;
        }

        /// <summary>
        /// Converts a <see cref="uint"/> to a <see cref="float"/>
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public static float ToSingle(uint value)
        {
            SingleStruct s = new SingleStruct();
            s.UInt32 = value;
            return s.Single;
        }

        /// <summary>
        /// Converts a <see cref="int"/> to a <see cref="float"/>
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public static float ToSingle(int value)
        {
            SingleStruct s = new SingleStruct();
            s.Int32 = value;
            return s.Single;
        }

        /// <summary>
        /// Converts a <see cref="float"/> to a <see cref="int"/>
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public static int ToInt32(float value)
        {
            SingleStruct s = new SingleStruct();
            s.Single = value;
            return s.Int32;
        }

        /// <summary>
        /// Converts a <see cref="float"/> to a <see cref="uint"/>
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public static uint ToUInt32(float value)
        {
            SingleStruct s = new SingleStruct();
            s.Single = value;
            return s.UInt32;
        }

        /// <summary>
        /// The value as UInt32
        /// </summary>
        [FieldOffset(0)]
        public uint UInt32;

        /// <summary>
        /// The value as UInt32
        /// </summary>
        [FieldOffset(0)]
        public int Int32;

        /// <summary>
        /// The value as Single
        /// </summary>
        [FieldOffset(0)]
        public float Single;

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>Determines whether the specified <see cref="object" />, is equal to this instance.</summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is SingleStruct)
            {
                return base.Equals((SingleStruct)obj);
            }

            return false;
        }

        /// <summary>Determines whether the specified <see cref="SingleStruct" />, is equal to this instance.</summary>
        /// <param name="other">The <see cref="SingleStruct" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="SingleStruct" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals(SingleStruct other)
        {
            return other.UInt32 == UInt32;
        }
    }
}
