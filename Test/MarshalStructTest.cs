using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Cave;
using Cave.IO;
using Test.Collections;

namespace Cave.IO
{
    [TestFixture]
    public class MarshalStructTest
    {
        [Test]
        public void Test_MarshalStruct_1()
        {
            InteropTestStruct l_Test = InteropTestStruct.Create(1);

            byte[] data = MarshalStruct.GetBytes(l_Test);
            InteropTestStruct result1 = MarshalStruct.GetStruct<InteropTestStruct>(data);
            Assert.AreEqual(l_Test, result1);

            MemoryStream stream = new MemoryStream();
            MarshalStruct.Write(stream, l_Test);
            stream.Position = 0;
            InteropTestStruct result2 = MarshalStruct.Read<InteropTestStruct>(stream);
            Assert.AreEqual(l_Test, result2);

            byte[] buffer = new byte[100000];
            MarshalStruct.Write(l_Test, buffer, 1024);
            InteropTestStruct result3 = MarshalStruct.Read<InteropTestStruct>(buffer, 1024);
            Assert.AreEqual(l_Test, result3);
        }
    }
}
