namespace Cave.IO
{
    /// <summary>Compression type.</summary>
    public enum IniCompressionType
    {
        /// <summary>No compression, no encryption</summary>
        None = 0x00,

        /// <summary>Compression using deflate</summary>
        Deflate = 0x01,

        /// <summary>Compression using gzip</summary>
        GZip = 0x02
    }
}
