using NUnit.Framework;
using System;
using System.Linq;

namespace AbfSharpTests.RawAbf
{
    class HeaderValues
    {
        private AbfSharp.ABF[] OfficialABFs;

        [OneTimeSetUp()]
        public void LoadOfficialABFs()
        {
            OfficialABFs = SampleData.GetAllAbfPaths().Select(x => new AbfSharp.ABF(x)).ToArray();
        }

        [Test]
        public void Test_MatchesOfficial_FileVersion()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.Path);

                Assert.AreEqual(official.Header.HeaderStruct.fFileVersionNumber, raw.Header.FileVersionNumber);
            }
        }

        [Test]
        public void Test_MatchesOfficial_OperationMode()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.Path);

                Assert.AreEqual(official.Header.HeaderStruct.nOperationMode, (int)raw.Header.OperationMode);
            }
        }

        [Test]
        public void Test_MatchesOfficial_GUID()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.Path);

                // ignore ABFs without an official GUID (weird?)
                if (official.Header.HeaderStruct.FileGUID == Guid.Empty)
                    continue;

                Assert.AreEqual(official.Header.HeaderStruct.FileGUID, raw.Header.GUID);
            }
        }

        [Test]
        public void Test_MatchesOfficial_ChannelCount()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.Path);

                Assert.AreEqual(official.Header.HeaderStruct.nADCNumChannels, raw.Header.ChannelCount);
            }
        }
    }
}
