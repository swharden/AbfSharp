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

        [Test]
        public void Test_MatchesOfficial_Group2()
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
        public void Test_MatchesOfficial_Group3()
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
        public void Test_MatchesOfficial_Group5()
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
    }
}
