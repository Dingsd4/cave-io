namespace Cave.IO
{
    /// <summary>
    /// Available endian types
    /// </summary>
    public enum EndianType
    {
        /// <summary>Unknown endian type</summary>
        None = 0,

        /// <summary>
        /// little endian byte order
        /// </summary>
        LittleEndian = 1,

        /// <summary>
        /// big endian byte order
        /// </summary>
        BigEndian = 2,
    }
}
