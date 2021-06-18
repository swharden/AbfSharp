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
        public void Test_MatchesOfficial_FileMajorVersion()
        {
            foreach (string abfFilePath in SampleData.GetAllAbfPaths())
            {
                var official = new AbfSharp.ABF(abfFilePath);
                var raw = new AbfSharp.RawABF(abfFilePath);

                if (official.Header.HeaderStruct.fFileVersionNumber < 2)
                    Assert.AreEqual(1, raw.AbfVersion.Major);
                else
                    Assert.AreEqual(2, raw.AbfVersion.Major);
            }
        }
    }
}
