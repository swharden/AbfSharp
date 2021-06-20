using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.HeaderData.Abf2
{
    public class Section
    {
        /// <summary>
        /// Block number where this section starts in the ABF file.
        /// </summary>
        public readonly uint SectionBlock;

        /// <summary>
        /// Byte position where this section starts in the ABF file.
        /// </summary>
        public readonly uint SectionStart;

        /// <summary>
        /// Size (bytes) of each item in this section.
        /// </summary>
        public readonly uint SectionSize;

        /// <summary>
        /// Number of items in this section.
        /// </summary>
        public readonly uint SectionCount;

        private const int BLOCKSIZE = 512;

        /// <summary>
        /// This object describes information about a section in an ABF2 file
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="indexByte">Byte position to read section location and size information</param>
        public Section(BinaryReader reader, int indexByte)
        {
            reader.BaseStream.Seek(indexByte, SeekOrigin.Begin);
            SectionBlock = reader.ReadUInt32();
            SectionStart = SectionBlock * BLOCKSIZE;
            SectionSize = reader.ReadUInt32();
            SectionCount = reader.ReadUInt32();
        }
    }
}
