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
        /// <exception cref="ObjectDisposedException"></exception>
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
        /// Ruft beim Überschreiben in einer abgeleiteten Klasse einen Wert ab, der angibt, ob der aktuelle Stream Lesevorgänge unterstützt.
        /// </summary>
        public override bool CanRead => stream.CanRead;

        /// <summary>
        /// Ruft beim Überschreiben in einer abgeleiteten Klasse einen Wert ab, der angibt, ob der aktuelle Stream Suchvorgänge unterstützt.
        /// </summary>
        public override bool CanSeek => stream.CanSeek;

        /// <summary>
        /// Ruft beim Überschreiben in einer abgeleiteten Klasse einen Wert ab, der angibt, ob der aktuelle Stream Schreibvorgänge unterstützt.
        /// </summary>
        public override bool CanWrite => stream.CanWrite;

        /// <summary>Ruft beim Überschreiben in einer abgeleiteten Klasse die Länge des Streams in Bytes ab.</summary>
        public override long Length => streamLength;

        /// <summary>Ruft beim Überschreiben in einer abgeleiteten Klasse die Position im aktuellen Stream ab oder legt diese fest.</summary>
        public override long Position { get => streamPosition; set => Seek(value, SeekOrigin.Begin); }

        /// <summary>
        /// Löscht beim Überschreiben in einer abgeleiteten Klasse alle Puffer für diesen Stream und veranlasst die Ausgabe aller gepufferten Daten an das zugrunde liegende Gerät.
        /// </summary>
        public override void Flush()
        {
            Resistant(() => { stream.Flush(); });
        }

        /// <summary>
        /// Liest beim Überschreiben in einer abgeleiteten Klasse eine Folge von Bytes aus dem aktuellen Stream und erhöht die Position im Stream um die Anzahl der gelesenen Bytes.
        /// </summary>
        /// <param name="buffer">Ein Bytearray.Nach dem Beenden dieser Methode enthält der Puffer das angegebene Bytearray mit den Werten zwischen <paramref name="offset" /> und (<paramref name="offset" /> + <paramref name="count" /> - 1), die durch aus der aktuellen Quelle gelesene Bytes ersetzt wurden.</param>
        /// <param name="offset">Der nullbasierte Byteoffset im <paramref name="buffer" />, ab dem die aus dem aktuellen Stream gelesenen Daten gespeichert werden.</param>
        /// <param name="count">Die maximale Anzahl an Bytes, die aus dem aktuellen Stream gelesen werden sollen.</param>
        /// <returns>
        /// Die Gesamtanzahl der in den Puffer gelesenen Bytes.Dies kann weniger als die Anzahl der angeforderten Bytes sein, wenn diese Anzahl an Bytes derzeit nicht verfügbar ist, oder 0, wenn das Ende des Streams erreicht ist.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return Resistant(() => { return stream.Read(buffer, offset, count); });
        }

        /// <summary>Legt beim Überschreiben in einer abgeleiteten Klasse die Position im aktuellen Stream fest.</summary>
        /// <param name="offset">Ein Byteoffset relativ zum <paramref name="origin" />-Parameter.</param>
        /// <param name="origin">Ein Wert vom Typ <see cref="T:System.IO.SeekOrigin" />, der den Bezugspunkt angibt, von dem aus die neue Position ermittelt wird.</param>
        /// <returns>Die neue Position innerhalb des aktuellen Streams.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return Resistant(() => { return stream.Seek(offset, origin); });
        }

        /// <summary>Legt beim Überschreiben in einer abgeleiteten Klasse die Länge des aktuellen Streams fest.</summary>
        /// <param name="value">Die gewünschte Länge des aktuellen Streams in Bytes.</param>
        public override void SetLength(long value)
        {
            Resistant(() => { stream.SetLength(value); });
        }

        /// <summary>
        /// Schreibt beim Überschreiben in einer abgeleiteten Klasse eine Folge von Bytes in den aktuellen Stream und erhöht die aktuelle Position im Stream um die Anzahl der geschriebenen Bytes.
        /// </summary>
        /// <param name="buffer">Ein Bytearray.Diese Methode kopiert <paramref name="count" /> Bytes aus dem <paramref name="buffer" /> in den aktuellen Stream.</param>
        /// <param name="offset">Der nullbasierte Byteoffset im <paramref name="buffer" />, ab dem Bytes in den aktuellen Stream kopiert werden.</param>
        /// <param name="count">Die Anzahl an Bytes, die in den aktuellen Stream geschrieben werden sollen.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            Resistant(() => { stream.Write(buffer, offset, count); });
        }
        #endregion
    }
}
