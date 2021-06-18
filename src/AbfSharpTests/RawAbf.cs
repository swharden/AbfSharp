using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfSharpTests
{
    class RawAbf
    {
        [Test]
        public void Test_MatchesOfficial_FileVersion()
        {
            foreach (string abfFilePath in SampleData.GetAllAbfPaths())
            {
                var official = new AbfSharp.ABF(abfFilePath);
                var raw = new AbfSharp.RawABF(abfFilePath);

                Assert.AreEqual((int)official.Header.HeaderStruct.fFileVersionNumber, (int)raw.FileVersion);
            }
        }

        [Test]
        public void Test_MatchesOfficial_OperationMode()
        {
            foreach (string abfFilePath in SampleData.GetAllAbfPaths())
            {
                var official = new AbfSharp.ABF(abfFilePath);
                var raw = new AbfSharp.RawABF(abfFilePath);
                Assert.AreEqual(official.Header.HeaderStruct.nOperationMode, (int)raw.OperationMode);
            }
        }
    }
}
