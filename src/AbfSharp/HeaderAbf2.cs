using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AbfSharp.HeaderData.Abf2;

namespace AbfSharp
{
    public class HeaderAbf2 : HeaderBase
    {
        private HeaderSection HeaderSection;
        private ProtocolSection ProtocolSection;
        private AdcSection AdcSection;
        private DacSection DacSection;
        private StringsSection StringsSection;
        private TagSection TagSection;
        private DataSection DataSection;
        private SynchSection SynchSection;

        public HeaderAbf2(BinaryReader reader, string filePath)
        {
            Path = System.IO.Path.GetFullPath(filePath);
            Read(reader);
        }

        public HeaderAbf2(string filePath)
        {
            Path = System.IO.Path.GetFullPath(filePath);
            using Stream fs = File.Open(filePath, FileMode.Open);
            using BinaryReader reader = new(fs);
            Read(reader);
        }

        private void Read(BinaryReader reader)
        {
            HeaderSection = new(reader);
            ProtocolSection = new(reader);
            AdcSection = new AdcSection(reader);
            DacSection = new DacSection(reader);
            StringsSection = new StringsSection(reader);
            TagSection = new TagSection(reader);
            DataSection = new DataSection(reader);
            SynchSection = new SynchSection(reader);

            ReadGroup1();
            ReadGroup2();
            ReadGroup3();
            ReadGroup5();
            ReadGroup6();
        }

        private void ReadGroup1()
        {
            SignatureBytes = HeaderSection.SignatureBytes;
            fFileVersionNumber = HeaderSection.fFileVersionNumber;
            nOperationMode = ProtocolSection.nOperationMode;
            lActualAcqLength = (int)DataSection.SectionCount;
            lActualEpisodes = (int)HeaderSection.lActualEpisodes;
            uFileStartDate = HeaderSection.uFileStartDate;
            uFileStartTimeMS = HeaderSection.uFileStartTimeMS;
            lStopwatchTime = (int)HeaderSection.uStopwatchTime;
            nFileType = (short)HeaderSection.nFileType;
        }

        private void ReadGroup2()
        {
            lDataSectionPtr = (int)DataSection.SectionBlock;
            lTagSectionPtr = (int)TagSection.SectionBlock;
            lNumTagEntries = (int)TagSection.SectionCount;
            lSynchArrayPtr = (int)SynchSection.SectionBlock;
            lSynchArraySize = (int)SynchSection.SectionCount;
            nDataFormat = (short)HeaderSection.nDataFormat;
            nSimultaneousScan = (short)HeaderSection.nSimultaneousScan;
            lDACFilePtr = DacSection.lDACFilePtr;
            lDACFileNumEpisodes = DacSection.lDACFileNumEpisodes;

            // TODO: scope section
            // TODO: delta array section
            // TODO: voice tag section
            // TODO: statistics section
            // TODO: annotations section
        }

        private void ReadGroup3()
        {
            nADCNumChannels = (short)AdcSection.SectionCount;
            fADCSequenceInterval = ProtocolSection.fADCSequenceInterval;
            uFileCompressionRatio = ProtocolSection.uFileCompressionRatio;
            bEnableFileCompression = ProtocolSection.bEnableFileCompression;
            fSynchTimeUnit = ProtocolSection.fSynchTimeUnit;
            fSecondsPerRun = ProtocolSection.fSecondsPerRun;
            lNumSamplesPerEpisode = (int)ProtocolSection.lNumSamplesPerEpisode;
            lPreTriggerSamples = (int)ProtocolSection.lPreTriggerSamples;
            lEpisodesPerRun = (int)ProtocolSection.lEpisodesPerRun;
            lRunsPerTrial = (int)ProtocolSection.lRunsPerTrial;
            lNumberOfTrials = (int)ProtocolSection.lNumberOfTrials;
            nAveragingMode = ProtocolSection.nAveragingMode;
            nUndoRunCount = ProtocolSection.nUndoRunCount;
            nFirstEpisodeInRun = ProtocolSection.nFirstEpisodeInRun;
            fTriggerThreshold = ProtocolSection.fTriggerThreshold;
            nTriggerSource = ProtocolSection.nTriggerSource;
            nTriggerAction = ProtocolSection.nTriggerAction;
            nTriggerPolarity = ProtocolSection.nTriggerPolarity;
            fScopeOutputInterval = ProtocolSection.fScopeOutputInterval;
            fEpisodeStartToStart = ProtocolSection.fEpisodeStartToStart;
            fRunStartToStart = ProtocolSection.fRunStartToStart;
            fTrialStartToStart = ProtocolSection.fTrialStartToStart;
            lAverageCount = (int)ProtocolSection.lAverageCount;
            nAutoTriggerStrategy = ProtocolSection.nAutoTriggerStrategy;
            fFirstRunDelayS = ProtocolSection.fFirstRunDelayS;
        }

        private void ReadGroup5()
        {
            fADCRange = ProtocolSection.fADCRange;
            fDACRange = ProtocolSection.fDACRange;
            lADCResolution = (int)ProtocolSection.lADCResolution;
            lDACResolution = (int)ProtocolSection.lDACResolution;
        }

        private void ReadGroup6()
        {

        }
    }
}