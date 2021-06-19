using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfSharpTests.RawAbf
{
    class HeaderValues
    {
        [Test]
        public void Test_MatchesOfficial_FileVersion()
        {
            foreach (string abfFilePath in SampleData.GetAllAbfPaths())
            {
                var official = new AbfSharp.ABF(abfFilePath);
                var raw = new AbfSharp.RawABF(abfFilePath);
                Console.WriteLine(System.IO.Path.GetFileName(abfFilePath));
                Assert.AreEqual(official.Header.HeaderStruct.fFileVersionNumber, raw.Header.FileVersionNumber);
            }
        }

        [Test]
        public void Test_MatchesOfficial_OperationMode()
        {
            foreach (string abfFilePath in SampleData.GetAllAbfPaths())
            {
                var official = new AbfSharp.ABF(abfFilePath);
                var raw = new AbfSharp.RawABF(abfFilePath);
                Console.WriteLine(System.IO.Path.GetFileName(abfFilePath));
                Assert.AreEqual(official.Header.HeaderStruct.nOperationMode, (int)raw.Header.OperationMode);
            }
        }

        [Test]
        public void Test_MatchesOfficial_GUID()
        {
            foreach (string abfFilePath in SampleData.GetAllAbfPaths())
            {
                var official = new AbfSharp.ABF(abfFilePath);
                var raw = new AbfSharp.RawABF(abfFilePath);
                Console.WriteLine(System.IO.Path.GetFileName(abfFilePath));

                // ignore ABFs without an official GUID (weird?)
                if (official.Header.HeaderStruct.FileGUID == Guid.Empty)
                    continue;

                Assert.AreEqual(official.Header.HeaderStruct.FileGUID, raw.Header.GUID);
            }
        }
    }
}
