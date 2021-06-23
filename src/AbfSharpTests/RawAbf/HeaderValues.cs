﻿using NUnit.Framework;
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
                var raw = new AbfSharp.RawABF(official.FilePath);

                Assert.AreEqual(official.Header.HeaderStruct.fFileVersionNumber, raw.Header.FileVersionNumber);
            }
        }

        [Test]
        public void Test_MatchesOfficial_OperationMode()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.FilePath);

                Assert.AreEqual(official.Header.HeaderStruct.nOperationMode, (int)raw.Header.OperationMode);
            }
        }

        [Test]
        public void Test_MatchesOfficial_GUID()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.FilePath);

                // ignore ABFs without an official GUID (weird?)
                if (official.Header.HeaderStruct.FileGUID == Guid.Empty)
                    continue;

                Assert.AreEqual(official.Header.HeaderStruct.FileGUID, raw.Header.FileGUID);
            }
        }

        [Test]
        public void Test_MatchesOfficial_ChannelCount()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.FilePath);

                Assert.AreEqual(official.Header.HeaderStruct.nADCNumChannels, raw.Header.ChannelCount);
            }
        }

        [Test]
        public void Test_MatchesOfficial_SweepCount()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.FilePath);

                // TODO: support other operation modes
                if (raw.Header.OperationMode != AbfSharp.HeaderData.OperationMode.Episodic)
                    continue;

                Assert.AreEqual(official.Header.HeaderStruct.lActualEpisodes, raw.Header.lActualEpisodes);
            }
        }

        private string ArrayString<T>(T[] arr) => "[" + string.Join(", ", arr.Select(x => x.ToString())) + "]";

        [Test]
        public void Test_MatchesOfficial_AdcSection()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var officialHeader = official.Header.HeaderStruct;
                var raw = new AbfSharp.RawABF(official.FilePath);

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
        public void Test_MatchesOfficial_ChannelMap()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.FilePath);

                Assert.AreEqual(
                    expected: ArrayString(official.Header.HeaderStruct.nADCPtoLChannelMap),
                    actual: ArrayString(raw.Header.nADCPtoLChannelMap)
                );

                Assert.AreEqual(
                    expected: ArrayString(official.Header.HeaderStruct.nADCSamplingSeq),
                    actual: ArrayString(raw.Header.nADCSamplingSeq)
                );
            }
        }

        [Test]
        public void Test_MatchesOfficial_Creator()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.FilePath);

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
                var raw = new AbfSharp.RawABF(official.FilePath);

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
                var raw = new AbfSharp.RawABF(official.FilePath);
                Assert.AreEqual(official.Header.HeaderStruct.uFileStartDate, raw.Header.uFileStartDate);
                Assert.AreEqual(official.Header.HeaderStruct.uFileStartTimeMS, raw.Header.uFileStartTimeMS);
            }
        }

        [Test]
        public void Test_MatchesOfficial_fDACHoldingLevel()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.FilePath);
                Assert.AreEqual(official.Header.HeaderStruct.fDACHoldingLevel, raw.Header.fDACHoldingLevel);
            }
        }

        [Test]
        public void Test_MatchesOfficial_sProtocolPath()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.FilePath);
                Assert.AreEqual(official.Header.HeaderStruct.sProtocolPath.Trim(), raw.Header.sProtocolPath);
            }
        }

        [Test]
        public void Test_MatchesOfficial_sFileComment()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.FilePath);
                Assert.AreEqual(official.Header.HeaderStruct.sFileComment.Trim(), raw.Header.sFileComment);
            }
        }

        [Test]
        public void Test_MatchesOfficial_SampleRate()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.FilePath);
                Assert.AreEqual(1e6 / official.Header.HeaderStruct.fADCSequenceInterval, raw.Header.SampleRate);
            }
        }

        [Test]
        public void Test_MatchesOfficial_TagHeaderValues()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.FilePath);
                Assert.AreEqual(official.Header.HeaderStruct.lTagSectionPtr, raw.Header.lTagSectionPtr);
                Assert.AreEqual(official.Header.HeaderStruct.lNumTagEntries, raw.Header.lNumTagEntries);

                // Converting tag positions to tag times requires a multiplier.
                // These checks verify the header values the multiplier depends on are accurate.
                Assert.AreEqual(official.Header.HeaderStruct.fADCSequenceInterval, raw.Header.fADCSequenceInterval);
                Assert.AreEqual(official.Header.HeaderStruct.nADCNumChannels, raw.Header.nADCNumChannels);

                // Tag times may be in are in fSynchTimeUnit units.
                Assert.AreEqual(official.Header.HeaderStruct.fSynchTimeUnit, raw.Header.fSynchTimeUnit);

                if (raw.Header.fSynchTimeUnit != 0)
                {
                    // TODO: clarify how fSynchTimeUnit can be calculated
                    double samplePeriod = 1e6 / raw.Header.SampleRate;
                    Console.WriteLine($"{raw.Header.fSynchTimeUnit:00.0}\t{samplePeriod}\t{System.IO.Path.GetFileName(official.FilePath)}");
                    //double fs = 1e6 / raw.Header.SampleRate / raw.Header.ChannelCount / raw.Header.BytesPerDataPoint;
                    //Assert.AreEqual(raw.Header.fSynchTimeUnit, fs);
                }
            }
        }

        [Test]
        public void Test_MatchesOfficial_DataHeaderValues()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                Console.WriteLine($"v={official.Header.HeaderStruct.fFileVersionNumber:0.0} {System.IO.Path.GetFileName(official.FilePath)}");
                var raw = new AbfSharp.RawABF(official.FilePath);
                Assert.AreEqual(official.Header.HeaderStruct.lDataSectionPtr, raw.Header.lDataSectionPtr);
                Assert.AreEqual(official.Header.HeaderStruct.nDataFormat, raw.Header.nDataFormat);
            }
        }

        [Test]
        public void Test_MatchesOfficial_SyncSection()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                Console.WriteLine($"v={official.Header.HeaderStruct.fFileVersionNumber:0.0} {System.IO.Path.GetFileName(official.FilePath)}");
                var raw = new AbfSharp.RawABF(official.FilePath);
                Assert.AreEqual(official.Header.HeaderStruct.lSynchArrayPtr, raw.Header.lSynchArrayPtr);
                Assert.AreEqual(official.Header.HeaderStruct.lSynchArraySize, raw.Header.lSynchArraySize);
                Assert.AreEqual(official.Header.HeaderStruct.fSynchTimeUnit, raw.Header.fSynchTimeUnit);

                if (raw.Header.fSynchTimeUnit > 0)
                    Assert.AreEqual(raw.Header.lActualEpisodes, raw.Header.SynchStartTimes.Length);
            }
        }

        [Test]
        public void Test_MatchesOfficial_lActualAcqLength()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                Console.WriteLine($"v={official.Header.HeaderStruct.fFileVersionNumber:0.0} {System.IO.Path.GetFileName(official.FilePath)}");
                var raw = new AbfSharp.RawABF(official.FilePath);
                Assert.AreEqual(official.Header.HeaderStruct.lActualAcqLength, raw.Header.lActualAcqLength);
            }
        }


        [Test]
        public void Test_MatchesOfficial_TelegraphOptions()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                Console.WriteLine($"v={official.Header.HeaderStruct.fFileVersionNumber:0.0} {System.IO.Path.GetFileName(official.FilePath)}");
                var raw = new AbfSharp.RawABF(official.FilePath);

                for (int i=0; i<8; i++)
                {
                    Assert.AreEqual(official.Header.HeaderStruct.nTelegraphEnable[i], raw.Header.nTelegraphEnable[i]);
                    Assert.AreEqual(official.Header.HeaderStruct.nTelegraphInstrument[i], raw.Header.nTelegraphInstrument[i]);
                    Assert.AreEqual(official.Header.HeaderStruct.fTelegraphAdditGain[i], raw.Header.fTelegraphAdditGain[i]);
                    Assert.AreEqual(official.Header.HeaderStruct.fTelegraphFilter[i], raw.Header.fTelegraphFilter[i]);
                    Assert.AreEqual(official.Header.HeaderStruct.fTelegraphMembraneCap[i], raw.Header.fTelegraphMembraneCap[i]);
                    Assert.AreEqual(official.Header.HeaderStruct.nTelegraphMode[i], raw.Header.nTelegraphMode[i]);
                    Assert.AreEqual(official.Header.HeaderStruct.nTelegraphDACScaleFactorEnable[i], raw.Header.nTelegraphDACScaleFactorEnable[i]);
                }
            }
        }
    }
}
