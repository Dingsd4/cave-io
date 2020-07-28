using System;

namespace Cave.IO
{
    /// <summary>Provides a id in binary form. This is much more memory efficient if storing a large amount of guids.</summary>
    public sealed class BinaryGuid : IComparable<BinaryGuid>, IComparable
    {
        byte[] data;

        /// <inheritdoc />
        public int CompareTo(object other) => string.CompareOrdinal(ToString(), other?.ToString());

        /// <inheritdoc />
        public int CompareTo(BinaryGuid other) => string.CompareOrdinal(ToString(), other?.ToString());

        /// <summary>Performs an implicit conversion from <see cref="Guid" /> to <see cref="BinaryGuid" />.</summary>
        /// <param name="id">The unique identifier.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator BinaryGuid(Guid id) => id == Guid.Empty ? null : new BinaryGuid { data = id.ToByteArray() };

        /// <summary>Performs an implicit conversion from <see cref="BinaryGuid" /> to <see cref="Guid" />.</summary>
        /// <param name="id">The unique identifier.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Guid(BinaryGuid id) => id?.ToGuid() ?? Guid.Empty;

        /// <summary>Implements the operator ==.</summary>
        /// <param name="g1">The first instance.</param>
        /// <param name="g2">The second instance.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(BinaryGuid g1, BinaryGuid g2) => Equals(g1?.ToString(), g2?.ToString());

        /// <summary>Implements the operator !=.</summary>
        /// <param name="g1">The first instance.</param>
        /// <param name="g2">The second instance.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(BinaryGuid g1, BinaryGuid g2) => !Equals(g1?.ToString(), g2?.ToString());

        /// <summary>Parses the specified text.</summary>
        /// <param name="text">The text.</param>
        /// <returns>the binary GUID.</returns>
        public static BinaryGuid Parse(string text)
        {
            if (text == null)
            {
                return null;
            }

            var guid = new Guid(text);
            if (guid == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(text));
            }

            return new BinaryGuid { data = guid.ToByteArray() };
        }

        /// <summary>Tries to parse the specified id.</summary>
        /// <param name="text">The text.</param>
        /// <param name="id">The unique identifier.</param>
        /// <returns>true if parsing was successful.</returns>
        public static bool TryParse(string text, out BinaryGuid id)
        {
#if NET20 || NET35
#pragma warning disable CA1031
            try
            {
                id = new Guid(text);
                return true;
            }
            catch
            {
                id = null;
                return false;
            }
#pragma warning restore CA1031
#else
            if (Guid.TryParse(text, out var g))
            {
                id = g;
                return true;
            }

            id = null;
            return false;
#endif
        }

        /// <summary>Returns a <see cref="string" /> that represents this instance.</summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString() => new Guid(data).ToString();

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() => new Guid(data).GetHashCode();

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

            return string.Equals(ToString(), obj.ToString(), StringComparison.Ordinal);
        }

        /// <summary>
        /// Gets the <see cref="Guid"/> representation of this instance.
        /// </summary>
        /// <returns>A new <see cref="Guid"/> representation of this instance.</returns>
        public Guid ToGuid() => new Guid(data);

        /// <summary>Converts to an byte array.</summary>
        /// <returns>Returns the byte array.</returns>
        public byte[] ToArray() => (byte[]) data.Clone();
    }
}
