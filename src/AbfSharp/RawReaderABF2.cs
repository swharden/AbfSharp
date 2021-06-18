using AbfSharp.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp
{
    /// <summary>
    /// Methods to extract information from the ABF2 header
    /// https://swharden.com/pyabf/abf2-file-format
    /// </summary>
    public class RawReaderABF2 : RawReader
    {
        private struct Section
        {
            public readonly long FirstByte;
            public readonly long Size;
            public readonly long Count;
            public Section(long fb, long sz, long ct) => (FirstByte, Size, Count) = (fb, sz, ct);
        }

        private readonly Section ProtocolSection;
        private readonly Section AdcSection;
        private readonly Section StringsSection;
        private readonly Section DataSection;

        public RawReaderABF2(BinaryReader reader) : base(reader)
        {
            // ABF2 files have different sections that start at different byte locations.
            // The location of each section is stored at fixed byte positions at the start of the file.
            Section GetSection(long position) => new(ReadUInt32(position) * 512, ReadUInt32(), ReadUInt32());

            ProtocolSection = GetSection(76);
            AdcSection = GetSection(92);
            StringsSection = GetSection(220);
            DataSection = GetSection(236);
        }

        public override Version GetFileVersion()
        {
            return new Version(2, 0);
        }

        public override OperationMode GetOperationMode()
        {
            // operation mode is gap free, episodic, etc.
            // in ABF2 files it's an integer at the start of the protocol section.
            int nOperationMode = ReadUInt16(ProtocolSection.FirstByte);
            return (OperationMode)nOperationMode;
        }
    }
}
