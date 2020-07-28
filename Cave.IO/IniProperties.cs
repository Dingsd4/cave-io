using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

#pragma warning disable CA1051

namespace Cave.IO
{
    /// <summary>Provides properties for the <see cref="IniReader" /> and <see cref="IniWriter" /> classes.</summary>
    public struct IniProperties : IEquatable<IniProperties>, IDisposable
    {
        /// <summary>Default is case insensitive. Set this to true to match properties exactly.</summary>
        public bool CaseSensitive;

        /// <summary>
        ///     Use simple synchroneous encryption to protect from users eyes ? (This is not a security feature, use file
        ///     system acl to protect from other users.)
        /// </summary>
        public SymmetricAlgorithm Encryption;

        /// <summary>Gets / sets the culture used to en/decode values.</summary>
        public CultureInfo Culture;

        /// <summary>Gets / sets the <see cref="IniCompressionType" />.</summary>
        public IniCompressionType Compression;

        /// <summary>Gets / sets the <see cref="Encoding" />.</summary>
        public Encoding Encoding;

        /// <summary>Gets / sets the format of date time fields.</summary>
        public string DateTimeFormat;

        /// <summary>Gets or sets the string boxing character.</summary>
        public char BoxCharacter;

        /// <summary>
        ///     Gets <see cref="IniProperties" /> with default settings: Encoding=UTF8, Compression=None, InvariantCulture and
        ///     no encryption.
        /// </summary>
        public static IniProperties Default
        {
            get
            {
                var result = new IniProperties
                {
                    Culture = CultureInfo.InvariantCulture,
                    Compression = IniCompressionType.None,
                    Encoding = new UTF8Encoding(false),
                    DateTimeFormat = StringExtensions.InterOpDateTimeFormat,
                    BoxCharacter = '"'
                };
                return result;
            }
        }

        /// <summary>Gets a value indicating whether the properties are all set or not.</summary>
        public bool Valid =>
            Enum.IsDefined(typeof(IniCompressionType), Compression) &&
            (Encoding != null) &&
            (Culture != null);

        /// <summary>Implements the operator ==.</summary>
        /// <param name="properties1">The properties1.</param>
        /// <param name="properties2">The properties2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(IniProperties properties1, IniProperties properties2) => properties1.Equals(properties2);

        /// <summary>Implements the operator !=.</summary>
        /// <param name="properties1">The properties1.</param>
        /// <param name="properties2">The properties2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(IniProperties properties1, IniProperties properties2) => !properties1.Equals(properties2);

        /// <summary>
        ///     Obtains <see cref="IniProperties" /> with default settings and simple encryption. (This is not a security
        ///     feature, use file system acl to protect from other users.)
        /// </summary>
        /// <param name="password">Password to use.</param>
        /// <returns>Returns a new <see cref="IniProperties" /> instance.</returns>
        public static IniProperties Encrypted(string password)
        {
            var salt = new byte[16];
            for (byte i = 0; i < salt.Length; salt[i] = ++i) { }

#pragma warning disable CA5379
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 20000);
            var result = Default;
            result.Encryption = new RijndaelManaged { BlockSize = 128 };
            result.Encryption.Key = pbkdf2.GetBytes(result.Encryption.KeySize / 8);
            result.Encryption.IV = pbkdf2.GetBytes(result.Encryption.BlockSize / 8);
            (pbkdf2 as IDisposable)?.Dispose();
            return result;
#pragma warning restore CA5379
        }

        /// <summary>Returns a hash code for this instance.</summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <inheritdoc />
        public void Dispose() => (Encryption as IDisposable)?.Dispose();

        /// <summary>Determines whether the specified <see cref="object" />, is equal to this instance.</summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) => obj is IniProperties properties && base.Equals(properties);

        /// <summary>Determines whether the specified <see cref="IniProperties" />, is equal to this instance.</summary>
        /// <param name="other">The <see cref="IniProperties" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="IniProperties" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public bool Equals(IniProperties other) =>
            (other.CaseSensitive == CaseSensitive)
         && (other.Compression == Compression)
         && Equals(other.Culture, Culture)
         && (other.DateTimeFormat == DateTimeFormat)
         && Equals(other.Encoding, Encoding)
         && (other.BoxCharacter == BoxCharacter)
         && (other.Encryption == Encryption);
    }
}

#pragma warning restore CA1051
