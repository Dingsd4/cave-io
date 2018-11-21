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

namespace Cave.IO
{
    /// <summary>
    /// Provides a guid in binary form. This is much more memory efficient if storing a large amount of guids.
    /// </summary>
    /// <seealso cref="System.IComparable" />
    public sealed class BinaryGuid : IComparable
    {
		/// <summary>Performs an implicit conversion from <see cref="Guid" /> to <see cref="BinaryGuid" />.</summary>
		/// <param name="guid">The unique identifier.</param>
		/// <returns>The result of the conversion.</returns>
		public static implicit operator BinaryGuid(Guid guid)
        {
            if (guid == Guid.Empty)
			{
				throw new Exception("Invalid Guid!");
			}

			return new BinaryGuid() { data = guid.ToByteArray() };
        }

		/// <summary>Performs an implicit conversion from <see cref="BinaryGuid"/> to <see cref="Guid"/>.</summary>
		/// <param name="guid">The unique identifier.</param>
		/// <returns>The result of the conversion.</returns>
		public static implicit operator Guid(BinaryGuid guid)
        {
            if (guid == null)
			{
				throw new Exception("Invalid Guid!");
			}

			return new Guid(guid.data);
        }

        /// <summary>Implements the operator ==.</summary>
        /// <param name="g1">The g1.</param>
        /// <param name="g2">The g2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(BinaryGuid g1, BinaryGuid g2)
        {
            return Equals(g1?.ToString(), g2?.ToString());
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="g1">The g1.</param>
        /// <param name="g2">The g2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(BinaryGuid g1, BinaryGuid g2)
        {
            return !Equals(g1?.ToString(), g2?.ToString());
        }

        /// <summary>Implements the operator ==.</summary>
        /// <param name="g1">The g1.</param>
        /// <param name="g2">The g2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Guid g1, BinaryGuid g2)
        {
            return Equals(g1, g2?.ToString());
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="g1">The g1.</param>
        /// <param name="g2">The g2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Guid g1, BinaryGuid g2)
        {
            return !Equals(g1.ToString(), g2?.ToString());
        }

        /// <summary>Implements the operator ==.</summary>
        /// <param name="g1">The g1.</param>
        /// <param name="g2">The g2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(BinaryGuid g1, Guid g2)
        {
            return Equals(g1?.ToString(), g2.ToString());
        }

        /// <summary>Implements the operator !=.</summary>
        /// <param name="g1">The g1.</param>
        /// <param name="g2">The g2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(BinaryGuid g1, Guid g2)
        {
            return !Equals(g1?.ToString(), g2.ToString());
        }

        /// <summary>Parses the specified text.</summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static BinaryGuid Parse(string text)
        {
            if (text == null) return null;
            var guid = new Guid(text);
            if (guid == Guid.Empty) throw new Exception("Invalid Guid!");
            return new BinaryGuid()
            {
                data = guid.ToByteArray()
            };
        }

		/// <summary>Tries to parse the specified guid.</summary>
		/// <param name="text">The text.</param>
		/// <param name="guid">The unique identifier.</param>
		/// <returns></returns>
		public static bool TryParse(string text, out BinaryGuid guid)
		{
#if NET20 || NET35
			try
			{
				guid = new Guid(text);
				return true;
			}
			catch
			{
				guid = null;
				return false;
			}
#else
			if (Guid.TryParse(text, out Guid g))
			{
				guid = g;
				return true;
			}
			guid = null;
			return false;
#endif
		}

		byte[] data;

        /// <summary>Returns a <see cref="string" /> that represents this instance.</summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return new Guid(data).ToString();
        }

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return new Guid(data).GetHashCode();
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
            return ToString().Equals(obj.ToString());
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
            if (null == obj) return 1;
            return ToString().CompareTo(obj.ToString());
        }

        /// <summary>Converts to an byte array</summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            return (byte[])data.Clone();
        }
    }
}
