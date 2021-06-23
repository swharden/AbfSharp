﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AbfSharp.Sections;

namespace AbfSharp
{
    public class HeaderAbf2 : Header
    {
        private HeaderSection HeaderSection;
        private ProtocolSection ProtocolSection;
        private AdcSection AdcSection;
        private DacSection DacSection;
        private StringsSection StringsSection;
        private TagSection TagSection;
        private DataSection DataSection;
        private SynchSection SynchSection;
        private EpochSection EpochSection;
        private EpochPerDacSection EpochPerDacSection;

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
            AdcSection = new(reader);
            DacSection = new(reader);
            StringsSection = new(reader);
            TagSection = new(reader);
            DataSection = new(reader);
            SynchSection = new(reader);
            EpochSection = new(reader);
            EpochPerDacSection = new(reader);

            ReadGroup1();
            ReadGroup2();
            ReadGroup3();
            ReadGroup5();
            ReadGroup6();
            ReadGroup7();
            ReadGroup9();
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
            nExperimentType = ProtocolSection.nExperimentType;
            nManualInfoStrategy = ProtocolSection.nManualInfoStrategy;

            fCellID1 = ProtocolSection.fCellID1;
            fCellID2 = ProtocolSection.fCellID2;
            fCellID3 = ProtocolSection.fCellID3;

            sProtocolPath = StringsSection.Strings[HeaderSection.uProtocolPathIndex];
            sCreatorInfo = StringsSection.Strings[HeaderSection.uCreatorNameIndex];
            sModifierInfo = StringsSection.Strings[HeaderSection.uModifierNameIndex];
            nCommentsEnable = ProtocolSection.nCommentsEnable;
            sFileComment = StringsSection.Strings[ProtocolSection.lFileCommentIndex];

            nTelegraphEnable = AdcSection.nTelegraphEnable;
            nTelegraphInstrument = AdcSection.nTelegraphInstrument;
            fTelegraphAdditGain = AdcSection.fTelegraphAdditGain;
            fTelegraphFilter = AdcSection.fTelegraphFilter;
            fTelegraphMembraneCap = AdcSection.fTelegraphMembraneCap;
            fTelegraphAccessResistance = AdcSection.fTelegraphAccessResistance;
            nTelegraphMode = AdcSection.nTelegraphMode;
            nTelegraphDACScaleFactorEnable = DacSection.nTelegraphDACScaleFactorEnable;

            FileGUID = MakeGuid(HeaderSection.FileGUID);
            fInstrumentHoldingLevel = DacSection.fInstrumentHoldingLevel;
            nCRCEnable = (short)HeaderSection.nCRCEnable;
        }

        private void ReadGroup7()
        {
            nSignalType = ProtocolSection.nSignalType;

            nADCPtoLChannelMap = AdcSection.nADCPtoLChannelMap;
            nADCSamplingSeq = AdcSection.nADCSamplingSeq;
            fADCProgrammableGain = AdcSection.fADCProgrammableGain;
            fADCDisplayAmplification = AdcSection.fADCDisplayAmplification;
            fADCDisplayOffset = AdcSection.fADCDisplayOffset;
            fInstrumentScaleFactor = AdcSection.fInstrumentScaleFactor;
            fInstrumentOffset = AdcSection.fInstrumentOffset;
            fSignalGain = AdcSection.fSignalGain;
            fSignalOffset = AdcSection.fSignalOffset;
            fSignalLowpassFilter = AdcSection.fSignalLowpassFilter;
            fSignalHighpassFilter = AdcSection.fSignalHighpassFilter;
            nLowpassFilterType = AdcSection.nLowpassFilterType;
            nHighpassFilterType = AdcSection.nHighpassFilterType;

            sADCChannelName = AdcSection.lADCChannelNameIndex.Select(x => StringsSection.Strings[x]).ToArray();
            sADCUnits = AdcSection.lADCUnitsIndex.Select(x => StringsSection.Strings[x]).ToArray();

            fDACScaleFactor = DacSection.fDACScaleFactor;
            fDACHoldingLevel = DacSection.fDACHoldingLevel;
            fDACCalibrationFactor = DacSection.fDACCalibrationFactor;
            fDACCalibrationOffset = DacSection.fDACCalibrationOffset;
            sDACChannelName = DacSection.lDACChannelNameIndex.Select(x => StringsSection.Strings[x]).ToArray();
            sDACChannelUnits = DacSection.lDACChannelUnitsIndex.Select(x => StringsSection.Strings[x]).ToArray();
        }

        private void ReadGroup9()
        {
            nDigitalEnable = ProtocolSection.nDigitalEnable;
            nActiveDACChannel = ProtocolSection.nActiveDACChannel;
            nDigitalDACChannel = ProtocolSection.nDigitalDACChannel;
            nDigitalHolding = ProtocolSection.nDigitalHolding;
            nDigitalInterEpisode = ProtocolSection.nDigitalInterEpisode;
            nDigitalTrainActiveLogic = ProtocolSection.nDigitalTrainActiveLogic;

            // TODO: support epoch section
            nDigitalValue = EpochSection.nDigitalValue;
            nDigitalTrainValue = EpochSection.nDigitalTrainValue;

            nEpochType = EpochPerDacSection.nEpochType;
            fEpochInitLevel = EpochPerDacSection.fEpochInitLevel;
            fEpochLevelInc = EpochPerDacSection.fEpochLevelInc;
            lEpochInitDuration = EpochPerDacSection.lEpochInitDuration;
            lEpochDurationInc = EpochPerDacSection.lEpochDurationInc;

            bEpochCompression = new byte[50];
            bEpochCompression[0] = ProtocolSection.bEnableFileCompression;

            nWaveformEnable = DacSection.nWaveformEnable;
            nWaveformSource = DacSection.nWaveformSource;
            nInterEpisodeLevel = DacSection.nInterEpisodeLevel;
            
        }
    }
}