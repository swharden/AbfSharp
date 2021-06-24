﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.Sections
{
    public class SynchArraySection : Section
    {
        /// <summary>
        /// Start time of sweep in fSynchTimeUnit units.
        /// </summary>
        public readonly Int32[] lStart;

        /// <summary>
        /// Length of the sweep in multiplexed samples.
        /// </summary>
        public readonly Int32[] lLength;

        public SynchArraySection(BinaryReader reader) : base(reader, 316)
        {
            lStart = new Int32[SectionCount];
            lLength = new Int32[SectionCount];

            for (int i = 0; i < SectionCount; i++)
            {
                reader.BaseStream.Seek(SectionStart + i * SectionSize, SeekOrigin.Begin);
                lStart[i] = reader.ReadInt32();
                lLength[i] = reader.ReadInt32();
            }
        }
    }
}