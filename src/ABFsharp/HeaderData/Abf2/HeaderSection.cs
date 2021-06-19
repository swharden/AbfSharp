using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.HeaderData.Abf2
{
    public class HeaderSection
    {
        public readonly string fFileSignature;
        public readonly float fFileVersionNumber;
        public readonly UInt32 uFileInfoSize;
        public readonly UInt32 lActualEpisodes;
        public readonly UInt32 uFileStartDate;
        public readonly UInt32 uFileStartTimeMS;
        public readonly UInt32 uStopwatchTime;
        public readonly UInt16 nFileType;
        public readonly UInt16 nDataFormat;
        public readonly UInt16 nSimultaneousScan;
        public readonly UInt16 nCRCEnable;
        public readonly UInt32 uFileCRC;
        public readonly byte[] FileGUID;
        public readonly UInt32 unknown1;
        public readonly UInt32 unknown2;
        public readonly UInt32 unknown3;
        public readonly UInt32 uCreatorVersion;
        public readonly UInt32 uCreatorNameIndex;
        public readonly UInt32 uModifierVersion;
        public readonly UInt32 uModifierNameIndex;
        public readonly UInt32 uProtocolPathIndex;

        public HeaderSection(BinaryReader reader)
        {
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            fFileSignature = string.Join("", reader.ReadChars(4));

            // TODO: locate documentation that describes this
            byte[] v = reader.ReadBytes(4);
            fFileVersionNumber = v[3] + v[2] * 0.01f + v[1] * .001f + v[0] * .0001f;

            uFileInfoSize = reader.ReadUInt32();
            lActualEpisodes = reader.ReadUInt32();
            uFileStartDate = reader.ReadUInt32();
            uFileStartTimeMS = reader.ReadUInt32();
            uStopwatchTime = reader.ReadUInt32();
            nFileType = reader.ReadUInt16();
            nDataFormat = reader.ReadUInt16();
            nSimultaneousScan = reader.ReadUInt16();
            nCRCEnable = reader.ReadUInt16();
            uFileCRC = reader.ReadUInt32();
            FileGUID = reader.ReadBytes(16);
            uCreatorVersion = reader.ReadUInt32();
            uCreatorNameIndex = reader.ReadUInt32();
            uModifierVersion = reader.ReadUInt32();
            uModifierNameIndex = reader.ReadUInt32();
            uProtocolPathIndex = reader.ReadUInt32();
        }
    }
}
