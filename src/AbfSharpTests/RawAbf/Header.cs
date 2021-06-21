using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfSharpTests.RawAbf
{
    class Header
    {
        private readonly Dictionary<AbfSharp.ABFFIO.Structs.ABFFileHeader, AbfSharp.HeaderBase> AbfHeaders = new();

        [OneTimeSetUp()]
        public void LoadABFs()
        {
            foreach (string abfPath in SampleData.GetAllAbfPaths())
            {
                var officialHeader = new AbfSharp.ABF(abfPath).Header.HeaderStruct;
                AbfHeaders[officialHeader] = (officialHeader.fFileVersionNumber < 2)
                    ? new AbfSharp.HeaderAbf1(abfPath)
                    : AbfHeaders[officialHeader] = new AbfSharp.HeaderAbf2(abfPath);
            }
        }

        [Test]
        public void Test_MatchesOfficial_Group1()
        {
            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                Assert.AreEqual(officialHeader.fFileVersionNumber, testHeader.fFileVersionNumber);
                Assert.AreEqual(officialHeader.nOperationMode, testHeader.nOperationMode);
                Assert.AreEqual(officialHeader.lActualAcqLength, testHeader.lActualAcqLength);
                Assert.AreEqual(officialHeader.nNumPointsIgnored, testHeader.nNumPointsIgnored);

                // TODO: get this working for other operation modes
                if (testHeader.OperationMode == AbfSharp.HeaderData.OperationMode.Episodic)
                    Assert.AreEqual(officialHeader.lActualEpisodes, testHeader.lActualEpisodes);

                Assert.AreEqual(officialHeader.uFileStartDate, testHeader.uFileStartDate);
                Assert.AreEqual(officialHeader.uFileStartTimeMS, testHeader.uFileStartTimeMS);
                Assert.AreEqual(officialHeader.lStopwatchTime, testHeader.lStopwatchTime);

                if (testHeader.nFileType > 0)
                    Assert.AreEqual(officialHeader.nFileType, testHeader.nFileType);
            }
        }
    }
}
