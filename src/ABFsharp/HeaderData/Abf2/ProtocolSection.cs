using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.HeaderData.Abf2
{
    public class ProtocolSection
    {
        private const int BLOCKSIZE = 512;

        public readonly Int16 nOperationMode;
        public readonly Single fADCSequenceInterval;
        public readonly byte bEnableFileCompression;
        public readonly UInt32 uFileCompressionRatio;
        public readonly Single fSynchTimeUnit;
        public readonly Single fSecondsPerRun;
        public readonly UInt32 lNumSamplesPerEpisode;
        public readonly UInt32 lPreTriggerSamples;
        public readonly UInt32 lEpisodesPerRun;
        public readonly UInt32 lRunsPerTrial;
        public readonly UInt32 lNumberOfTrials;
        public readonly Int16 nAveragingMode;
        public readonly Int16 nUndoRunCount;
        public readonly Int16 nFirstEpisodeInRun;
        public readonly Single fTriggerThreshold;
        public readonly Int16 nTriggerSource;
        public readonly Int16 nTriggerAction;
        public readonly Int16 nTriggerPolarity;
        public readonly Single fScopeOutputInterval;
        public readonly Single fEpisodeStartToStart;
        public readonly Single fRunStartToStart;
        public readonly UInt32 lAverageCount;
        public readonly Single fTrialStartToStart;
        public readonly Int16 nAutoTriggerStrategy;
        public readonly Single fFirstRunDelayS;
        public readonly Int16 nChannelStatsStrategy;
        public readonly UInt32 lSamplesPerTrace;
        public readonly UInt32 lStartDisplayNum;
        public readonly UInt32 lFinishDisplayNum;
        public readonly Int16 nShowPNRawData;
        public readonly Single fStatisticsPeriod;
        public readonly UInt32 lStatisticsMeasurements;
        public readonly Int16 nStatisticsSaveStrategy;
        public readonly Single fADCRange;
        public readonly Single fDACRange;
        public readonly UInt32 lADCResolution;
        public readonly UInt32 lDACResolution;
        public readonly Int16 nExperimentType;
        public readonly Int16 nManualInfoStrategy;
        public readonly Int16 nCommentsEnable;
        public readonly UInt32 lFileCommentIndex;
        public readonly Int16 nAutoAnalyseEnable;
        public readonly Int16 nSignalType;
        public readonly Int16 nDigitalEnable;
        public readonly Int16 nActiveDACChannel;
        public readonly Int16 nDigitalHolding;
        public readonly Int16 nDigitalInterEpisode;
        public readonly Int16 nDigitalDACChannel;
        public readonly Int16 nDigitalTrainActiveLogic;
        public readonly Int16 nStatsEnable;
        public readonly Int16 nStatisticsClearStrategy;
        public readonly Int16 nLevelHysteresis;
        public readonly UInt32 lTimeHysteresis;
        public readonly Int16 nAllowExternalTags;
        public readonly Int16 nAverageAlgorithm;
        public readonly Single fAverageWeighting;
        public readonly Int16 nUndoPromptStrategy;
        public readonly Int16 nTrialTriggerSource;
        public readonly Int16 nStatisticsDisplayStrategy;
        public readonly Int16 nExternalTagType;
        public readonly Int16 nScopeTriggerOut;
        public readonly Int16 nLTPType;
        public readonly Int16 nAlternateDACOutputState;
        public readonly Int16 nAlternateDigitalOutputState;
        public readonly Single[] fCellID;
        public readonly Int16 nDigitizerADCs;
        public readonly Int16 nDigitizerDACs;
        public readonly Int16 nDigitizerTotalDigitalOuts;
        public readonly Int16 nDigitizerSynchDigitalOuts;
        public readonly Int16 nDigitizerType;

        public ProtocolSection(BinaryReader reader)
        {
            // "h" = 2-byte int
            // "H" = 2-byte uint
            // "i" = 4-byte int
            // "I" = 4-byte uint
            // "l" = 4-byte int
            // "L" = 4-byte uint
            // "f" = 4-byte float

            reader.BaseStream.Seek(76, SeekOrigin.Begin);
            long firstByte = reader.ReadUInt32() * BLOCKSIZE;
            long size = reader.ReadUInt32();
            long count = reader.ReadUInt32();

            reader.BaseStream.Seek(firstByte, SeekOrigin.Begin);
            nOperationMode = reader.ReadInt16();  // 0
            fADCSequenceInterval = reader.ReadSingle();  // 2
            bEnableFileCompression = reader.ReadByte();  // 6
            reader.ReadBytes(3); // 7
            uFileCompressionRatio = reader.ReadUInt32();  // 10
            fSynchTimeUnit = reader.ReadSingle();  // 14
            fSecondsPerRun = reader.ReadSingle();  // 18
            lNumSamplesPerEpisode = reader.ReadUInt32();  // 22
            lPreTriggerSamples = reader.ReadUInt32();  // 26
            lEpisodesPerRun = reader.ReadUInt32();  // 30
            lRunsPerTrial = reader.ReadUInt32();  // 34
            lNumberOfTrials = reader.ReadUInt32();  // 38
            nAveragingMode = reader.ReadInt16();  // 42
            nUndoRunCount = reader.ReadInt16();  // 44
            nFirstEpisodeInRun = reader.ReadInt16();  // 46
            fTriggerThreshold = reader.ReadSingle();  // 48
            nTriggerSource = reader.ReadInt16();  // 52
            nTriggerAction = reader.ReadInt16();  // 54
            nTriggerPolarity = reader.ReadInt16();  // 56
            fScopeOutputInterval = reader.ReadSingle();  // 58
            fEpisodeStartToStart = reader.ReadSingle();  // 62
            fRunStartToStart = reader.ReadSingle();  // 66
            lAverageCount = reader.ReadUInt32();  // 70
            fTrialStartToStart = reader.ReadSingle();  // 74
            nAutoTriggerStrategy = reader.ReadInt16();  // 78
            fFirstRunDelayS = reader.ReadSingle();  // 80
            nChannelStatsStrategy = reader.ReadInt16();  // 84
            lSamplesPerTrace = reader.ReadUInt32();  // 86
            lStartDisplayNum = reader.ReadUInt32();  // 90
            lFinishDisplayNum = reader.ReadUInt32();  // 94
            nShowPNRawData = reader.ReadInt16();  // 98
            fStatisticsPeriod = reader.ReadSingle();  // 100
            lStatisticsMeasurements = reader.ReadUInt32();  // 104
            nStatisticsSaveStrategy = reader.ReadInt16();  // 108
            fADCRange = reader.ReadSingle();  // 110
            fDACRange = reader.ReadSingle();  // 114
            lADCResolution = reader.ReadUInt32();  // 118
            lDACResolution = reader.ReadUInt32();  // 122
            nExperimentType = reader.ReadInt16();  // 126
            nManualInfoStrategy = reader.ReadInt16();  // 128
            nCommentsEnable = reader.ReadInt16();  // 130
            lFileCommentIndex = reader.ReadUInt32();  // 132
            nAutoAnalyseEnable = reader.ReadInt16();  // 136
            nSignalType = reader.ReadInt16();  // 138
            nDigitalEnable = reader.ReadInt16();  // 140
            nActiveDACChannel = reader.ReadInt16();  // 142
            nDigitalHolding = reader.ReadInt16();  // 144
            nDigitalInterEpisode = reader.ReadInt16();  // 146
            nDigitalDACChannel = reader.ReadInt16();  // 148
            nDigitalTrainActiveLogic = reader.ReadInt16();  // 150
            nStatsEnable = reader.ReadInt16();  // 152
            nStatisticsClearStrategy = reader.ReadInt16();  // 154
            nLevelHysteresis = reader.ReadInt16();  // 156
            lTimeHysteresis = reader.ReadUInt32();  // 158
            nAllowExternalTags = reader.ReadInt16();  // 162
            nAverageAlgorithm = reader.ReadInt16();  // 164
            fAverageWeighting = reader.ReadSingle();  // 166
            nUndoPromptStrategy = reader.ReadInt16();  // 170
            nTrialTriggerSource = reader.ReadInt16();  // 172
            nStatisticsDisplayStrategy = reader.ReadInt16();  // 174
            nExternalTagType = reader.ReadInt16();  // 176
            nScopeTriggerOut = reader.ReadInt16();  // 178
            nLTPType = reader.ReadInt16();  // 180
            nAlternateDACOutputState = reader.ReadInt16();  // 182
            nAlternateDigitalOutputState = reader.ReadInt16();  // 184
            Single[] fCellID = new Single[] { reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() }; // 186
            nDigitizerADCs = reader.ReadInt16();  // 198
            nDigitizerDACs = reader.ReadInt16();  // 200
            nDigitizerTotalDigitalOuts = reader.ReadInt16();  // 202
            nDigitizerSynchDigitalOuts = reader.ReadInt16();  // 204
            nDigitizerType = reader.ReadInt16();  // 206
        }
    }
}
