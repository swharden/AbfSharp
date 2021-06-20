using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.HeaderData.Abf2
{
    public class DacSection : Section
    {
        const int BLOCKSIZE = 512;
        const int DAC_COUNT = 8;

        public readonly Int16[] nDACNum = new Int16[DAC_COUNT]; // TODO: hide this?
        public readonly Int16[] nTelegraphDACScaleFactorEnable = new Int16[DAC_COUNT];
        public readonly Single[] fInstrumentHoldingLevel = new Single[DAC_COUNT];
        public readonly Single[] fDACScaleFactor = new Single[DAC_COUNT];
        public readonly Single[] fDACHoldingLevel = new Single[DAC_COUNT];
        public readonly Single[] fDACCalibrationFactor = new Single[DAC_COUNT];
        public readonly Single[] fDACCalibrationOffset = new Single[DAC_COUNT];
        public readonly Int32[] lDACChannelNameIndex = new Int32[DAC_COUNT];
        public readonly Int32[] lDACChannelUnitsIndex = new Int32[DAC_COUNT];
        public readonly Int32[] lDACFilePtr = new Int32[DAC_COUNT];
        public readonly Int32[] lDACFileNumEpisodes = new Int32[DAC_COUNT];
        public readonly Int16[] nWaveformEnable = new Int16[DAC_COUNT];
        public readonly Int16[] nWaveformSource = new Int16[DAC_COUNT];
        public readonly Int16[] nInterEpisodeLevel = new Int16[DAC_COUNT];
        public readonly Single[] fDACFileScale = new Single[DAC_COUNT];
        public readonly Single[] fDACFileOffset = new Single[DAC_COUNT];
        public readonly Int32[] lDACFileEpisodeNum = new Int32[DAC_COUNT];
        public readonly Int16[] nDACFileADCNum = new Int16[DAC_COUNT];
        public readonly Int16[] nConditEnable = new Int16[DAC_COUNT];
        public readonly Int32[] lConditNumPulses = new Int32[DAC_COUNT];
        public readonly Single[] fBaselineDuration = new Single[DAC_COUNT];
        public readonly Single[] fBaselineLevel = new Single[DAC_COUNT];
        public readonly Single[] fStepDuration = new Single[DAC_COUNT];
        public readonly Single[] fStepLevel = new Single[DAC_COUNT];
        public readonly Single[] fPostTrainPeriod = new Single[DAC_COUNT];
        public readonly Single[] fPostTrainLevel = new Single[DAC_COUNT];
        public readonly Int16[] nMembTestEnable = new Int16[DAC_COUNT];
        public readonly Int16[] nLeakSubtractType = new Int16[DAC_COUNT];
        public readonly Int16[] nPNPolarity = new Int16[DAC_COUNT];
        public readonly Single[] fPNHoldingLevel = new Single[DAC_COUNT];
        public readonly Int16[] nPNNumADCChannels = new Int16[DAC_COUNT];
        public readonly Int16[] nPNPosition = new Int16[DAC_COUNT];
        public readonly Int16[] nPNNumPulses = new Int16[DAC_COUNT];
        public readonly Single[] fPNSettlingTime = new Single[DAC_COUNT];
        public readonly Single[] fPNInterpulse = new Single[DAC_COUNT];
        public readonly Int16[] nLTPUsageOfDAC = new Int16[DAC_COUNT];
        public readonly Int16[] nLTPPresynapticPulses = new Int16[DAC_COUNT];
        public readonly Int32[] lDACFilePathIndex = new Int32[DAC_COUNT];
        public readonly Single[] fMembTestPreSettlingTimeMS = new Single[DAC_COUNT];
        public readonly Single[] fMembTestPostSettlingTimeMS = new Single[DAC_COUNT];
        public readonly Int16[] nLeakSubtractADCIndex = new Int16[DAC_COUNT];

        public DacSection(BinaryReader reader) : base(reader, 108)
        {
            for (int i = 0; i < SectionCount; i++)
            {
                reader.BaseStream.Seek(SectionStart + i * SectionSize, SeekOrigin.Begin);

                nDACNum[i] = reader.ReadInt16();
                int dacIndex = nDACNum[i];

                nTelegraphDACScaleFactorEnable[dacIndex] = reader.ReadInt16();  // 2
                fInstrumentHoldingLevel[dacIndex] = reader.ReadSingle();  // 4
                fDACScaleFactor[dacIndex] = reader.ReadSingle();  // 8
                fDACHoldingLevel[dacIndex] = reader.ReadSingle();  // 12
                fDACCalibrationFactor[dacIndex] = reader.ReadSingle();  // 16
                fDACCalibrationOffset[dacIndex] = reader.ReadSingle();  // 20
                lDACChannelNameIndex[dacIndex] = reader.ReadInt32();  // 24
                lDACChannelUnitsIndex[dacIndex] = reader.ReadInt32();  // 28
                lDACFilePtr[dacIndex] = reader.ReadInt32();  // 32
                lDACFileNumEpisodes[dacIndex] = reader.ReadInt32();  // 36
                nWaveformEnable[dacIndex] = reader.ReadInt16();  // 40
                nWaveformSource[dacIndex] = reader.ReadInt16();  // 42
                nInterEpisodeLevel[dacIndex] = reader.ReadInt16();  // 44
                fDACFileScale[dacIndex] = reader.ReadSingle();  // 46
                fDACFileOffset[dacIndex] = reader.ReadSingle();  // 50
                lDACFileEpisodeNum[dacIndex] = reader.ReadInt32();  // 54
                nDACFileADCNum[dacIndex] = reader.ReadInt16();  // 58
                nConditEnable[dacIndex] = reader.ReadInt16();  // 60
                lConditNumPulses[dacIndex] = reader.ReadInt32();  // 62
                fBaselineDuration[dacIndex] = reader.ReadSingle();  // 66
                fBaselineLevel[dacIndex] = reader.ReadSingle();  // 70
                fStepDuration[dacIndex] = reader.ReadSingle();  // 74
                fStepLevel[dacIndex] = reader.ReadSingle();  // 78
                fPostTrainPeriod[dacIndex] = reader.ReadSingle();  // 82
                fPostTrainLevel[dacIndex] = reader.ReadSingle();  // 86
                nMembTestEnable[dacIndex] = reader.ReadInt16();  // 90
                nLeakSubtractType[dacIndex] = reader.ReadInt16();  // 92
                nPNPolarity[dacIndex] = reader.ReadInt16();  // 94
                fPNHoldingLevel[dacIndex] = reader.ReadSingle();  // 96
                nPNNumADCChannels[dacIndex] = reader.ReadInt16();  // 100
                nPNPosition[dacIndex] = reader.ReadInt16();  // 102
                nPNNumPulses[dacIndex] = reader.ReadInt16();  // 104
                fPNSettlingTime[dacIndex] = reader.ReadSingle();  // 106
                fPNInterpulse[dacIndex] = reader.ReadSingle();  // 110
                nLTPUsageOfDAC[dacIndex] = reader.ReadInt16();  // 114
                nLTPPresynapticPulses[dacIndex] = reader.ReadInt16();  // 116
                lDACFilePathIndex[dacIndex] = reader.ReadInt32();  // 118
                fMembTestPreSettlingTimeMS[dacIndex] = reader.ReadSingle();  // 122
                fMembTestPostSettlingTimeMS[dacIndex] = reader.ReadSingle();  // 126
                nLeakSubtractADCIndex[dacIndex] = reader.ReadInt16();  // 130
            }
        }
    }
}
