using System;
using System.Text;

namespace Cave.IO
{
    /// <summary>A FourCC (literally, four-character code) is a sequence of four bytes used to uniquely identify data formats.</summary>
    public struct FourCC : IEquatable<FourCC>
    {
        /// <summary>The equality operator == returns true if its operands are equal, false otherwise.</summary>
        /// <param name="f1">First value.</param>
        /// <param name="f2">Second value.</param>
        /// <returns>true if the operands are equal.</returns>
        public static bool operator ==(FourCC f1, FourCC f2) => Equals(f1, f2);

        /// <summary>The equality operator != returns false if its operands are equal, true otherwise.</summary>
        /// <param name="f1">First value.</param>
        /// <param name="f2">Second value.</param>
        /// <returns>false if the operands are equal.</returns>
        public static bool operator !=(FourCC f1, FourCC f2) => !Equals(f1, f2);

        /// <summary>Implicit conversion from <see cref="FourCC" /> to string[4].</summary>
        /// <param name="val">Value to convert.</param>
        public static implicit operator string(FourCC val) => val.ToString();

        /// <summary>Implicit conversion from <see cref="FourCC" /> to int.</summary>
        /// <param name="val">Value to convert.</param>
        public static implicit operator int(FourCC val) => (int) val.value;

        /// <summary>Implicit conversion from <see cref="FourCC" /> to uint.</summary>
        /// <param name="val">Value to convert.</param>
        public static implicit operator uint(FourCC val) => val.value;

        /// <summary>Implicit conversion from string[4] to <see cref="FourCC" />.</summary>
        /// <param name="val">Value to convert.</param>
        public static implicit operator FourCC(string val) => FromString(val);

        /// <summary>Implicit conversion from int to <see cref="FourCC" />.</summary>
        /// <param name="val">Value to convert.</param>
        public static implicit operator FourCC(int val) => FromUInt32((uint) val);

        /// <summary>Implicit conversion from uint to <see cref="FourCC" />.</summary>
        /// <param name="val">Value to convert.</param>
        public static implicit operator FourCC(uint val) => FromUInt32(val);

        /// <summary>Creates a new <see cref="FourCC" /> instance with the specified string[4].</summary>
        /// <param name="str">String to set.</param>
        /// <returns>Returns a new <see cref="FourCC" /> instance.</returns>
        public static FourCC FromString(string str)
        {
            var bytes = Encoding.ASCII.GetBytes(str);
            if (bytes.Length != 4)
            {
                throw new ArgumentOutOfRangeException(nameof(str));
            }

            return new FourCC { value = BitConverter.ToUInt32(bytes, 0) };
        }

        /// <summary>Creates a new <see cref="FourCC" /> instance with the specified content.</summary>
        /// <param name="val">The value to set.</param>
        /// <returns>Returns a new <see cref="FourCC" /> instance.</returns>
        public static FourCC FromUInt32(uint val) => new FourCC { value = val };

        uint value;

        /// <summary>Retrieves the 4 character string this value represents.</summary>
        /// <returns>Returns a string[4].</returns>
        public override string ToString()
        {
            var bytes = BitConverter.GetBytes(value);
            return Encoding.ASCII.GetString(bytes);
        }

        /// <summary>Checks for equality with the <see cref="object.ToString()" /> method of the specified <paramref name="obj" />.</summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>Returns true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj) => Equals(ToString(), obj?.ToString());

        /// <inheritdoc />
        public bool Equals(FourCC other) => value == other.value;

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>Returns a hash code for the current object.</returns>
        public override int GetHashCode() => ToString().GetHashCode();
    }
}
