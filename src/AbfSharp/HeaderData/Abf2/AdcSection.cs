using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AbfSharp.HeaderData.Abf2
{
    public class AdcSection : Section
    {
        // identifier (index)
        public readonly Int16[] nADCNum;

        // telegraph settings
        public readonly Int16[] nTelegraphEnable;
        public readonly Int16[] nTelegraphInstrument;
        public readonly Single[] fTelegraphAdditGain;
        public readonly Single[] fTelegraphFilter;
        public readonly Single[] fTelegraphMembraneCap;
        public readonly Int16[] nTelegraphMode;
        public readonly Single[] fTelegraphAccessResistance;

        // mapping
        public readonly Int16[] nADCPtoLChannelMap;
        public readonly Int16[] nADCSamplingSeq;

        // scaling
        public readonly Single[] fADCProgrammableGain;
        public readonly Single[] fADCDisplayAmplification;
        public readonly Single[] fADCDisplayOffset;
        public readonly Single[] fInstrumentScaleFactor;
        public readonly Single[] fInstrumentOffset;
        public readonly Single[] fSignalGain;
        public readonly Single[] fSignalOffset;

        // filtering
        public readonly Single[] fSignalLowpassFilter;
        public readonly Single[] fSignalHighpassFilter;
        public readonly Byte[] nLowpassFilterType;
        public readonly Byte[] nHighpassFilterType;
        public readonly Single[] fPostProcessLowpassFilter;
        public readonly Byte[] nPostProcessLowpassFilterType;

        // options
        public readonly Byte[] bEnabledDuringPN;
        public readonly Int16[] nStatsChannelPolarity;

        // string indexes
        public readonly Int32[] lADCChannelNameIndex;
        public readonly Int32[] lADCUnitsIndex;

        public AdcSection(BinaryReader reader) : base(reader, 92)
        {
            const int ADC_COUNT = 16;
            nADCNum = new Int16[ADC_COUNT];
            nTelegraphEnable = new Int16[ADC_COUNT];
            nTelegraphInstrument = new Int16[ADC_COUNT];
            fTelegraphAdditGain = new Single[ADC_COUNT];
            fTelegraphFilter = new Single[ADC_COUNT];
            fTelegraphMembraneCap = new Single[ADC_COUNT];
            nTelegraphMode = new Int16[ADC_COUNT];
            fTelegraphAccessResistance = new Single[ADC_COUNT];
            nADCPtoLChannelMap = new Int16[ADC_COUNT];
            nADCSamplingSeq = new Int16[ADC_COUNT];
            fADCProgrammableGain = new Single[ADC_COUNT];
            fADCDisplayAmplification = new Single[ADC_COUNT];
            fADCDisplayOffset = new Single[ADC_COUNT];
            fInstrumentScaleFactor = new Single[ADC_COUNT];
            fInstrumentOffset = new Single[ADC_COUNT];
            fSignalGain = new Single[ADC_COUNT];
            fSignalOffset = new Single[ADC_COUNT];
            fSignalLowpassFilter = new Single[ADC_COUNT];
            fSignalHighpassFilter = new Single[ADC_COUNT];
            nLowpassFilterType = new Byte[ADC_COUNT];
            nHighpassFilterType = new Byte[ADC_COUNT];
            fPostProcessLowpassFilter = new Single[ADC_COUNT];
            nPostProcessLowpassFilterType = new Byte[ADC_COUNT];
            bEnabledDuringPN = new Byte[ADC_COUNT];
            nStatsChannelPolarity = new Int16[ADC_COUNT];
            lADCChannelNameIndex = new Int32[ADC_COUNT];
            lADCUnitsIndex = new Int32[ADC_COUNT];

            for (int i = 0; i < SectionCount; i++)
            {
                reader.BaseStream.Seek(SectionStart + i * SectionSize, SeekOrigin.Begin);
                nADCNum[i] = reader.ReadInt16();

                int adcNum = nADCNum[i];
                nTelegraphEnable[adcNum] = reader.ReadInt16();
                nTelegraphInstrument[adcNum] = reader.ReadInt16();
                fTelegraphAdditGain[adcNum] = reader.ReadSingle();
                fTelegraphFilter[adcNum] = reader.ReadSingle();
                fTelegraphMembraneCap[adcNum] = reader.ReadSingle();
                nTelegraphMode[adcNum] = reader.ReadInt16();
                fTelegraphAccessResistance[adcNum] = reader.ReadSingle();
                nADCPtoLChannelMap[adcNum] = reader.ReadInt16();
                nADCSamplingSeq[adcNum] = reader.ReadInt16();
                fADCProgrammableGain[adcNum] = reader.ReadSingle();
                fADCDisplayAmplification[adcNum] = reader.ReadSingle();
                fADCDisplayOffset[adcNum] = reader.ReadSingle();
                fInstrumentScaleFactor[adcNum] = reader.ReadSingle();
                fInstrumentOffset[adcNum] = reader.ReadSingle();
                fSignalGain[adcNum] = reader.ReadSingle();
                fSignalOffset[adcNum] = reader.ReadSingle();
                fSignalLowpassFilter[adcNum] = reader.ReadSingle();
                fSignalHighpassFilter[adcNum] = reader.ReadSingle();
                nLowpassFilterType[adcNum] = reader.ReadByte();
                nHighpassFilterType[adcNum] = reader.ReadByte();
                fPostProcessLowpassFilter[adcNum] = reader.ReadSingle();
                nPostProcessLowpassFilterType[adcNum] = reader.ReadByte();
                bEnabledDuringPN[adcNum] = reader.ReadByte();
                nStatsChannelPolarity[adcNum] = reader.ReadInt16();
                lADCChannelNameIndex[adcNum] = reader.ReadInt32();
                lADCUnitsIndex[adcNum] = reader.ReadInt32();

                nADCSamplingSeq[i] = (Int16)adcNum;
            }
        }
    }
}
