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
	/// Provides an easy way to access the bits of a double value
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
    public struct DoubleStruct : IEquatable<DoubleStruct>
    {
        /// <summary>Implements the operator ==.</summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(DoubleStruct value1, DoubleStruct value2)
        {
            return value1.UInt64 == value2.UInt64;
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(DoubleStruct value1, DoubleStruct value2)
        {
            return value1.UInt64 != value2.UInt64;
        }

        /// <summary>
        /// Converts a <see cref="ulong"/> to a <see cref="double"/>
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public static double ToDouble(ulong value)
        {
            DoubleStruct d = new DoubleStruct();
            d.UInt64 = value;
            return d.Double;
        }

        /// <summary>
        /// Converts a <see cref="long"/> to a <see cref="double"/>
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public static double ToDouble(long value)
        {
            DoubleStruct d = new DoubleStruct();
            d.Int64 = value;
            return d.Double;
        }

        /// <summary>
        /// Converts a <see cref="double"/> to a <see cref="long"/>
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public static long ToInt64(double value)
        {
            DoubleStruct d = new DoubleStruct();
            d.Double = value;
            return d.Int64;
        }

        /// <summary>
        /// Converts a <see cref="double"/> to a <see cref="ulong"/>
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <returns></returns>
        public static ulong ToUInt64(double value)
        {
            DoubleStruct d = new DoubleStruct();
            d.Double = value;
            return d.UInt64;
        }

        /// <summary>
        /// The value as UInt64
        /// </summary>
        [FieldOffset(0)]
        public ulong UInt64;

        /// <summary>
        /// The value as Int64
        /// </summary>
        [FieldOffset(0)]
        public long Int64;

        /// <summary>
        /// The value as Double
        /// </summary>
        [FieldOffset(0)]
        public double Double;

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
            if (obj is DoubleStruct) return base.Equals((DoubleStruct)obj);
            return false;
        }

        /// <summary>Determines whether the specified <see cref="DoubleStruct" />, is equal to this instance.</summary>
        /// <param name="other">The <see cref="DoubleStruct" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="DoubleStruct" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals(DoubleStruct other)
        {
            return other.UInt64 == UInt64;
        }
    }
}
