using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.Sections
{
    /// <summary>
    /// This section contains the digital output signals for each epoch.
    /// Each output is a single byte, but represents 8 bits corresponding to 8 outputs (7->0)
    /// </summary>
    public class EpochSection : Section
    {
        /// <summary>
        /// Index of each epoch (A, B, C is 0, 1, 2, etc.)
        /// </summary>
        public readonly short[] nEpochNum;

        /// <summary>
        /// Output state for a each epoch (8 bits for 8 digital outputs)
        /// </summary>
        public readonly short[] nDigitalValue;

        /// <summary>
        /// I'm guessing 0 for steady-stae (based on nDigitalValue) and 1 for train (indicated by a * in waveform editor)
        /// </summary>
        public readonly short[] nDigitalTrainValue;

        public EpochSection(BinaryReader reader) : base(reader, 124)
        {
            nEpochNum = new short[SectionCount];
            nDigitalValue = new short[SectionCount];
            nDigitalTrainValue = new short[SectionCount];

            for (int i=0; i<SectionCount; i++)
            {
                reader.BaseStream.Seek(SectionStart + i * SectionSize, SeekOrigin.Begin);
                nEpochNum[i] = reader.ReadInt16();
                nDigitalValue[i] = reader.ReadInt16();
                nDigitalTrainValue[i] = reader.ReadInt16();
            }
        }
    }
}
