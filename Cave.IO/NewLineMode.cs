namespace Cave.IO
{
    /// <summary>Available NewLine modes for DataReader and DataWriter.</summary>
    public enum NewLineMode
    {
        /// <summary>undefined</summary>
        Undefined = -1,

        /// <summary>carriage return only</summary>
        CR = 0x13,

        /// <summary>line feed only</summary>
        LF = 0x10,

        /// <summary>carriage return followed by line feed</summary>
        CRLF = 0x1310
    }
}
