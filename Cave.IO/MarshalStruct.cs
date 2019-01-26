using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Cave.IO
{
    /// <summary>
    /// Provides tools for manual struct mashalling.
    /// </summary>
    public static class MarshalStruct // MakeInternal:KEEP
    {
        /// <summary>
        /// Gets the size of the specified structure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int SizeOf<T>()
            where T : struct
        {
#if NET20 || NET35 || NET40 || NET45
            return Marshal.SizeOf(typeof(T));
#else
            return Marshal.SizeOf<T>();
#endif
        }

        /// <summary>
        /// Marshalls the specified buffer to a new structure instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer">Buffer to copy.</param>
        /// <param name="result"></param>
        public static void Copy<T>(byte[] buffer, out T result)
            where T : struct
            => Copy<T>(buffer, 0, out result);

        /// <summary>
        /// Marshalls the specified buffer to a new structure instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer">Buffer to copy.</param>
        /// <param name="offset">Offset to start reading the byte buffer at.</param>
        /// <param name="result"></param>
        public static void Copy<T>(byte[] buffer, int offset, out T result)
            where T : struct
        {
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                if (offset != 0)
                {
                    ptr = new IntPtr(ptr.ToInt64() + offset);
                }
#if NET20 || NET35 || NET40 || NET45
                result = (T)Marshal.PtrToStructure(ptr, typeof(T));
#else
                result = Marshal.PtrToStructure<T>(ptr);
#endif
            }
            finally
            {
                handle.Free();
            }
        }

        /// <summary>
        /// Marshalls the specified structure to a new byte[] instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="data"></param>
        public static void Copy<T>(T item, out byte[] data)
            where T : struct
        {
            int size = SizeOf<T>();
            data = new byte[size];
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr(item, handle.AddrOfPinnedObject(), true);
            }
            finally
            {
                handle.Free();
            }
        }

        /// <summary>
        /// Reads a struct from a stream (see <see cref="DataReader"/> for a comfortable reader class supporting this, too).
        /// </summary>
        /// <typeparam name="T">struct type.</typeparam>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Returns a new struct instance.</returns>
        public static T Read<T>(Stream stream)
            where T : struct
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            int size = SizeOf<T>();
            byte[] buffer = new byte[size];
            if (stream.Read(buffer, 0, size) < size)
            {
                throw new EndOfStreamException();
            }
            Copy(buffer, 0, out T result);
            return result;
        }

        /// <summary>
        /// Writes a struct to a stream (see <see cref="DataWriter"/> for a comfortable reader class supporting this, too).
        /// </summary>
        /// <typeparam name="T">struct type.</typeparam>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="item">the struct to write.</param>
        public static void Write<T>(Stream stream, T item)
            where T : struct
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            Copy(item, out byte[] data);
            stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Reads a struct from a byte buffer.
        /// </summary>
        /// <typeparam name="T">struct type.</typeparam>
        /// <param name="data">byte buffer.</param>
        /// <param name="offset">Offset at the byte buffer to start reading.</param>
        /// <returns></returns>
        public static T Read<T>(byte[] data, int offset = 0)
            where T : struct
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            Copy(data, out T result);
            return result;
        }

        /// <summary>
        /// Writes a struct to a byte buffer.
        /// </summary>
        /// <typeparam name="T">struct type.</typeparam>
        /// <param name="item">the struct to write.</param>
        /// <param name="buffer">byte buffer.</param>
        /// <param name="offset">Offset at the byte buffer to start writing.</param>
        public static void Write<T>(T item, byte[] buffer, int offset)
            where T : struct
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("data");
            }

            Copy(item, out byte[] data);
            Array.Copy(data, 0, buffer, offset, data.Length);
        }

        /// <summary>
        /// Obtains a new byte buffer containing the data of the struct.
        /// </summary>
        /// <typeparam name="T">struct type.</typeparam>
        /// <param name="item">the struct to read.</param>
        /// <returns>returns a new byte buffer.</returns>
        public static byte[] GetBytes<T>(T item)
            where T : struct
        {
            Copy(item, out byte[] data);
            return data;
        }

        /// <summary>
        /// Obtains a new struct instance containing the data of the buffer.
        /// </summary>
        /// <typeparam name="T">struct type.</typeparam>
        /// <param name="data">byte buffer.</param>
        /// <returns>returns a new struct.</returns>
        public static T GetStruct<T>(byte[] data)
            where T : struct
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            Copy(data, out T result);
            return result;
        }

        /// <summary>Reads a native UTF8 string.</summary>
        /// <param name="ptr">The pointer.</param>
        /// <returns></returns>
        public static string ReadUtf8(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            var data = new List<byte>();
            int i = 0;
            while (true)
            {
                byte b = Marshal.ReadByte(ptr, i++);
                if (b == 0)
                {
                    break;
                }

                data.Add(b);
            }
            return Encoding.UTF8.GetString(data.ToArray());
        }

        /// <summary>Reads a native UTF8 strings array.</summary>
        /// <remarks>utf8 string arrays are a memory reagon containing null terminated utf8 strings terminated by an empty utf8 string.</remarks>
        /// <param name="ptr">The pointer.</param>
        /// <returns></returns>
        public static string[] ReadUtf8Strings(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

            var strings = new List<string>();
            var current = new List<byte>();
            for (int i = 0; ; i++)
            {
                byte b = Marshal.ReadByte(ptr, i);
                if (b == 0)
                {
                    if (current.Count == 0)
                    {
                        break;
                    }

                    strings.Add(Encoding.UTF8.GetString(current.ToArray()));
                    current.Clear();
                    continue;
                }
                current.Add(b);
            }
            return strings.ToArray();
        }
    }
}
