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
        public void Test_MatchesOfficial_Group01()
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

        [Test]
        public void Test_MatchesOfficial_Group02()
        {
            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
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
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                Assert.AreEqual(officialHeader.nADCNumChannels, testHeader.nADCNumChannels);
                Assert.AreEqual(officialHeader.fADCSequenceInterval, testHeader.fADCSequenceInterval);
                Assert.AreEqual(officialHeader.fSynchTimeUnit, testHeader.fSynchTimeUnit);
                Assert.AreEqual(officialHeader.fSecondsPerRun, testHeader.fSecondsPerRun);

                if (testHeader.OperationMode == AbfSharp.HeaderData.OperationMode.Episodic)
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
                AbfSharp.HeaderBase testHeader = dict.Value;
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
                AbfSharp.HeaderBase testHeader = dict.Value;
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
                AbfSharp.HeaderBase testHeader = dict.Value;
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
                AbfSharp.HeaderBase testHeader = dict.Value;
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

        [Ignore("group not yet implemented")]
        [Test]
        public void Test_MatchesOfficial_Group10()
        {

            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 10 - DAC Output File
                Assert.AreEqual(officialHeader.fDACFileScale, testHeader.fDACFileScale);
                Assert.AreEqual(officialHeader.fDACFileOffset, testHeader.fDACFileOffset);
                Assert.AreEqual(officialHeader.lDACFileEpisodeNum, testHeader.lDACFileEpisodeNum);
                Assert.AreEqual(officialHeader.nDACFileADCNum, testHeader.nDACFileADCNum);
                Assert.AreEqual(officialHeader.sDACFilePath, testHeader.sDACFilePath);
            }
        }

        [Ignore("group not yet implemented")]
        [Test]
        public void Test_MatchesOfficial_Group11()
        {

            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 11a - Presweep (conditioning) pulse train
                Assert.AreEqual(officialHeader.nConditEnable, testHeader.nConditEnable);
                Assert.AreEqual(officialHeader.lConditNumPulses, testHeader.lConditNumPulses);
                Assert.AreEqual(officialHeader.fBaselineDuration, testHeader.fBaselineDuration);
                Assert.AreEqual(officialHeader.fBaselineLevel, testHeader.fBaselineLevel);
                Assert.AreEqual(officialHeader.fStepDuration, testHeader.fStepDuration);
                Assert.AreEqual(officialHeader.fStepLevel, testHeader.fStepLevel);
                Assert.AreEqual(officialHeader.fPostTrainPeriod, testHeader.fPostTrainPeriod);
                Assert.AreEqual(officialHeader.fPostTrainLevel, testHeader.fPostTrainLevel);
                Assert.AreEqual(officialHeader.fCTStartLevel, testHeader.fCTStartLevel);
                Assert.AreEqual(officialHeader.fCTEndLevel, testHeader.fCTEndLevel);
                Assert.AreEqual(officialHeader.fCTIntervalDuration, testHeader.fCTIntervalDuration);
                Assert.AreEqual(officialHeader.fCTStartToStartInterval, testHeader.fCTStartToStartInterval);

                //region GROUP 11b - Membrane Test Between Sweeps
                Assert.AreEqual(officialHeader.nMembTestEnable, testHeader.nMembTestEnable);
                Assert.AreEqual(officialHeader.fMembTestPreSettlingTimeMS, testHeader.fMembTestPreSettlingTimeMS);
                Assert.AreEqual(officialHeader.fMembTestPostSettlingTimeMS, testHeader.fMembTestPostSettlingTimeMS);

                //region GROUP 11c - PreSignal test pulse
                Assert.AreEqual(officialHeader.nPreSignalEnable, testHeader.nPreSignalEnable);
                Assert.AreEqual(officialHeader.fPreSignalPreStepDuration, testHeader.fPreSignalPreStepDuration);
                Assert.AreEqual(officialHeader.fPreSignalPreStepLevel, testHeader.fPreSignalPreStepLevel);
                Assert.AreEqual(officialHeader.fPreSignalStepDuration, testHeader.fPreSignalStepDuration);
                Assert.AreEqual(officialHeader.fPreSignalStepLevel, testHeader.fPreSignalStepLevel);
                Assert.AreEqual(officialHeader.fPreSignalPostStepDuration, testHeader.fPreSignalPostStepDuration);
                Assert.AreEqual(officialHeader.fPreSignalPostStepLevel, testHeader.fPreSignalPostStepLevel);

                //region GROUP 11d - Hum Silncer Adapt between sweeps
                Assert.AreEqual(officialHeader.nAdaptEnable, testHeader.nAdaptEnable);
                Assert.AreEqual(officialHeader.fInterSweepAdaptTimeS, testHeader.fInterSweepAdaptTimeS);

            }
        }

        [Ignore("group not yet implemented")]
        [Test]
        public void Test_MatchesOfficial_Group12()
        {

            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 12 - Variable parameter user list
                Assert.AreEqual(officialHeader.nULEnable, testHeader.nULEnable);
                Assert.AreEqual(officialHeader.nULParamToVary, testHeader.nULParamToVary);
                Assert.AreEqual(officialHeader.nULRepeat, testHeader.nULRepeat);
                Assert.AreEqual(officialHeader.sULParamValueList, testHeader.sULParamValueList);
            }
        }

        [Ignore("group not yet implemented")]
        [Test]
        public void Test_MatchesOfficial_Group13()
        {

            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 13 - Statistics measurements
                Assert.AreEqual(officialHeader.nStatsEnable, testHeader.nStatsEnable);
                Assert.AreEqual(officialHeader.nStatsActiveChannels, testHeader.nStatsActiveChannels);
                Assert.AreEqual(officialHeader.nStatsSearchRegionFlags, testHeader.nStatsSearchRegionFlags);
                Assert.AreEqual(officialHeader.nStatsSmoothing, testHeader.nStatsSmoothing);
                Assert.AreEqual(officialHeader.nStatsSmoothingEnable, testHeader.nStatsSmoothingEnable);
                Assert.AreEqual(officialHeader.nStatsBaseline, testHeader.nStatsBaseline);
                Assert.AreEqual(officialHeader.nStatsBaselineDAC, testHeader.nStatsBaselineDAC);
                Assert.AreEqual(officialHeader.lStatsBaselineStart, testHeader.lStatsBaselineStart);
                Assert.AreEqual(officialHeader.lStatsBaselineEnd, testHeader.lStatsBaselineEnd);
                Assert.AreEqual(officialHeader.lStatsMeasurements, testHeader.lStatsMeasurements);
                Assert.AreEqual(officialHeader.lStatsStart, testHeader.lStatsStart);
                Assert.AreEqual(officialHeader.lStatsEnd, testHeader.lStatsEnd);
                Assert.AreEqual(officialHeader.nRiseBottomPercentile, testHeader.nRiseBottomPercentile);
                Assert.AreEqual(officialHeader.nRiseTopPercentile, testHeader.nRiseTopPercentile);
                Assert.AreEqual(officialHeader.nDecayBottomPercentile, testHeader.nDecayBottomPercentile);
                Assert.AreEqual(officialHeader.nDecayTopPercentile, testHeader.nDecayTopPercentile);
                Assert.AreEqual(officialHeader.nStatsChannelPolarity, testHeader.nStatsChannelPolarity);
                Assert.AreEqual(officialHeader.nStatsSearchMode, testHeader.nStatsSearchMode);
                Assert.AreEqual(officialHeader.nStatsSearchDAC, testHeader.nStatsSearchDAC);

            }
        }

        [Ignore("group not yet implemented")]
        [Test]
        public void Test_MatchesOfficial_Group14()
        {

            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 14 - Channel Arithmetic
                Assert.AreEqual(officialHeader.nArithmeticEnable, testHeader.nArithmeticEnable);
                Assert.AreEqual(officialHeader.nArithmeticExpression, testHeader.nArithmeticExpression);
                Assert.AreEqual(officialHeader.fArithmeticUpperLimit, testHeader.fArithmeticUpperLimit);
                Assert.AreEqual(officialHeader.fArithmeticLowerLimit, testHeader.fArithmeticLowerLimit);
                Assert.AreEqual(officialHeader.nArithmeticADCNumA, testHeader.nArithmeticADCNumA);
                Assert.AreEqual(officialHeader.nArithmeticADCNumB, testHeader.nArithmeticADCNumB);
                Assert.AreEqual(officialHeader.fArithmeticK1, testHeader.fArithmeticK1);
                Assert.AreEqual(officialHeader.fArithmeticK2, testHeader.fArithmeticK2);
                Assert.AreEqual(officialHeader.fArithmeticK3, testHeader.fArithmeticK3);
                Assert.AreEqual(officialHeader.fArithmeticK4, testHeader.fArithmeticK4);
                Assert.AreEqual(officialHeader.fArithmeticK5, testHeader.fArithmeticK5);
                Assert.AreEqual(officialHeader.fArithmeticK6, testHeader.fArithmeticK6);
                Assert.AreEqual(officialHeader.sArithmeticOperator, testHeader.sArithmeticOperator);
                Assert.AreEqual(officialHeader.sArithmeticUnits, testHeader.sArithmeticUnits);
            }
        }

        [Ignore("group not yet implemented")]
        [Test]
        public void Test_MatchesOfficial_Group15()
        {

            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 15 - Leak subtraction
                Assert.AreEqual(officialHeader.nPNPosition, testHeader.nPNPosition);
                Assert.AreEqual(officialHeader.nPNNumPulses, testHeader.nPNNumPulses);
                Assert.AreEqual(officialHeader.nPNPolarity, testHeader.nPNPolarity);
                Assert.AreEqual(officialHeader.fPNSettlingTime, testHeader.fPNSettlingTime);
                Assert.AreEqual(officialHeader.fPNInterpulse, testHeader.fPNInterpulse);
                Assert.AreEqual(officialHeader.nLeakSubtractType, testHeader.nLeakSubtractType);
                Assert.AreEqual(officialHeader.fPNHoldingLevel, testHeader.fPNHoldingLevel);
                Assert.AreEqual(officialHeader.nLeakSubtractADCIndex, testHeader.nLeakSubtractADCIndex);

            }
        }

        [Ignore("group not yet implemented")]
        [Test]
        public void Test_MatchesOfficial_Group16()
        {

            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 16 - Miscellaneous variables
                Assert.AreEqual(officialHeader.nLevelHysteresis, testHeader.nLevelHysteresis);
                Assert.AreEqual(officialHeader.lTimeHysteresis, testHeader.lTimeHysteresis);
                Assert.AreEqual(officialHeader.nAllowExternalTags, testHeader.nAllowExternalTags);
                Assert.AreEqual(officialHeader.nAverageAlgorithm, testHeader.nAverageAlgorithm);
                Assert.AreEqual(officialHeader.fAverageWeighting, testHeader.fAverageWeighting);
                Assert.AreEqual(officialHeader.nUndoPromptStrategy, testHeader.nUndoPromptStrategy);
                Assert.AreEqual(officialHeader.nTrialTriggerSource, testHeader.nTrialTriggerSource);
                Assert.AreEqual(officialHeader.nStatisticsDisplayStrategy, testHeader.nStatisticsDisplayStrategy);
                Assert.AreEqual(officialHeader.nExternalTagType, testHeader.nExternalTagType);
                Assert.AreEqual(officialHeader.lHeaderSize, testHeader.lHeaderSize);
                Assert.AreEqual(officialHeader.nStatisticsClearStrategy, testHeader.nStatisticsClearStrategy);
                Assert.AreEqual(officialHeader.nEnableFirstLastHolding, testHeader.nEnableFirstLastHolding);
            }
        }

        [Ignore("group not yet implemented")]
        [Test]
        public void Test_MatchesOfficial_Group17()
        {

            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 17 - Trains parameters
                Assert.AreEqual(officialHeader.lEpochPulsePeriod, testHeader.lEpochPulsePeriod);
                Assert.AreEqual(officialHeader.lEpochPulseWidth, testHeader.lEpochPulseWidth);
            }
        }

        [Ignore("group not yet implemented")]
        [Test]
        public void Test_MatchesOfficial_Group18()
        {

            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 18 - Application version data
                Assert.AreEqual(officialHeader.nCreatorMajorVersion, testHeader.nCreatorMajorVersion);
                Assert.AreEqual(officialHeader.nCreatorMinorVersion, testHeader.nCreatorMinorVersion);
                Assert.AreEqual(officialHeader.nCreatorBugfixVersion, testHeader.nCreatorBugfixVersion);
                Assert.AreEqual(officialHeader.nCreatorBuildVersion, testHeader.nCreatorBuildVersion);
                Assert.AreEqual(officialHeader.nModifierMajorVersion, testHeader.nModifierMajorVersion);
                Assert.AreEqual(officialHeader.nModifierMinorVersion, testHeader.nModifierMinorVersion);
                Assert.AreEqual(officialHeader.nModifierBugfixVersion, testHeader.nModifierBugfixVersion);
                Assert.AreEqual(officialHeader.nModifierBuildVersion, testHeader.nModifierBuildVersion);
            }
        }

        [Ignore("group not yet implemented")]
        [Test]
        public void Test_MatchesOfficial_Group19()
        {

            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 19 - LTP protocol
                Assert.AreEqual(officialHeader.nLTPType, testHeader.nLTPType);
                Assert.AreEqual(officialHeader.nLTPUsageOfDAC, testHeader.nLTPUsageOfDAC);
                Assert.AreEqual(officialHeader.nLTPPresynapticPulses, testHeader.nLTPPresynapticPulses);
            }
        }

        [Ignore("group not yet implemented")]
        [Test]
        public void Test_MatchesOfficial_Group20()
        {

            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 20 - Digidata 132x Trigger out flag
                Assert.AreEqual(officialHeader.nScopeTriggerOut, testHeader.nScopeTriggerOut);
            }
        }

        [Ignore("group not yet implemented")]
        [Test]
        public void Test_MatchesOfficial_Group21()
        {

            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 21 - Epoch resistance
                Assert.AreEqual(officialHeader.sEpochResistanceSignalName, testHeader.sEpochResistanceSignalName);
                Assert.AreEqual(officialHeader.nEpochResistanceState, testHeader.nEpochResistanceState);
            }
        }

        [Ignore("group not yet implemented")]
        [Test]
        public void Test_MatchesOfficial_Group22()
        {

            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 22 - Alternating episodic mode
                Assert.AreEqual(officialHeader.nAlternateDACOutputState, testHeader.nAlternateDACOutputState);
                Assert.AreEqual(officialHeader.nAlternateDigitalOutputState, testHeader.nAlternateDigitalOutputState);
                Assert.AreEqual(officialHeader.nAlternateDigitalValue, testHeader.nAlternateDigitalValue);
                Assert.AreEqual(officialHeader.nAlternateDigitalTrainValue, testHeader.nAlternateDigitalTrainValue);
            }
        }

        [Ignore("group not yet implemented")]
        [Test]
        public void Test_MatchesOfficial_Group23()
        {
            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 23 - Post-processing actions
                Assert.AreEqual(officialHeader.fPostProcessLowpassFilter, testHeader.fPostProcessLowpassFilter);
                Assert.AreEqual(officialHeader.nPostProcessLowpassFilterType, testHeader.nPostProcessLowpassFilterType);
            }
        }

        [Ignore("group not yet implemented")]
        [Test]
        public void Test_MatchesOfficial_Group24()
        {
            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 24 - Legacy gear shift info
                Assert.AreEqual(officialHeader.fLegacyADCSequenceInterval, testHeader.fLegacyADCSequenceInterval);
                Assert.AreEqual(officialHeader.fLegacyADCSecondSequenceInterval, testHeader.fLegacyADCSecondSequenceInterval);
                Assert.AreEqual(officialHeader.lLegacyClockChange, testHeader.lLegacyClockChange);
                Assert.AreEqual(officialHeader.lLegacyNumSamplesPerEpisode, testHeader.lLegacyNumSamplesPerEpisode);
            }
        }

        [Ignore("group not yet implemented")]
        [Test]
        public void Test_MatchesOfficial_Group25()
        {
            foreach (var dict in AbfHeaders)
            {
                AbfSharp.ABFFIO.Structs.ABFFileHeader officialHeader = dict.Key;
                AbfSharp.HeaderBase testHeader = dict.Value;
                Console.WriteLine($"{testHeader.AbfID} {testHeader.fFileVersionNumber}");

                //region GROUP 25 - Gap-Free Config
                Assert.AreEqual(officialHeader.nGapFreeEpochType, testHeader.nGapFreeEpochType);
                Assert.AreEqual(officialHeader.fGapFreeEpochLevel, testHeader.fGapFreeEpochLevel);
                Assert.AreEqual(officialHeader.lGapFreeEpochDuration, testHeader.lGapFreeEpochDuration);
                Assert.AreEqual(officialHeader.nGapFreeDigitalValue, testHeader.nGapFreeDigitalValue);
                Assert.AreEqual(officialHeader.nGapFreeEpochStart, testHeader.nGapFreeEpochStart);
            }
        }
    }
}
