using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AbfSharpTests
{
    class HeaderValues
    {
        private readonly Dictionary<AbfSharp.ABFFIO.Structs.ABFFileHeader, AbfSharp.Header> AbfHeaders = new();

        [OneTimeSetUp()]
        public void LoadABFs()
        {
            foreach (string abfPath in SampleData.GetAllAbfPaths())
            {
                var officialHeader = new AbfSharp.ABFFIO.ABF(abfPath).Header;
                AbfHeaders[officialHeader] = (officialHeader.fFileVersionNumber < 2)
                    ? new AbfSharp.HeaderAbf1(abfPath)
                    : AbfHeaders[officialHeader] = new AbfSharp.HeaderAbf2(abfPath);
            }
        }

        private AbfSharp.Header GetLoadedHeader(string filename)
        {
            foreach (AbfSharp.Header header in AbfHeaders.Values)
                if (header.Filename == filename)
                    return header;

            throw new ArgumentException($"ABF file not loaded: {filename}");
        }

        [Test]
        public void Test_MatchesOfficial_Group01()
        {
            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.Header testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                Assert.AreEqual(officialHeader.fFileVersionNumber, testHeader.fFileVersionNumber);
                Assert.AreEqual(officialHeader.nOperationMode, testHeader.nOperationMode);
                Assert.AreEqual(officialHeader.lActualAcqLength, testHeader.lActualAcqLength);
                Assert.AreEqual(officialHeader.nNumPointsIgnored, testHeader.nNumPointsIgnored);

                // TODO: get this working for other operation modes
                if (testHeader.OperationMode == AbfSharp.OperationMode.Episodic)
                    Assert.AreEqual(officialHeader.lActualEpisodes, testHeader.lActualEpisodes);

                Assert.AreEqual(officialHeader.uFileStartDate, testHeader.uFileStartDate);
                Assert.AreEqual(officialHeader.uFileStartTimeMS, testHeader.uFileStartTimeMS);
                Assert.AreEqual(officialHeader.lStopwatchTime, testHeader.lStopwatchTime);

                if (testHeader.nFileType > 0)
                    Assert.AreEqual(officialHeader.nFileType, testHeader.nFileType);
            }
        }

        [TestCase("16921011-vc-memtest-tags.abf", "2016-09-21 14:43:46.434000")]
        [TestCase("17n16012-vc-steps.abf", "2017-11-16 14:05:01.627000")]
        [TestCase("17n16016-ic-ramp.abf", "2017-11-16 14:07:11.016000")]
        [TestCase("17n16016-ic-steps.abf", "2017-11-16 14:08:10.748000")]
        [TestCase("18808025-memtest.abf", "2018-08-08 13:49:04.826000")]
        public void Test_Creation_DateTime(string filename, string expectedDateTimeString)
        {
            AbfSharp.Header header = GetLoadedHeader(filename);
            DateTime expectedDateTime = DateTime.Parse(expectedDateTimeString);
            Console.WriteLine($"{header.StartDateTime} Date={header.uFileStartDate} TimeMS={header.uFileStartTimeMS}");
            Assert.AreEqual(expectedDateTime, header.StartDateTime);
        }

        [TestCase("16921011-vc-memtest-tags.abf", 20_000)]
        public void Test_SampleRate_MatchesKnownValue(string filename, int sampleRate)
        {
            AbfSharp.Header header = GetLoadedHeader(filename);
            Assert.AreEqual(sampleRate, header.SampleRate);
        }

        [TestCase("16921011-vc-memtest-tags.abf", new string[] { "+TGOT", "-TGOT" }, new double[] { 399.6672, 520.8576 })]
        public void Test_AbfsWithTags(string filename, string[] comments, double[] times)
        {
            AbfSharp.Header header = GetLoadedHeader(filename);
            for (int i = 0; i < header.Tags.Length; i++)
            {
                Assert.AreEqual(comments[i], header.Tags[i].Comment);
                Assert.AreEqual(times[i], header.Tags[i].Time, 1e-3);
            }
        }

        [Test]
        public void Test_MatchesOfficial_Group02()
        {
            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.Header testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                Assert.AreEqual(officialHeader.lDataSectionPtr, testHeader.lDataSectionPtr);
                Assert.AreEqual(officialHeader.lTagSectionPtr, testHeader.lTagSectionPtr);
                Assert.AreEqual(officialHeader.lNumTagEntries, testHeader.lNumTagEntries);
                Assert.AreEqual(officialHeader.lSynchArrayPtr, testHeader.lSynchArrayPtr);
                Assert.AreEqual(officialHeader.lSynchArraySize, testHeader.lSynchArraySize);
                Assert.AreEqual(officialHeader.nDataFormat, testHeader.nDataFormat);
                Assert.AreEqual(officialHeader.nSimultaneousScan, testHeader.nSimultaneousScan);
                Assert.AreEqual(officialHeader.lDACFilePtr, testHeader.lDACFilePtr);
                Assert.AreEqual(officialHeader.lDACFileNumEpisodes, testHeader.lDACFileNumEpisodes);

                // TODO: implement more sections
                //Assert.AreEqual(officialHeader.lScopeConfigPtr, testHeader.lScopeConfigPtr);
                //Assert.AreEqual(officialHeader.lNumScopes, testHeader.lNumScopes);
                //Assert.AreEqual(officialHeader.lDeltaArrayPtr, testHeader.lDeltaArrayPtr);
                //Assert.AreEqual(officialHeader.lNumDeltas, testHeader.lNumDeltas);
                //Assert.AreEqual(officialHeader.lVoiceTagPtr, testHeader.lVoiceTagPtr);
                //Assert.AreEqual(officialHeader.lVoiceTagEntries, testHeader.lVoiceTagEntries);
                //Assert.AreEqual(officialHeader.lStatisticsConfigPtr, testHeader.lStatisticsConfigPtr);
                //Assert.AreEqual(officialHeader.lAnnotationSectionPtr, testHeader.lAnnotationSectionPtr);
                //Assert.AreEqual(officialHeader.lNumAnnotations, testHeader.lNumAnnotations);
            }
        }

        [Test]
        public void Test_MatchesOfficial_Group03()
        {
            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.Header testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                Assert.AreEqual(officialHeader.nADCNumChannels, testHeader.nADCNumChannels);
                Assert.AreEqual(officialHeader.fADCSequenceInterval, testHeader.fADCSequenceInterval);
                Assert.AreEqual(officialHeader.fSynchTimeUnit, testHeader.fSynchTimeUnit);
                Assert.AreEqual(officialHeader.fSecondsPerRun, testHeader.fSecondsPerRun);

                if (testHeader.OperationMode == AbfSharp.OperationMode.Episodic)
                    Assert.AreEqual(officialHeader.lNumSamplesPerEpisode, testHeader.lNumSamplesPerEpisode);

                Assert.AreEqual(officialHeader.lPreTriggerSamples, testHeader.lPreTriggerSamples);
                Assert.AreEqual(officialHeader.lEpisodesPerRun, testHeader.lEpisodesPerRun);
                Assert.AreEqual(officialHeader.lRunsPerTrial, testHeader.lRunsPerTrial);
                Assert.AreEqual(officialHeader.lNumberOfTrials, testHeader.lNumberOfTrials);
                Assert.AreEqual(officialHeader.nAveragingMode, testHeader.nAveragingMode);
                Assert.AreEqual(officialHeader.nUndoRunCount, testHeader.nUndoRunCount);
                Assert.AreEqual(officialHeader.nFirstEpisodeInRun, testHeader.nFirstEpisodeInRun);
                Assert.AreEqual(officialHeader.fTriggerThreshold, testHeader.fTriggerThreshold);

                if (testHeader.fFileVersionNumber >= 2)
                    Assert.AreEqual(officialHeader.nTriggerSource, testHeader.nTriggerSource);

                Assert.AreEqual(officialHeader.nTriggerAction, testHeader.nTriggerAction);
                Assert.AreEqual(officialHeader.nTriggerPolarity, testHeader.nTriggerPolarity);
                Assert.AreEqual(officialHeader.fScopeOutputInterval, testHeader.fScopeOutputInterval);
                Assert.AreEqual(officialHeader.fEpisodeStartToStart, testHeader.fEpisodeStartToStart);
                Assert.AreEqual(officialHeader.fRunStartToStart, testHeader.fRunStartToStart);
                Assert.AreEqual(officialHeader.fTrialStartToStart, testHeader.fTrialStartToStart);
                Assert.AreEqual(officialHeader.lAverageCount, testHeader.lAverageCount);
                Assert.AreEqual(officialHeader.lLegacyClockChange, testHeader.lLegacyClockChange);
                Assert.AreEqual(officialHeader.nAutoTriggerStrategy, testHeader.nAutoTriggerStrategy);
            }
        }

        [Test]
        public void Test_MatchesOfficial_Group04()
        {
            // These values just adjust the display.
            // They're not important for AbfSHarp

            /*
            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 4 - Display Parameters
                Assert.AreEqual(officialHeader.nDataDisplayMode, testHeader.nDataDisplayMode);
                Assert.AreEqual(officialHeader.nChannelStatsStrategy, testHeader.nChannelStatsStrategy);
                Assert.AreEqual(officialHeader.lSamplesPerTrace, testHeader.lSamplesPerTrace);
                Assert.AreEqual(officialHeader.lStartDisplayNum, testHeader.lStartDisplayNum);
                Assert.AreEqual(officialHeader.lFinishDisplayNum, testHeader.lFinishDisplayNum);
                Assert.AreEqual(officialHeader.nShowPNRawData, testHeader.nShowPNRawData);
                Assert.AreEqual(officialHeader.fStatisticsPeriod, testHeader.fStatisticsPeriod);
                Assert.AreEqual(officialHeader.lStatisticsMeasurements, testHeader.lStatisticsMeasurements);
                Assert.AreEqual(officialHeader.nStatisticsSaveStrategy, testHeader.nStatisticsSaveStrategy);
            }*/
        }

        [Test]
        public void Test_MatchesOfficial_Group05()
        {
            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.Header testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                Assert.AreEqual(officialHeader.fADCRange, testHeader.fADCRange);
                Assert.AreEqual(officialHeader.lADCResolution, testHeader.lADCResolution);

                // ignore de devo ABFs with zeros for DAC
                if (testHeader.fDACRange == 0)
                    continue;

                Assert.AreEqual(officialHeader.fDACRange, testHeader.fDACRange);
                Assert.AreEqual(officialHeader.lDACResolution, testHeader.lDACResolution);
            }
        }

        [Test]
        public void Test_MatchesOfficial_Group06()
        {
            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.Header testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                Assert.AreEqual(officialHeader.nExperimentType, testHeader.nExperimentType);
                Assert.AreEqual(officialHeader.nManualInfoStrategy, testHeader.nManualInfoStrategy);
                Assert.AreEqual(officialHeader.fCellID1, testHeader.fCellID1);
                Assert.AreEqual(officialHeader.fCellID2, testHeader.fCellID2);
                Assert.AreEqual(officialHeader.fCellID3, testHeader.fCellID3);

                if (!string.IsNullOrWhiteSpace(officialHeader.sProtocolPath))
                    Assert.AreEqual(officialHeader.sProtocolPath.Trim(), testHeader.sProtocolPath);

                if (!string.IsNullOrWhiteSpace(officialHeader.sCreatorInfo))
                    Assert.AreEqual(officialHeader.sCreatorInfo.Trim(), testHeader.sCreatorInfo);

                if (!string.IsNullOrWhiteSpace(officialHeader.sModifierInfo))
                    Assert.AreEqual(officialHeader.sModifierInfo.Trim(), testHeader.sModifierInfo);

                if (!string.IsNullOrWhiteSpace(officialHeader.sFileComment))
                    Assert.AreEqual(officialHeader.sFileComment.Trim(), testHeader.sFileComment);

                if (!officialHeader.sCreatorInfo.Contains("FETCHEX"))
                {
                    Assert.AreEqual(officialHeader.nTelegraphEnable, testHeader.nTelegraphEnable);
                    Assert.AreEqual(officialHeader.nTelegraphInstrument, testHeader.nTelegraphInstrument);
                    Assert.AreEqual(officialHeader.fTelegraphAdditGain, testHeader.fTelegraphAdditGain);
                    Assert.AreEqual(officialHeader.fTelegraphFilter, testHeader.fTelegraphFilter);
                    Assert.AreEqual(officialHeader.fTelegraphMembraneCap, testHeader.fTelegraphMembraneCap);
                    Assert.AreEqual(officialHeader.nTelegraphMode, testHeader.nTelegraphMode);
                    Assert.AreEqual(officialHeader.nTelegraphDACScaleFactorEnable, testHeader.nTelegraphDACScaleFactorEnable.Take(officialHeader.nTelegraphDACScaleFactorEnable.Length));
                }

                if (officialHeader.FileGUID != Guid.Empty)
                    Assert.AreEqual(officialHeader.FileGUID, testHeader.FileGUID);

                for (int i = 0; i < officialHeader.fInstrumentHoldingLevel.Length; i++)
                    if (officialHeader.fInstrumentHoldingLevel[i] > 0)
                        Assert.AreEqual(officialHeader.fInstrumentHoldingLevel[i], testHeader.fInstrumentHoldingLevel[i]);
            }
        }

        [Test]
        public void Test_MatchesOfficial_Group07()
        {
            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.Header testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 7 - Multi-channel information
                Assert.AreEqual(officialHeader.nSignalType, testHeader.nSignalType);
                Assert.AreEqual(officialHeader.nADCPtoLChannelMap, testHeader.nADCPtoLChannelMap);
                Assert.AreEqual(officialHeader.nADCSamplingSeq, testHeader.nADCSamplingSeq);
                Assert.AreEqual(officialHeader.fADCProgrammableGain, testHeader.fADCProgrammableGain);
                Assert.AreEqual(officialHeader.fADCDisplayAmplification, testHeader.fADCDisplayAmplification);
                Assert.AreEqual(officialHeader.fADCDisplayOffset, testHeader.fADCDisplayOffset);
                Assert.AreEqual(officialHeader.fInstrumentScaleFactor, testHeader.fInstrumentScaleFactor);
                Assert.AreEqual(officialHeader.fInstrumentOffset, testHeader.fInstrumentOffset);
                Assert.AreEqual(officialHeader.fSignalGain, testHeader.fSignalGain);
                Assert.AreEqual(officialHeader.fSignalOffset, testHeader.fSignalOffset);
                Assert.AreEqual(officialHeader.fSignalLowpassFilter, testHeader.fSignalLowpassFilter);
                Assert.AreEqual(officialHeader.fSignalHighpassFilter, testHeader.fSignalHighpassFilter);
                Assert.AreEqual(officialHeader.nLowpassFilterType, testHeader.nLowpassFilterType);
                Assert.AreEqual(officialHeader.nHighpassFilterType, testHeader.nHighpassFilterType);
                Assert.AreEqual(officialHeader.sADCChannelName, testHeader.sADCChannelName);
                Assert.AreEqual(officialHeader.sADCUnits, testHeader.sADCUnits);
                Assert.AreEqual(officialHeader.fDACScaleFactor.Take(testHeader.fDACScaleFactor.Length), testHeader.fDACScaleFactor);
                Assert.AreEqual(officialHeader.fDACHoldingLevel.Take(testHeader.fDACHoldingLevel.Length), testHeader.fDACHoldingLevel);
                Assert.AreEqual(officialHeader.fDACCalibrationFactor.Take(testHeader.fDACCalibrationFactor.Length).ToArray(), testHeader.fDACCalibrationFactor);
                Assert.AreEqual(officialHeader.fDACCalibrationOffset.Take(testHeader.fDACCalibrationOffset.Length).ToArray(), testHeader.fDACCalibrationOffset);
                Assert.AreEqual(officialHeader.sDACChannelName.Take(testHeader.sDACChannelName.Length).ToArray(), testHeader.sDACChannelName);
                Assert.AreEqual(officialHeader.sDACChannelUnits.Take(testHeader.sDACChannelUnits.Length).ToArray(), testHeader.sDACChannelUnits);
            }
        }

        [Test]
        public void Test_MatchesOfficial_Group09()
        {
            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.Header testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 9 - Epoch Waveform and Pulses
                Assert.AreEqual(officialHeader.nDigitalEnable, testHeader.nDigitalEnable);
                Assert.AreEqual(officialHeader.nActiveDACChannel, testHeader.nActiveDACChannel);
                Assert.AreEqual(officialHeader.nDigitalDACChannel, testHeader.nDigitalDACChannel);
                Assert.AreEqual(officialHeader.nDigitalHolding, testHeader.nDigitalHolding);
                Assert.AreEqual(officialHeader.nDigitalInterEpisode, testHeader.nDigitalInterEpisode);
                Assert.AreEqual(officialHeader.nDigitalValue.Take(testHeader.nDigitalValue.Length), testHeader.nDigitalValue);
                Assert.AreEqual(officialHeader.bEpochCompression, testHeader.bEpochCompression);

                if (testHeader.fFileVersionNumber < 1.6)
                    continue;

                Assert.AreEqual(officialHeader.nDigitalTrainActiveLogic, testHeader.nDigitalTrainActiveLogic);
                Assert.AreEqual(officialHeader.nDigitalTrainValue.Take(testHeader.nDigitalTrainValue.Length), testHeader.nDigitalTrainValue);
                Assert.AreEqual(officialHeader.nInterEpisodeLevel.Take(testHeader.nInterEpisodeLevel.Length), testHeader.nInterEpisodeLevel);

                // HOW TO TEST THE EPOCH TABLE?
                // The epoch table read from disk is good.
                // ABFFIO is rediculous - it changes these values after reading them from the file.
                // It likes to change nEpochType
                // It also changes waveform levels according to nTelegraphDACScaleFactorEnable and fTelegraphAdditGain
                string[] ignoreAbfIDs =
                {
                    "18425108",
                    "18425108_abf1",
                    "18702001-biphasicTrain",
                    "18702001-cosTrain",
                    "18702001-pulseTrain",
                    "18702001-ramp",
                    "18702001-step",
                    "18702001-triangleTrain",
                    "19122043",
                    "2015_09_10_0001",
                    "2018_05_08_0028-IC-VC-pair",
                    "2018_12_15_0000",
                    "DM1_0000",
                    "DM1_0001",
                    "DM1_0002",
                    "DM1_0003",
                    "H19_29_150_11_21_01_0011",
                    "pclamp11_4ch",
                    "pclamp11_4ch_abf1",
                    "multichannelAbf1WithTags",
                    "File_axon_2"
                };
                if (ignoreAbfIDs.Contains(testHeader.AbfID))
                    continue;

                for (int i = 0; i < testHeader.nWaveformEnable.Length; i++)
                {
                    Assert.AreEqual(officialHeader.nWaveformEnable[i], testHeader.nWaveformEnable[i]);
                    Assert.AreEqual(officialHeader.nWaveformSource[i], testHeader.nWaveformSource[i]);
                }

                // validate the waveform table only for episodic files
                for (int i = 0; i < testHeader.nEpochType.Length; i++)
                {
                    // ABFFIO drops all types down to 2, but more complex types exist in file
                    Assert.AreEqual(officialHeader.nEpochType[i], testHeader.nEpochType[i]);

                    // I think for some ABFs this needs to be scaled by fTelegraphAdditGain
                    // but only if nTelegraphDACScaleFactorEnable is enabled
                    Assert.AreEqual(officialHeader.fEpochInitLevel[i], testHeader.fEpochInitLevel[i]);
                    Assert.AreEqual(officialHeader.fEpochLevelInc[i], testHeader.fEpochLevelInc[i]);

                    Assert.AreEqual(officialHeader.lEpochDurationInc[i], testHeader.lEpochDurationInc[i]);
                    Assert.AreEqual(officialHeader.lEpochInitDuration[i], testHeader.lEpochInitDuration[i]);
                }
            }
        }

        [Test]
        public void Test_MatchesOfficial_Group10()
        {
            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.Header testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                if (testHeader.fFileVersionNumber < 1.6)
                    continue;

                //region GROUP 10 - DAC Output File
                Assert.AreEqual(officialHeader.fDACFileScale.Take(testHeader.fDACFileScale.Length), testHeader.fDACFileScale);
                Assert.AreEqual(officialHeader.fDACFileOffset.Take(testHeader.fDACFileOffset.Length), testHeader.fDACFileOffset);
                Assert.AreEqual(officialHeader.lDACFileEpisodeNum.Take(testHeader.lDACFileEpisodeNum.Length), testHeader.lDACFileEpisodeNum);
                Assert.AreEqual(officialHeader.nDACFileADCNum.Take(testHeader.nDACFileADCNum.Length), testHeader.nDACFileADCNum);
                Assert.AreEqual(officialHeader.sDACFilePath.Take(testHeader.sDACFilePath.Length), testHeader.sDACFilePath);
            }
        }
    }
}
