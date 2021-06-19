using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.HeaderData.Abf2
{
    public class ProtocolSection
    {
        private const int BLOCKSIZE = 512;

        public int nOperationMode { get; private set; }

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
            var fADCSequenceInterval = reader.ReadSingle();  // 2
            var bEnableFileCompression = reader.ReadByte();  // 6
            var sUnused = reader.ReadBytes(3); // 7
            var uFileCompressionRatio = reader.ReadUInt32();  // 10
            var fSynchTimeUnit = reader.ReadSingle();  // 14
            var fSecondsPerRun = reader.ReadSingle();  // 18
            var lNumSamplesPerEpisode = reader.ReadUInt32();  // 22
            var lPreTriggerSamples = reader.ReadUInt32();  // 26
            var lEpisodesPerRun = reader.ReadUInt32();  // 30
            var lRunsPerTrial = reader.ReadUInt32();  // 34
            var lNumberOfTrials = reader.ReadUInt32();  // 38
            var nAveragingMode = reader.ReadInt16();  // 42
            var nUndoRunCount = reader.ReadInt16();  // 44
            var nFirstEpisodeInRun = reader.ReadInt16();  // 46
            var fTriggerThreshold = reader.ReadSingle();  // 48
            var nTriggerSource = reader.ReadInt16();  // 52
            var nTriggerAction = reader.ReadInt16();  // 54
            var nTriggerPolarity = reader.ReadInt16();  // 56
            var fScopeOutputInterval = reader.ReadSingle();  // 58
            var fEpisodeStartToStart = reader.ReadSingle();  // 62
            var fRunStartToStart = reader.ReadSingle();  // 66
            var lAverageCount = reader.ReadUInt32();  // 70
            var fTrialStartToStart = reader.ReadSingle();  // 74
            var nAutoTriggerStrategy = reader.ReadInt16();  // 78
            var fFirstRunDelayS = reader.ReadSingle();  // 80
            var nChannelStatsStrategy = reader.ReadInt16();  // 84
            var lSamplesPerTrace = reader.ReadUInt32();  // 86
            var lStartDisplayNum = reader.ReadUInt32();  // 90
            var lFinishDisplayNum = reader.ReadUInt32();  // 94
            var nShowPNRawData = reader.ReadInt16();  // 98
            var fStatisticsPeriod = reader.ReadSingle();  // 100
            var lStatisticsMeasurements = reader.ReadUInt32();  // 104
            var nStatisticsSaveStrategy = reader.ReadInt16();  // 108
            var fADCRange = reader.ReadSingle();  // 110
            var fDACRange = reader.ReadSingle();  // 114
            var lADCResolution = reader.ReadUInt32();  // 118
            var lDACResolution = reader.ReadUInt32();  // 122
            var nExperimentType = reader.ReadInt16();  // 126
            var nManualInfoStrategy = reader.ReadInt16();  // 128
            var nCommentsEnable = reader.ReadInt16();  // 130
            var lFileCommentIndex = reader.ReadUInt32();  // 132
            var nAutoAnalyseEnable = reader.ReadInt16();  // 136
            var nSignalType = reader.ReadInt16();  // 138
            var nDigitalEnable = reader.ReadInt16();  // 140
            var nActiveDACChannel = reader.ReadInt16();  // 142
            var nDigitalHolding = reader.ReadInt16();  // 144
            var nDigitalInterEpisode = reader.ReadInt16();  // 146
            var nDigitalDACChannel = reader.ReadInt16();  // 148
            var nDigitalTrainActiveLogic = reader.ReadInt16();  // 150
            var nStatsEnable = reader.ReadInt16();  // 152
            var nStatisticsClearStrategy = reader.ReadInt16();  // 154
            var nLevelHysteresis = reader.ReadInt16();  // 156
            var lTimeHysteresis = reader.ReadUInt32();  // 158
            var nAllowExternalTags = reader.ReadInt16();  // 162
            var nAverageAlgorithm = reader.ReadInt16();  // 164
            var fAverageWeighting = reader.ReadSingle();  // 166
            var nUndoPromptStrategy = reader.ReadInt16();  // 170
            var nTrialTriggerSource = reader.ReadInt16();  // 172
            var nStatisticsDisplayStrategy = reader.ReadInt16();  // 174
            var nExternalTagType = reader.ReadInt16();  // 176
            var nScopeTriggerOut = reader.ReadInt16();  // 178
            var nLTPType = reader.ReadInt16();  // 180
            var nAlternateDACOutputState = reader.ReadInt16();  // 182
            var nAlternateDigitalOutputState = reader.ReadInt16();  // 184
            float[] fCellID = new float[] { reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() }; // 186
            var nDigitizerADCs = reader.ReadInt16();  // 198
            var nDigitizerDACs = reader.ReadInt16();  // 200
            var nDigitizerTotalDigitalOuts = reader.ReadInt16();  // 202
            var nDigitizerSynchDigitalOuts = reader.ReadInt16();  // 204
            var nDigitizerType = reader.ReadInt16();  // 206
        }
    }
}
