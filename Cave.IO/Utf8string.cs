using System;
using System.Diagnostics;
using System.Text;

namespace Cave.IO
{
    /// <summary>
    ///     Provides a string encoded on the heap using utf8. This will reduce the memory usage by about 40-50% on most
    ///     western languages / ascii based character sets.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public sealed class Utf8string : IComparable<Utf8string>, IComparable
    {
        byte[] data;

        /// <summary>Gets the length.</summary>
        /// <value>The length.</value>
        public int Length { get; private set; }

        /// <inheritdoc />
        public int CompareTo(object other) => string.CompareOrdinal(ToString(), other?.ToString());

        /// <inheritdoc />
        public int CompareTo(Utf8string other) => string.CompareOrdinal(ToString(), other?.ToString());

        /// <summary>Performs an implicit conversion from <see cref="Utf8string" /> to <see cref="string" />.</summary>
        /// <param name="s">The string.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(Utf8string s) => s?.ToString();

        /// <summary>Performs an implicit conversion from <see cref="string" /> to <see cref="Utf8string" />.</summary>
        /// <param name="s">The string.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Utf8string(string s) => s == null ? null : Parse(s);

        /// <summary>Implements the operator ==.</summary>
        /// <param name="s1">The s1.</param>
        /// <param name="s2">The s2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Utf8string s1, Utf8string s2) => Equals(s1?.ToString(), s2?.ToString());

        /// <summary>Implements the operator !=.</summary>
        /// <param name="s1">The s1.</param>
        /// <param name="s2">The s2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Utf8string s1, Utf8string s2) => !Equals(s1?.ToString(), s2?.ToString());

        /// <summary>Implements the operator ==.</summary>
        /// <param name="s1">The s1.</param>
        /// <param name="s2">The s2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(string s1, Utf8string s2) => Equals(s1, s2?.ToString());

        /// <summary>Implements the operator !=.</summary>
        /// <param name="s1">The s1.</param>
        /// <param name="s2">The s2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(string s1, Utf8string s2) => !Equals(s1, s2?.ToString());

        /// <summary>Implements the operator ==.</summary>
        /// <param name="s1">The s1.</param>
        /// <param name="s2">The s2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Utf8string s1, string s2) => Equals(s2, s1?.ToString());

        /// <summary>Implements the operator !=.</summary>
        /// <param name="s1">The s1.</param>
        /// <param name="s2">The s2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Utf8string s1, string s2) => !Equals(s2, s1?.ToString());

        /// <summary>Parses the specified text.</summary>
        /// <param name="text">The text.</param>
        /// <returns>The text as UTF-8 string.</returns>
        public static Utf8string Parse(string text) =>
            new Utf8string { data = Encoding.UTF8.GetBytes(text), Length = text.Length };

        /// <summary>Returns a <see cref="string" /> that represents this instance.</summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString() => Encoding.UTF8.GetString(data);

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() => ToString().GetHashCode();

        /// <summary>Determines whether the specified <see cref="object" />, is equal to this instance.</summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
            {
                return true;
            }

            if (obj is null)
            {
                return false;
            }

            return Equals(ToString(), obj.ToString());
        }
    }
}
