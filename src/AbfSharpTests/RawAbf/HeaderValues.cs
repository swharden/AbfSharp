using NUnit.Framework;
using System;
using System.Linq;
using System.Text;

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

        private string ArrayString<T>(T[] arr) => "[" + string.Join(", ", arr.Select(x => x.ToString())) + "]";

        [Test]
        public void Test_MatchesOfficial_AdcSection()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var officialHeader = official.Header.HeaderStruct;
                var raw = new AbfSharp.RawABF(official.Path);
                Console.WriteLine($"ABF: {System.IO.Path.GetFileName(official.Path)} Ver={raw.Header.FileVersionNumber}");

                if (officialHeader.fFileVersionNumber >= 2)
                {
                    // telegraph settings
                    Assert.AreEqual(officialHeader.nTelegraphEnable, raw.Header.Abf2Header.AdcSection.nTelegraphEnable);
                    Assert.AreEqual(officialHeader.nTelegraphInstrument, raw.Header.Abf2Header.AdcSection.nTelegraphInstrument);
                    Assert.AreEqual(officialHeader.fTelegraphAdditGain, raw.Header.Abf2Header.AdcSection.fTelegraphAdditGain);
                    Assert.AreEqual(officialHeader.fTelegraphFilter, raw.Header.Abf2Header.AdcSection.fTelegraphFilter);
                    Assert.AreEqual(officialHeader.fTelegraphMembraneCap, raw.Header.Abf2Header.AdcSection.fTelegraphMembraneCap);
                    Assert.AreEqual(officialHeader.nTelegraphMode, raw.Header.Abf2Header.AdcSection.nTelegraphMode);
                    Assert.AreEqual(officialHeader.fTelegraphAccessResistance, raw.Header.Abf2Header.AdcSection.fTelegraphAccessResistance);

                    // mapping
                    Assert.AreEqual(officialHeader.nADCPtoLChannelMap, raw.Header.Abf2Header.AdcSection.nADCPtoLChannelMap);

                    // scaling
                    Assert.AreEqual(officialHeader.fADCProgrammableGain, raw.Header.Abf2Header.AdcSection.fADCProgrammableGain);
                    Assert.AreEqual(officialHeader.fADCDisplayAmplification, raw.Header.Abf2Header.AdcSection.fADCDisplayAmplification);
                    Assert.AreEqual(officialHeader.fADCDisplayOffset, raw.Header.Abf2Header.AdcSection.fADCDisplayOffset);
                    Assert.AreEqual(officialHeader.fInstrumentScaleFactor, raw.Header.Abf2Header.AdcSection.fInstrumentScaleFactor);
                    Assert.AreEqual(officialHeader.fInstrumentOffset, raw.Header.Abf2Header.AdcSection.fInstrumentOffset);
                    Assert.AreEqual(officialHeader.fSignalGain, raw.Header.Abf2Header.AdcSection.fSignalGain);
                    Assert.AreEqual(officialHeader.fSignalOffset, raw.Header.Abf2Header.AdcSection.fSignalOffset);

                    // filtering
                    Assert.AreEqual(officialHeader.fSignalLowpassFilter, raw.Header.Abf2Header.AdcSection.fSignalLowpassFilter);
                    Assert.AreEqual(officialHeader.fSignalHighpassFilter, raw.Header.Abf2Header.AdcSection.fSignalHighpassFilter);
                    Assert.AreEqual(officialHeader.fPostProcessLowpassFilter, raw.Header.Abf2Header.AdcSection.fPostProcessLowpassFilter);
                }
                else
                {
                    // mapping
                    Assert.AreEqual(officialHeader.nADCPtoLChannelMap, raw.Header.Abf1Header.nADCPtoLChannelMap);

                    // scaling
                    Assert.AreEqual(officialHeader.fADCProgrammableGain, raw.Header.Abf1Header.fADCProgrammableGain);
                    Assert.AreEqual(officialHeader.fInstrumentScaleFactor, raw.Header.Abf1Header.fInstrumentScaleFactor);
                    Assert.AreEqual(officialHeader.fInstrumentOffset, raw.Header.Abf1Header.fInstrumentOffset);
                    Assert.AreEqual(officialHeader.fSignalGain, raw.Header.Abf1Header.fSignalGain);
                    Assert.AreEqual(officialHeader.fSignalOffset, raw.Header.Abf1Header.fSignalOffset);
                }
            }
        }

        [Test]
        public void Test_MatchesOfficial_nADCPtoLChannelMap()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.Path);
                Console.WriteLine($"{System.IO.Path.GetFileName(official.Path)} Ver={raw.Header.FileVersionNumber}");
                Assert.AreEqual(
                    expected: ArrayString(official.Header.HeaderStruct.nADCPtoLChannelMap),
                    actual: ArrayString(raw.Header.nADCPtoLChannelMap)
                );
            }
        }

        [Test]
        public void Test_MatchesOfficial_AdcScalingVariables()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.Path);
                var officialHeader = official.Header.HeaderStruct;

                Console.WriteLine($"{System.IO.Path.GetFileName(official.Path)} {raw.Header.FileVersionNumber}");

                for (int channelIndex = 0; channelIndex < official.ChannelCount; channelIndex++)
                {
                    var thisChannelInfo = raw.Header.AdcDataInfo[channelIndex];
                    Assert.AreEqual(officialHeader.nDataFormat, thisChannelInfo.nDataFormat);
                    Assert.AreEqual(officialHeader.fInstrumentOffset[channelIndex], thisChannelInfo.fInstrumentOffset);
                    Assert.AreEqual(officialHeader.fSignalOffset[channelIndex], thisChannelInfo.fSignalOffset);
                    Assert.AreEqual(officialHeader.fInstrumentScaleFactor[channelIndex], thisChannelInfo.fInstrumentScaleFactor);
                    Assert.AreEqual(officialHeader.fSignalGain[channelIndex], thisChannelInfo.fSignalGain);
                    Assert.AreEqual(officialHeader.fADCProgrammableGain[channelIndex], thisChannelInfo.fADCProgrammableGain);
                    Assert.AreEqual(officialHeader.lADCResolution, thisChannelInfo.lADCResolution);
                    Assert.AreEqual(officialHeader.fADCRange, thisChannelInfo.fADCRange);
                }
            }
        }

        [Test]
        public void Test_MatchesOfficial_Creator()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.Path);

                Assert.AreEqual(official.Header.HeaderStruct.sCreatorInfo.Trim(), raw.Header.Creator);

                string officialCreatorVersion = string.Join(".", new Int16[]
                {
                    official.Header.HeaderStruct.nCreatorMajorVersion,
                    official.Header.HeaderStruct.nCreatorMinorVersion,
                    official.Header.HeaderStruct.nCreatorBugfixVersion,
                    official.Header.HeaderStruct.nCreatorBuildVersion
                });

                Assert.AreEqual(officialCreatorVersion, raw.Header.CreatorVersion);
            }
        }

        [Test]
        public void Test_MatchesOfficial_Modifier()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.Path);
                Console.WriteLine($"{System.IO.Path.GetFileName(official.Path)} {raw.Header.FileVersionNumber}");

                if (string.IsNullOrWhiteSpace(official.Header.HeaderStruct.sModifierInfo) == false)
                    Assert.AreEqual(official.Header.HeaderStruct.sModifierInfo.Trim(), raw.Header.Modifier);

                string officialVersion = string.Join(".", new Int16[]
                {
                    official.Header.HeaderStruct.nModifierMajorVersion,
                    official.Header.HeaderStruct.nModifierMinorVersion,
                    official.Header.HeaderStruct.nModifierBugfixVersion,
                    official.Header.HeaderStruct.nModifierBuildVersion
                });

                Assert.AreEqual(officialVersion, raw.Header.ModifierVersion);
            }
        }

        [Test]
        public void Test_MatchesOfficial_FileStart()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.Path);
                Console.WriteLine($"{System.IO.Path.GetFileName(official.Path)} {raw.Header.FileVersionNumber} {raw.Header.uFileStartDate} {raw.Header.FileStart}");

                Assert.AreEqual(official.Header.HeaderStruct.uFileStartDate, raw.Header.uFileStartDate);
                Assert.AreEqual(official.Header.HeaderStruct.uFileStartTimeMS, raw.Header.uFileStartTimeMS);
            }
        }

        [Test]
        public void Test_MatchesOfficial_fDACHoldingLevel()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.Path);
                Console.WriteLine($"{System.IO.Path.GetFileName(official.Path)} {raw.Header.FileVersionNumber} {raw.Header.uFileStartDate} {raw.Header.FileStart}");

                Assert.AreEqual(official.Header.HeaderStruct.fDACHoldingLevel, raw.Header.fDACHoldingLevel);
            }
        }

        [Test]
        public void Test_MatchesOfficial_sProtocolPath()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.Path);
                Console.WriteLine($"{System.IO.Path.GetFileName(official.Path)} {raw.Header.FileVersionNumber} {raw.Header.uFileStartDate} {raw.Header.FileStart}");
                Assert.AreEqual(official.Header.HeaderStruct.sProtocolPath.Trim(), raw.Header.sProtocolPath);
            }
        }
    }
}
