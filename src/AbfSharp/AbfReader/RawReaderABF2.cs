using AbfSharp.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.AbfReader
{
    /// <summary>
    /// Methods to extract information from the ABF2 header
    /// https://swharden.com/pyabf/abf2-file-format
    /// </summary>
    public class RawReaderABF2 : RawReader
    {
        private readonly AbfReader.Section ProtocolSection;
        private readonly AbfReader.Section AdcSection;
        private readonly AbfReader.Section StringsSection;
        private readonly AbfReader.Section DataSection;

        private const int BLOCKSIZE = 512;

        public readonly HeaderSection HeaderSection;

        public RawReaderABF2(BinaryReader reader) : base(reader)
        {
            // ABF2 files have different sections that start at different byte locations.
            // The location of each section is stored at fixed byte positions at the start of the file.

            HeaderSection = new HeaderSection(reader);

            AbfReader.Section GetSection(long position)
            {
                Seek(position);
                return new(ReadUInt32() * BLOCKSIZE, ReadUInt32(), ReadUInt32());
            }

            ProtocolSection = GetSection(76);
            AdcSection = GetSection(92);
            StringsSection = GetSection(220);
            DataSection = GetSection(236);
        }

        public override float GetFileVersion() => HeaderSection.fFileVersionNumber;

        public override OperationMode GetOperationMode()
        {
            // operation mode is gap free, episodic, etc.
            // in ABF2 files it's an integer at the start of the protocol section.
            Seek(ProtocolSection.FirstByte);
            int nOperationMode = ReadUInt16();
            return (OperationMode)nOperationMode;
        }
    }
}
