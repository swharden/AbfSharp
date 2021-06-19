using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
        /// Length of the data (bytes)
        /// </summary>
        public readonly uint Size;

        public AdcSection(BinaryReader reader)
        {
            // "h" = 2-byte int
            // "H" = 2-byte uint
            // "i" = 4-byte int
            // "I" = 4-byte uint
            // "l" = 4-byte int
            // "L" = 4-byte uint
            // "f" = 4-byte float

            reader.BaseStream.Seek(92, SeekOrigin.Begin);
            FirstByte = reader.ReadUInt32() * BLOCKSIZE;
            Size = reader.ReadUInt32();
            Count = reader.ReadUInt32();
        }
    }
}
