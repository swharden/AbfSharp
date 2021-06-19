using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#pragma warning disable CS0618 // Type or member is obsolete

namespace AbfSharp.HeaderData.Abf2
{
    public class AdcSection
    {
        const int BLOCKSIZE = 512;

        /// <summary>
        /// Number of ADC channels
        /// </summary>
        public readonly uint Count;

        /// <summary>
        /// Position in the file of the dat
        /// </summary>
        public readonly uint FirstByte;

        /// <summary>
        /// Length (bytes) in memory for each channel's settings
        /// </summary>
        public readonly uint Size;

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

        [Obsolete("this may not always be calculated perfectly...")]
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
        public readonly Char[] nLowpassFilterType;
        public readonly Char[] nHighpassFilterType;
        public readonly Single[] fPostProcessLowpassFilter;
        public readonly Char[] nPostProcessLowpassFilterType;

        // options
        public readonly Byte[] bEnabledDuringPN;
        public readonly Int16[] nStatsChannelPolarity;

        // string indexes
        public readonly Int32[] lADCChannelNameIndex;
        public readonly Int32[] lADCUnitsIndex;

        public AdcSection(BinaryReader reader)
        {
            reader.BaseStream.Seek(92, SeekOrigin.Begin);
            FirstByte = reader.ReadUInt32() * BLOCKSIZE;
            Size = reader.ReadUInt32();
            Count = reader.ReadUInt32();

            const int ABF_ADCCOUNT = 16;
            nADCNum = new Int16[ABF_ADCCOUNT];
            nTelegraphEnable = new Int16[ABF_ADCCOUNT];
            nTelegraphInstrument = new Int16[ABF_ADCCOUNT];
            fTelegraphAdditGain = new Single[ABF_ADCCOUNT];
            fTelegraphFilter = new Single[ABF_ADCCOUNT];
            fTelegraphMembraneCap = new Single[ABF_ADCCOUNT];
            nTelegraphMode = new Int16[ABF_ADCCOUNT];
            fTelegraphAccessResistance = new Single[ABF_ADCCOUNT];
            nADCPtoLChannelMap = new Int16[ABF_ADCCOUNT];
            nADCSamplingSeq = new Int16[ABF_ADCCOUNT];
            fADCProgrammableGain = new Single[ABF_ADCCOUNT];
            fADCDisplayAmplification = new Single[ABF_ADCCOUNT];
            fADCDisplayOffset = new Single[ABF_ADCCOUNT];
            fInstrumentScaleFactor = new Single[ABF_ADCCOUNT];
            fInstrumentOffset = new Single[ABF_ADCCOUNT];
            fSignalGain = new Single[ABF_ADCCOUNT];
            fSignalOffset = new Single[ABF_ADCCOUNT];
            fSignalLowpassFilter = new Single[ABF_ADCCOUNT];
            fSignalHighpassFilter = new Single[ABF_ADCCOUNT];
            nLowpassFilterType = new Char[ABF_ADCCOUNT];
            nHighpassFilterType = new Char[ABF_ADCCOUNT];
            fPostProcessLowpassFilter = new Single[ABF_ADCCOUNT];
            nPostProcessLowpassFilterType = new Char[ABF_ADCCOUNT];
            bEnabledDuringPN = new Byte[ABF_ADCCOUNT];
            nStatsChannelPolarity = new Int16[ABF_ADCCOUNT];
            lADCChannelNameIndex = new Int32[ABF_ADCCOUNT];
            lADCUnitsIndex = new Int32[ABF_ADCCOUNT];

            for (int i = 0; i < Count; i++)
            {
                reader.BaseStream.Seek(FirstByte + i * Size, SeekOrigin.Begin);
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
                nLowpassFilterType[adcNum] = reader.ReadChar();
                nHighpassFilterType[adcNum] = reader.ReadChar();
                fPostProcessLowpassFilter[adcNum] = reader.ReadSingle();
                nPostProcessLowpassFilterType[adcNum] = reader.ReadChar();
                bEnabledDuringPN[adcNum] = reader.ReadByte();
                nStatsChannelPolarity[adcNum] = reader.ReadInt16();
                lADCChannelNameIndex[adcNum] = reader.ReadInt32();
                lADCUnitsIndex[adcNum] = reader.ReadInt32();
            }

            // nADCPtoLChannelMap is a hot mess!
            // It's not actually stored in the ABF file. It must be calculated later.
            // https://github.com/swharden/ABFsharp/blob/5b79204068aec7258be6f3c02bd2ca6c125bab5c/dev/ABFFIO/axon32/AxAbfFio32/Oldheadr.cpp
            // This solution works for 99% of cases
            for (int i = 0; i < ABF_ADCCOUNT; i++)
                nADCSamplingSeq[i] = 0;
            for (int i = 0; i < Count; i++)
                nADCSamplingSeq[i] = nADCPtoLChannelMap[nADCNum[i]];
        }
    }
}
