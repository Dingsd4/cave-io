using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Cave.IO;
using NUnit.Framework; 

namespace Test
{
    [TestFixture]
    public class DataReaderWriterTest
    {
        void TestReaderWriter(EncodingInfo encoding)
        {
            var stream = new MemoryStream();
            var writer = new DataWriter(stream, encoding.GetEncoding());
            var reader = new DataReader(stream, encoding.GetEncoding());
            TestReaderWriter(reader, writer);
        }

        public void TestReaderWriter(StringEncoding stringEncoding)
        {
            if (stringEncoding == 0)
            {
                return;
            }

            //little endian
            using (var stream = new MemoryStream())
            {
                var writer = new DataWriter(stream, stringEncoding);
                var reader = new DataReader(stream, stringEncoding);
                TestReaderWriter(reader, writer);
            }

            //big endian
            using (var stream = new MemoryStream())
            {
                var writer = new DataWriter(stream, stringEncoding, NewLineMode.CRLF, EndianType.BigEndian);
                var reader = new DataReader(stream, stringEncoding, NewLineMode.CRLF, EndianType.BigEndian);
                TestReaderWriter(reader, writer);
            }
        }

        void TestReaderWriter(DataReader reader, DataWriter writer)
        {
            var buffer = new byte[16 * 1024];
            new Random().NextBytes(buffer);
            var dateTime = DateTime.UtcNow;
            var timeSpan = new TimeSpan(Environment.TickCount);
            string randomString;
            byte[] randomStringBytes;
            try
            {
                var charArray = new char[short.MaxValue];
                for (var i = 0; i < charArray.Length; i++)
                {
                    charArray[i] = (char) i;
                }

                randomStringBytes = writer.Encoding.GetBytes(charArray);
                randomString = writer.Encoding.GetString(randomStringBytes);
            }
            catch
            {
                var charArray = new char[128];
                for (var i = 0; i < charArray.Length; i++)
                {
                    charArray[i] = (char) i;
                }

                randomStringBytes = writer.Encoding.GetBytes(charArray);
                randomString = writer.Encoding.GetString(randomStringBytes);
            }

            for (var i = int.MaxValue; i > 0; i >>= 1)
            {
                writer.Write7BitEncoded32(-i);
                writer.Write7BitEncoded32(i);
            }

            for (var i = long.MaxValue; i > 0; i >>= 1)
            {
                writer.Write7BitEncoded64(-i);
                writer.Write7BitEncoded64(i);
            }

            writer.Write(true);
            writer.Write(false);
            try
            {
                writer.Write(randomString[0]);
            }
            catch (NotSupportedException)
            {
                if (!writer.Encoding.IsDead())
                {
                    throw;
                }
            }

            try
            {
                writer.Write(randomString[1]);
            }
            catch (NotSupportedException)
            {
                if (!writer.Encoding.IsDead())
                {
                    throw;
                }
            }

            try
            {
                writer.Write(randomString.ToArray());
            }
            catch (NotSupportedException)
            {
                if (!writer.Encoding.IsDead())
                {
                    throw;
                }
            }

            var position = writer.BaseStream.Position;
            writer.Write(dateTime);
            writer.Write(timeSpan);
            writer.Write(1.23456789m);
            writer.Write(1.23456);
            writer.Write(double.NaN);
            writer.Write(double.PositiveInfinity);
            writer.Write(double.NegativeInfinity);
            writer.Write(1.234f);
            writer.Write(float.NaN);
            writer.Write(float.PositiveInfinity);
            writer.Write(float.NegativeInfinity);
            writer.Write(12345678);
            writer.Write(int.MaxValue);
            writer.Write(int.MinValue);
            writer.Write(uint.MaxValue);
            writer.Write(uint.MinValue);
            writer.Write(long.MaxValue);
            writer.Write(long.MinValue);
            writer.Write(ulong.MaxValue);
            writer.Write(ulong.MinValue);
            writer.Write(short.MaxValue);
            writer.Write(short.MinValue);
            writer.Write(ushort.MaxValue);
            writer.Write(ushort.MinValue);
            writer.Write(byte.MaxValue);
            writer.Write(byte.MinValue);
            writer.Write(sbyte.MaxValue);
            writer.Write(sbyte.MinValue);
            writer.WritePrefixed(randomString);
            writer.WritePrefixed("");
            writer.WritePrefixed((string) null);
            writer.WritePrefixed(buffer);
            try
            {
                writer.WriteZeroTerminated(randomString.Replace("\0", ""));
            }
            catch (NotSupportedException)
            {
                if (!writer.Encoding.IsDead())
                {
                    throw;
                }
            }

            writer.Write(buffer);
            writer.WriteEpoch32(dateTime);
            writer.WriteEpoch64(dateTime);
            var supportsWriteLine = true;
            try
            {
                writer.WriteLine();
                switch (reader.NewLineMode)
                {
                    case NewLineMode.CR:
                        writer.WriteLine("\n\n\n");
                        break;
                    case NewLineMode.CRLF:
                    case NewLineMode.LF:
                        writer.WriteLine("\r\r\r");
                        break;
                    default: throw new NotSupportedException();
                }

                writer.WriteLine(randomString.Replace("\r", "").Replace("\n", ""));
            }
            catch (NotSupportedException)
            {
                if ((writer.StringEncoding == StringEncoding.X_EUROPA) || writer.Encoding.IsDead())
                {
                    supportsWriteLine = false;
                }
                else
                {
                    throw;
                }
            }

            reader.BaseStream.Position = 0;
            for (var i = int.MaxValue; i > 0; i >>= 1)
            {
                Assert.AreEqual(-i, reader.Read7BitEncodedInt32());
                Assert.AreEqual(i, reader.Read7BitEncodedInt32());
            }

            for (var i = long.MaxValue; i > 0; i >>= 1)
            {
                Assert.AreEqual(-i, reader.Read7BitEncodedInt64());
                Assert.AreEqual(i, reader.Read7BitEncodedInt64());
            }

            Assert.AreEqual(true, reader.ReadBool());
            Assert.AreEqual(false, reader.ReadBool());
            try
            {
                Assert.AreEqual(randomString[0], reader.ReadChar());
            }
            catch (NotSupportedException)
            {
                if (!writer.Encoding.IsDead())
                {
                    throw;
                }
            }

            try
            {
                Assert.AreEqual(randomString[1], reader.ReadChar());
            }
            catch (NotSupportedException)
            {
                if (!writer.Encoding.IsDead())
                {
                    throw;
                }
            }

            try
            {
                var readChars = reader.ReadChars(randomString.Length);
                CollectionAssert.AreEqual(randomString.ToCharArray(), readChars);
            }
            catch (NotSupportedException)
            {
                if (!writer.Encoding.IsDead())
                {
                    throw;
                }
            }

            if (reader.BaseStream.Position != position)
            {
                throw new Exception();
            }

            Assert.AreEqual(dateTime, reader.ReadDateTime());
            Assert.AreEqual(timeSpan, reader.ReadTimeSpan());
            Assert.AreEqual(1.23456789m, reader.ReadDecimal());
            Assert.AreEqual(1.23456, reader.ReadDouble());
            Assert.AreEqual(double.NaN, reader.ReadDouble());
            Assert.AreEqual(double.PositiveInfinity, reader.ReadDouble());
            Assert.AreEqual(double.NegativeInfinity, reader.ReadDouble());
            Assert.AreEqual(1.234f, reader.ReadSingle());
            Assert.AreEqual(float.NaN, reader.ReadSingle());
            Assert.AreEqual(float.PositiveInfinity, reader.ReadSingle());
            Assert.AreEqual(float.NegativeInfinity, reader.ReadSingle());
            Assert.AreEqual(12345678, reader.ReadInt32());
            Assert.AreEqual(int.MaxValue, reader.ReadInt32());
            Assert.AreEqual(int.MinValue, reader.ReadInt32());
            Assert.AreEqual(uint.MaxValue, reader.ReadUInt32());
            Assert.AreEqual(uint.MinValue, reader.ReadUInt32());
            Assert.AreEqual(long.MaxValue, reader.ReadInt64());
            Assert.AreEqual(long.MinValue, reader.ReadInt64());
            Assert.AreEqual(ulong.MaxValue, reader.ReadUInt64());
            Assert.AreEqual(ulong.MinValue, reader.ReadUInt64());
            Assert.AreEqual(short.MaxValue, reader.ReadInt16());
            Assert.AreEqual(short.MinValue, reader.ReadInt16());
            Assert.AreEqual(ushort.MaxValue, reader.ReadUInt16());
            Assert.AreEqual(ushort.MinValue, reader.ReadUInt16());
            Assert.AreEqual(byte.MaxValue, reader.ReadUInt8());
            Assert.AreEqual(byte.MinValue, reader.ReadUInt8());
            Assert.AreEqual(sbyte.MaxValue, reader.ReadInt8());
            Assert.AreEqual(sbyte.MinValue, reader.ReadInt8());
            Assert.AreEqual(randomString, reader.ReadString());
            Assert.AreEqual("", reader.ReadString());
            Assert.AreEqual(null, reader.ReadString());
            CollectionAssert.AreEqual(buffer, reader.ReadBytes());
            try
            {
                var readString = reader.ReadZeroTerminatedString(8 * randomString.Length);
                var expected = randomString.Replace("\0", "");
                CollectionAssert.AreEqual(expected.ToCharArray(), readString.ToCharArray());
                Assert.AreEqual(expected, readString);
            }
            catch (NotSupportedException)
            {
                if (!writer.Encoding.IsDead())
                {
                    throw;
                }
            }

            CollectionAssert.AreEqual(buffer, reader.ReadBytes(buffer.Length));
            var epoch = new DateTime(dateTime.Ticks - (dateTime.Ticks % TimeSpan.TicksPerSecond));
            Assert.AreEqual(epoch, reader.ReadEpoch32());
            Assert.AreEqual(epoch, reader.ReadEpoch64());
            if (supportsWriteLine)
            {
                Assert.AreEqual("", reader.ReadLine());
                switch (reader.NewLineMode)
                {
                    case NewLineMode.CR:
                    {
                        var expected = "\n\n\n";
                        var readLine = reader.ReadLine();
                        Assert.AreEqual(expected, readLine);
                        break;
                    }
                    case NewLineMode.CRLF:
                    case NewLineMode.LF:
                    {
                        var expected = "\r\r\r";
                        var readLine = reader.ReadLine();
                        Assert.AreEqual(expected, readLine);
                        break;
                    }
                    default: throw new NotSupportedException();
                }

                {
                    var expected = randomString.Replace("\r", "").Replace("\n", "");
                    var readLine = reader.ReadLine(randomString.Length * 4);
                    CollectionAssert.AreEqual(expected.ToCharArray(), readLine.ToCharArray());
                    Assert.AreEqual(expected, readLine);
                }
            }
        }

        [Test]
        public void TestReaderWriter1()
        {
            var id = "T" + MethodBase.GetCurrentMethod().GetHashCode().ToString("x4");
            foreach (StringEncoding stringEncoding in Enum.GetValues(typeof(StringEncoding)))
            {
                TestReaderWriter(stringEncoding);
                Console.WriteLine($"Test : info {id}: TestReaderWriter({stringEncoding}) ok");
            }
        }

        [Test]
        public void TestReaderWriter2()
        {
            var id = "T" + MethodBase.GetCurrentMethod().GetHashCode().ToString("x4");
            foreach (var encoding in Encoding.GetEncodings())
            {
                TestReaderWriter(encoding);
                Console.WriteLine($"Test : info {id}: TestReaderWriter({encoding.DisplayName}) ok");
            }
        }
    }
}
