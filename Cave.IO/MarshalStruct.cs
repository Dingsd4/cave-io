using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Cave.IO
{
    /// <summary>
    /// Provides tools for manual struct mashalling
    /// </summary>
    public static class MarshalStruct //MakeInternal:KEEP
    {
        /// <summary>
        /// Reads a struct from a stream (see <see cref="DataReader"/> for a comfortable reader class supporting this, too)
        /// </summary>
        /// <typeparam name="T">struct type</typeparam>
        /// <param name="stream">Stream to read from</param>
        /// <returns>Returns a new struct instance</returns>
        public static T Read<T>(Stream stream) where T : struct
        {
            if (stream == null) throw new ArgumentNullException("stream");
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            if (stream.Read(buffer, 0, size) < size) throw new EndOfStreamException();
            GCHandle l_Handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            T result = (T)Marshal.PtrToStructure(l_Handle.AddrOfPinnedObject(), typeof(T));
            l_Handle.Free();
            return result;
        }

        /// <summary>
        /// Writes a struct to a stream (see <see cref="DataWriter"/> for a comfortable reader class supporting this, too)
        /// </summary>
        /// <typeparam name="T">struct type</typeparam>
        /// <param name="stream">Stream to write to</param>
        /// <param name="item">the struct to write</param>
        public static void Write<T>(Stream stream, T item) where T : struct
        {
            if (stream == null) throw new ArgumentNullException("stream");
            int size = Marshal.SizeOf(item);
            byte[] data = new byte[size];
            GCHandle l_Handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            Marshal.StructureToPtr(item, l_Handle.AddrOfPinnedObject(), false);
            l_Handle.Free();
            stream.Write(data, 0, size);
        }

        /// <summary>
        /// Reads a struct from a byte buffer
        /// </summary>
        /// <typeparam name="T">struct type</typeparam>
        /// <param name="data">byte buffer</param>
        /// <param name="offset">Offset at the byte buffer to start reading</param>
        /// <returns></returns>
        public static T Read<T>(byte[] data, int offset) where T : struct
        {
            if (data == null) throw new ArgumentNullException("data");
            int size = Marshal.SizeOf(typeof(T));
            if (offset + size > data.Length) throw new ArgumentOutOfRangeException(nameof(offset), "Buffer smaller than Offset+Size!");
            GCHandle l_Handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            IntPtr addr = new IntPtr(l_Handle.AddrOfPinnedObject().ToInt64() + offset);
            T result = (T)Marshal.PtrToStructure(addr, typeof(T));
            l_Handle.Free();
            return result;
        }

        /// <summary>
        /// Writes a struct to a byte buffer
        /// </summary>
        /// <typeparam name="T">struct type</typeparam>
        /// <param name="item">the struct to write</param>
        /// <param name="data">byte buffer</param>
        /// <param name="offset">Offset at the byte buffer to start writing</param>
        public static void Write<T>(T item, byte[] data, int offset) where T : struct
        {
            if (data == null) throw new ArgumentNullException("data");
            int size = Marshal.SizeOf(item);
            if (offset + size > data.Length) throw new ArgumentOutOfRangeException(nameof(offset), "Buffer smaller than Offset + Size!");
            GCHandle l_Handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            IntPtr addr = new IntPtr(l_Handle.AddrOfPinnedObject().ToInt64() + offset);
            Marshal.StructureToPtr(item, addr, false);
            l_Handle.Free();
        }

        /// <summary>
        /// Obtains a new byte buffer containing the data of the struct
        /// </summary>
        /// <typeparam name="T">struct type</typeparam>
        /// <param name="item">the struct to read</param>
        /// <returns>returns a new byte buffer</returns>
        public static byte[] GetBytes<T>(T item) where T : struct
        {
            int size = Marshal.SizeOf(item);
            byte[] data = new byte[size];
            GCHandle l_Handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            Marshal.StructureToPtr(item, l_Handle.AddrOfPinnedObject(), false);
            l_Handle.Free();
            return data;
        }

        /// <summary>
        /// Obtains a new struct instance containing the data of the buffer
        /// </summary>
        /// <typeparam name="T">struct type</typeparam>
        /// <param name="data">byte buffer</param>
        /// <returns>returns a new struct</returns>
        public static T GetStruct<T>(byte[] data) where T : struct
        {
            if (data == null) throw new ArgumentNullException("data");
            Type type = typeof(T);
            int size = Marshal.SizeOf(type);
            if (size != data.Length) throw new InvalidDataException("Buffer length does not match struct size!");
            GCHandle l_Handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            T result = (T)Marshal.PtrToStructure(l_Handle.AddrOfPinnedObject(), type);
            l_Handle.Free();
            return result;
        }

        /// <summary>Reads a native UTF8 string.</summary>
        /// <param name="ptr">The pointer.</param>
        /// <returns></returns>
        public static string ReadUtf8(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) return null;
            List<byte> data = new List<byte>();
            int i = 0;
            while (true)
            {
                byte b = Marshal.ReadByte(ptr, i++);
                if (b == 0) break;
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
            if (ptr == IntPtr.Zero) return null;
            List<string> strings = new List<string>();
            List<byte> current = new List<byte>();
            for (int i = 0; ; i++)
            {
                byte b = Marshal.ReadByte(ptr, i);
                if (b == 0)
                {
                    if (current.Count == 0) break;
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
