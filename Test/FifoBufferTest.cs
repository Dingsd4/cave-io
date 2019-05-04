using Cave.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [TestClass]
    public class FifoBufferTest
    {
        [TestMethod]
        public void Test1()
        {
            var buffer = new FifoBuffer();
            buffer.Enqueue(Encoding.ASCII.GetBytes("1234"));
            buffer.Enqueue(Encoding.ASCII.GetBytes("5"));
            buffer.Enqueue(Encoding.ASCII.GetBytes("678"));
            buffer.Enqueue(Encoding.ASCII.GetBytes("90"));
            Assert.AreEqual(buffer.Length, 10);
            Assert.AreEqual("1234567890", Encoding.ASCII.GetString(buffer.ToArray()));
            Assert.AreEqual(buffer.Length, 10);
            for(int i = 1; i <= 10; i++)
            {
                var t = buffer.Dequeue(1);
                Assert.AreEqual(1, t.Length);
                Assert.AreEqual((byte)('0' + (i % 10)), t[0]);
                Assert.AreEqual(10 - i, buffer.Length);
            }
            buffer.Clear();
            Assert.AreEqual(buffer.Length, 0);
        }

        [TestMethod]
        public void Test2()
        {
            var buffer = new FifoBuffer();
            buffer.Enqueue(new MemoryStream(Encoding.ASCII.GetBytes("1234")), 4);
            buffer.Enqueue(new MemoryStream(Encoding.ASCII.GetBytes("5")), 1);
            buffer.Enqueue(new MemoryStream(Encoding.ASCII.GetBytes("678")), 3);
            buffer.Enqueue(new MemoryStream(Encoding.ASCII.GetBytes("90")), 2);
            Assert.AreEqual(buffer.Length, 10);
            Assert.AreEqual("1234567890", Encoding.ASCII.GetString(buffer.Dequeue(buffer.Length)));
            Assert.AreEqual(buffer.Length, 0);
        }
    }
}
