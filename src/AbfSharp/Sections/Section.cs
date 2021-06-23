using System.IO;

namespace AbfSharp.Sections
{
    /// <summary>
    /// This class describes a section of a variably-sized and variably-placed ABF2 header.
    /// </summary>
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
        /// This object represents a dynamic section (with variable posiition in size) in ABF 2 files.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="indexByte">Position indicating section location and size information</param>
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
