using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AbfSharpTests
{
    class Abffio
    {
        private static string GetMD5Hash(string filePath)
        {
            var md5 = MD5.Create();
            using var stream = File.OpenRead(filePath);
            return string.Join("", md5.ComputeHash(stream).Select(x => x.ToString("x2")));
        }

        [Test]
        public void Test_ABFFIO_DllHash()
        {
            // hash generated from Windows command line: certutil -hashfile ABFFIO.dll MD5
            // tested on ABFFIO.dll in the ABF FileSupportPack distributed with pCLAMP11.2
            Assert.AreEqual("08ddd0757f32ed733f958526693ccd66", GetMD5Hash("ABFFIO.dll"));
        }
    }
}
