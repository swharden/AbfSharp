using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.HeaderData.Abf2
{
    public class EpochPerDacSection : Section
    {
        public readonly short[] nEpochNum;
        public readonly short[] nDACNum;
        public readonly short[] nEpochType;
        public readonly float[] fEpochInitLevel;
        public readonly float[] fEpochLevelInc;
        public readonly int[] lEpochInitDuration;
        public readonly int[] lEpochDurationInc;
        public readonly int[] lEpochPulsePeriod;
        public readonly int[] lEpochPulseWidth;

        public EpochPerDacSection(BinaryReader reader) : base(reader, 156)
        {
            nEpochNum = new short[SectionCount];
            nDACNum = new short[SectionCount];
            nEpochType = new short[SectionCount];
            fEpochInitLevel = new float[SectionCount];
            fEpochLevelInc = new float[SectionCount];
            lEpochInitDuration = new int[SectionCount];
            lEpochDurationInc = new int[SectionCount];
            lEpochPulsePeriod = new int[SectionCount];
            lEpochPulseWidth = new int[SectionCount];

            for (int i = 0; i < SectionCount; i++)
            {
                reader.BaseStream.Seek(SectionStart + i * SectionSize, SeekOrigin.Begin);
                nEpochNum[i] = reader.ReadInt16();
                int epochNum = nEpochNum[i];

                nDACNum[epochNum] = reader.ReadInt16();
                nEpochType[epochNum] = reader.ReadInt16();
                fEpochInitLevel[epochNum] = reader.ReadSingle();
                fEpochLevelInc[epochNum] = reader.ReadSingle();
                lEpochInitDuration[epochNum] = reader.ReadInt32();
                lEpochDurationInc[epochNum] = reader.ReadInt32();
                lEpochPulsePeriod[epochNum] = reader.ReadInt32();
                lEpochPulseWidth[epochNum] = reader.ReadInt32();
            }
        }
    }
}
