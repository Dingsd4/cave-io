using Cave.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Test
{
    [TestClass]
    public class DataReaderWriterTest
    {
        [TestMethod]
        public void TestReaderWriter1()
        {
            var id = "T" + MethodBase.GetCurrentMethod().GetHashCode().ToString("x4");

            foreach (StringEncoding stringEncoding in Enum.GetValues(typeof(StringEncoding)))
            {
                try
                {
                    TestReaderWriter(stringEncoding);
                    Console.WriteLine($"Test : info {id}: TestReaderWriter({stringEncoding}) ok");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Test : warning {id}: TestReaderWriter({stringEncoding}) {ex.Message}");
                }
            }
        }

        [TestMethod]
        public void TestReaderWriter2()
        {
            var id = "T" + MethodBase.GetCurrentMethod().GetHashCode().ToString("x4");

            foreach (var encoding in Encoding.GetEncodings())
            {
                try
                {
                    TestReaderWriter(encoding);
                    Console.WriteLine($"Test : info {id}: TestReaderWriter({encoding.DisplayName}) ok");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Test : warning {id}: TestReaderWriter({encoding.DisplayName}) {ex.Message}");
                }
            }
        }

        void TestReaderWriter(EncodingInfo encoding)
        {
            var stream = new MemoryStream();
            DataWriter writer = new DataWriter(stream, encoding.GetEncoding());
            DataReader reader = new DataReader(stream, encoding.GetEncoding());
            TestReaderWriter(reader, writer);
        }

        void TestReaderWriter(StringEncoding stringEncoding)
        {
            if (stringEncoding == 0)
            {
                return;
            }

            var stream = new MemoryStream();
            DataWriter writer = new DataWriter(stream, stringEncoding);
            DataReader reader = new DataReader(stream, stringEncoding);
            TestReaderWriter(reader, writer);
        }

        void TestReaderWriter(DataReader reader, DataWriter writer)
        {
            byte[] buffer = new byte[16 * 1024];
            new Random().NextBytes(buffer);
            var dateTime = DateTime.UtcNow;
            var timeSpan = new TimeSpan(Environment.TickCount);

            string randomString;
            byte[] randomStringBytes;
            try
            {
                char[] charArray = new char[short.MaxValue];
                for (int i = 0; i < charArray.Length; i++) charArray[i] = (char)i;
                randomStringBytes = writer.Encoding.GetBytes(charArray);
                randomString = writer.Encoding.GetString(randomStringBytes);
            }
            catch
            {
                char[] charArray = new char[127];
                for (int i = 0; i < 127; i++) charArray[i] = (char)i;
                randomStringBytes = writer.Encoding.GetBytes(charArray);
                randomString = writer.Encoding.GetString(randomStringBytes);
            }

            for (int i = int.MaxValue; i > 0; i >>= 1)
            {
                writer.Write7BitEncoded32(-i);
                writer.Write7BitEncoded32(i);
            }
            for (long i = long.MaxValue; i > 0; i >>= 1)
            {
                writer.Write7BitEncoded64(-i);
                writer.Write7BitEncoded64(i);
            }
            writer.Write(true);
            writer.Write(false);
            writer.Write(randomString[0]);
            writer.Write(randomString[1]);
            writer.Write(randomString.ToArray());
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
            writer.WritePrefixed((string)null);
            writer.WritePrefixed(buffer);
            writer.WriteZeroTerminated(randomString.Replace("\0", ""));
            writer.Write(randomString);
            writer.Write(buffer);
            writer.WriteEpoch32(dateTime);
            writer.WriteEpoch64(dateTime);
            bool supportsWriteLine = true;
            try
            {
                writer.WriteLine();
                writer.WriteLine("\r\r\r");
                writer.WriteLine(randomString.Replace("\r", "").Replace("\n", ""));
            }
            catch (InvalidOperationException)
            {
                if (writer.StringEncoding != StringEncoding.X_EUROPA && writer.Encoding.WebName.ToUpper() != "X-EUROPA")
                {
                    Assert.Fail($"{writer.Encoding.EncodingName} should support writeline!");
                }
                supportsWriteLine = false;
            }

            reader.BaseStream.Position = 0;

            for (int i = int.MaxValue; i > 0; i >>= 1)
            {
                Assert.AreEqual(-i, reader.Read7BitEncodedInt32());
                Assert.AreEqual(i, reader.Read7BitEncodedInt32());
            }
            for (long i = long.MaxValue; i > 0; i >>= 1)
            {
                Assert.AreEqual(-i, reader.Read7BitEncodedInt64());
                Assert.AreEqual(i, reader.Read7BitEncodedInt64());
            }
            Assert.AreEqual(true, reader.ReadBool());
            Assert.AreEqual(false, reader.ReadBool());
            Assert.AreEqual(randomString[0], reader.ReadChar());
            Assert.AreEqual(randomString[1], reader.ReadChar());
            {
                var readChars = reader.ReadChars(randomString.Length);
                CollectionAssert.AreEqual(randomString.ToCharArray(), readChars);
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
            {
                var readString = reader.ReadZeroTerminatedString(8 * randomString.Length);
                var expected = randomString.Replace("\0", "");
                CollectionAssert.AreEqual(expected.ToCharArray(), readString.ToCharArray());
                Assert.AreEqual(expected, readString);
            }
            {
                var readString = reader.ReadString(randomStringBytes.Length);
                CollectionAssert.AreEqual(randomString.ToCharArray(), readString.ToCharArray());
                Assert.AreEqual(randomString, readString);
            }
            CollectionAssert.AreEqual(buffer, reader.ReadBytes(buffer.Length));
            DateTime epoch = new DateTime(dateTime.Ticks - dateTime.Ticks % TimeSpan.TicksPerSecond);
            Assert.AreEqual(epoch, reader.ReadEpoch32());
            Assert.AreEqual(epoch, reader.ReadEpoch64());
            if (supportsWriteLine)
            {
                Assert.AreEqual("", reader.ReadLine());
                Assert.AreEqual("\r\r\r", reader.ReadLine());
                var expected = randomString.Replace("\r", "").Replace("\n", "");
                var readLine = reader.ReadLine();
                CollectionAssert.AreEqual(expected.ToCharArray(), readLine.ToCharArray());
                Assert.AreEqual(expected, readLine);
            }
        }
    }
}
