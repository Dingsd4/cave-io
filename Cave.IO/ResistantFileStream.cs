using System;
using System.IO;
using System.Threading;

namespace Cave.IO
{
    /// <summary>
    /// Provides a error resistant file stream for read operations.
    /// </summary>
    /// <seealso cref="Stream" />
    public class ResistantFileStream : Stream
    {
        /// <summary>Opens a new <see cref="ResistantFileStream"/> for sequential reading with a buffer of 128kib.</summary>
        /// <param name="filename">The filename.</param>
        /// <returns>Returns a new <see cref="ResistantFileStream"/> instance.</returns>
        public static ResistantFileStream OpenSequentialRead(string filename)
        {
            return new ResistantFileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.SequentialScan);
        }

        #region private implementation
        volatile FileStream stream;
        long streamLength;
        long streamPosition;

        void OpenStream()
        {
            if (stream != null)
            {
                try
                {
                    stream.Dispose();
                }
                catch
                {
                }
            }

            stream = new FileStream(FileName, FileMode, FileAccess, FileShare, 128 * 1024, FileOptions)
            {
                Position = streamPosition,
            };
            streamLength = stream.Length;
        }

        T Resistant<T>(Func<T> function)
            where T : struct
        {
            Exception exception = null;
            for (int i = 0; i < FileMaxErrors; i++)
            {
                try
                {
                    T result = function();
                    streamPosition = stream.Position;
                    return result;
                }
                catch (Exception ex)
                {
                    exception = ex;
                    try
                    {
                        OpenStream();
                    }
                    catch
                    {
#if NETSTANDARD13
                        System.Threading.Tasks.Task.Delay(100).Wait();
#else
                        Thread.Sleep(100);
#endif
                    }
                }
            }
            throw exception;
        }

        void Resistant(Action action)
        {
            Exception exception = null;
            for (int i = 0; i < FileMaxErrors; i++)
            {
                try
                {
                    action();
                    streamPosition = stream.Position;
                }
                catch (Exception ex)
                {
                    exception = ex;
                    try
                    {
                        OpenStream();
                    }
                    catch
                    {
                    }
                }
            }
            throw exception;
        }
        #endregion

        #region additional properties

        /// <summary>Gets the name of the file.</summary>
        /// <value>The name of the file.</value>
        public string FileName { get; private set; }

        /// <summary>Gets the file mode.</summary>
        /// <value>The file mode.</value>
        public FileMode FileMode { get; private set; }

        /// <summary>Gets the file access.</summary>
        /// <value>The file access.</value>
        public FileAccess FileAccess { get; private set; }

        /// <summary>Gets the file share.</summary>
        /// <value>The file share.</value>
        public FileShare FileShare { get; private set; }

        /// <summary>Gets the file options.</summary>
        /// <value>The file options.</value>
        public FileOptions FileOptions { get; private set; }

        /// <summary>Gets or sets the file access maximum error rate.</summary>
        /// <remarks>Any operation needing more than <see cref="FileMaxErrors"/> retries will fail with the original exception.</remarks>
        /// <value>The file maximum error rate.</value>
        public int FileMaxErrors { get; set; } = 50;

        /// <summary>Gets the file stream.</summary>
        /// <value>The file stream.</value>
        /// <exception cref="ObjectDisposedException">Thrown if object is already disposed.</exception>
        public FileStream BaseStream
        {
            get
            {
                if (stream == null)
                {
                    throw new ObjectDisposedException(nameof(BaseStream));
                }

                return stream;
            }
        }
        #endregion

        #region constructor

        /// <summary>Initializes a new instance of the <see cref="ResistantFileStream"/> class.</summary>
        /// <param name="filename">The filename.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="access">The access.</param>
        /// <param name="share">The share.</param>
        /// <param name="options">The options.</param>
        public ResistantFileStream(string filename, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.Read, FileOptions options = FileOptions.SequentialScan)
        {
            FileName = filename;
            FileMode = mode;
            FileAccess = access;
            FileShare = share;
            FileOptions = options;
            OpenStream();
        }
        #endregion

        #region protected overrides

        /// <summary>
        /// Gibt die vom <see cref="T:System.IO.Stream" /> verwendeten nicht verwalteten Ressourcen und optional auch die verwalteten Ressourcen frei.
        /// </summary>
        /// <param name="disposing">true, um sowohl verwaltete als auch nicht verwaltete Ressourcen freizugeben. false, um ausschließlich nicht verwaltete Ressourcen freizugeben.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                }
            }
            catch
            {
            }
            base.Dispose(disposing);
        }

        #endregion

        #region stream implementation

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead => stream.CanRead;

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek => stream.CanSeek;

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite => stream.CanWrite;

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        public override long Length => streamLength;

        /// <summary>
        /// Gets or sets the current position of this stream.
        /// </summary>
        public override long Position { get => streamPosition; set => Seek(value, SeekOrigin.Begin); }

        /// <summary>
        /// Clears buffers for this stream and causes any buffered data to be written to the file.
        /// </summary>
        public override void Flush()
        {
            Resistant(() => { stream.Flush(); });
        }

        /// <summary>
        /// Reads a block of bytes from the stream and writes the data in a given buffer.
        /// </summary>
        /// <param name="buffer">When this method returns, contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The byte offset in array at which the read bytes will be placed.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This might be less than the number of bytes requested if that number of bytes are not currently available, or zero if the end of the stream is reached.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return Resistant(() => { return stream.Read(buffer, offset, count); });
        }

        /// <summary>Sets the current position of this stream to the given value.</summary>
        /// <param name="offset">The point relative to origin from which to begin seeking.</param>
        /// <param name="origin">Specifies the beginning, the end, or the current position as a reference point for offset, using a value of type SeekOrigin.</param>
        /// <returns>The new position in the stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return Resistant(() => { return stream.Seek(offset, origin); });
        }

        /// <summary>Sets the length of this stream to the given value.</summary>
        /// <param name="value">The new length of the stream.</param>
        public override void SetLength(long value)
        {
            Resistant(() => { stream.SetLength(value); });
        }

        /// <summary>
        /// Writes a block of bytes to the file stream.
        /// </summary>
        /// <param name="buffer">The buffer containing data to write to the stream.</param>
        /// <param name="offset">The zero-based byte offset in array from which to begin copying bytes to the stream.</param>
        /// <param name="count">The maximum number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            Resistant(() => { stream.Write(buffer, offset, count); });
        }
        #endregion
    }
}
