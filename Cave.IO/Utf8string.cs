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
using System.Diagnostics;
using System.Text;

namespace Cave.IO
{
    /// <summary>
    /// Provides a string encoded on the heap using utf8. This will reduce the memory usage by about 40-50% on most western languages / ascii based
    /// character sets.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public sealed class Utf8string : IComparable
    {
        /// <summary>Performs an implicit conversion from <see cref="Utf8string"/> to <see cref="string"/>.</summary>
        /// <param name="s">The string.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(Utf8string s)
        {
            return s?.ToString();
        }

        /// <summary>Performs an implicit conversion from <see cref="string"/> to <see cref="Utf8string"/>.</summary>
        /// <param name="s">The string.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Utf8string(string s)
        {
            if (s == null) return null;
            return Parse(s);
        }

        /// <summary>Implements the operator ==.</summary>
        /// <param name="s1">The s1.</param>
        /// <param name="s2">The s2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Utf8string s1, Utf8string s2)
        {
            return Equals(s1?.ToString(), s2?.ToString());
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="s1">The s1.</param>
        /// <param name="s2">The s2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Utf8string s1, Utf8string s2)
        {
            return !Equals(s1?.ToString(), s2?.ToString());
        }

        /// <summary>Implements the operator ==.</summary>
        /// <param name="s1">The s1.</param>
        /// <param name="s2">The s2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(string s1, Utf8string s2)
        {
            return Equals(s1, s2?.ToString());
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="s1">The s1.</param>
        /// <param name="s2">The s2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(string s1, Utf8string s2)
        {
            return !Equals(s1, s2?.ToString());
        }

        /// <summary>Implements the operator ==.</summary>
        /// <param name="s1">The s1.</param>
        /// <param name="s2">The s2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Utf8string s1, string s2)
        {
            return Equals(s2, s1?.ToString());
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="s1">The s1.</param>
        /// <param name="s2">The s2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Utf8string s1, string s2)
        {
            return !Equals(s2, s1?.ToString());
        }

        /// <summary>Parses the specified text.</summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static Utf8string Parse(string text)
        {
            return new Utf8string()
            {
                data = Encoding.UTF8.GetBytes(text),
                Length = text.Length,
            };
        }

        byte[] data;

        /// <summary>Gets the length.</summary>
        /// <value>The length.</value>
        public int Length { get; private set; }

        /// <summary>Returns a <see cref="string" /> that represents this instance.</summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>Determines whether the specified <see cref="object" />, is equal to this instance.</summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this)) return true;
            if (ReferenceEquals(obj, null)) return false;
            return Equals(ToString(), obj.ToString());
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in the sort order.
        /// </returns>
        public int CompareTo(object obj)
        {
            if (ReferenceEquals(obj, null)) return -1;
            return ToString().CompareTo(obj.ToString());
        }
    }
}
