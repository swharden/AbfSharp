using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp
{
    public class HeaderAbf1 : HeaderBase
    {
        public HeaderAbf1(BinaryReader reader, string filePath)
        {
            Path = System.IO.Path.GetFullPath(filePath);
            Read(reader);
        }

        public HeaderAbf1(string filePath)
        {
            Path = System.IO.Path.GetFullPath(filePath);
            using Stream fs = File.Open(filePath, FileMode.Open);
            using BinaryReader reader = new(fs);
            Read(reader);
        }

        private void Read(BinaryReader reader)
        {
            ReadGroup1(reader);
        }

        private void ReadGroup1(BinaryReader reader)
        {
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            SignatureBytes = reader.ReadBytes(4);
            fFileVersionNumber = reader.ReadSingle();
            nOperationMode = reader.ReadInt16();
            lActualAcqLength = reader.ReadInt32();
            nNumPointsIgnored = reader.ReadInt16();
            lActualEpisodes = reader.ReadInt32();

            // fix Y2K bug
            int datecode = reader.ReadInt32();
            int day = datecode % 100;
            datecode /= 100;
            int month = datecode % 100;
            datecode /= 100;
            int year = datecode;
            if (year < 80)
                year += 2000;
            if (year < 100)
                year += 1900;
            uFileStartDate = (uint)(year) * 10000 + (uint)month * 100 + (uint)day;

            uFileStartTimeMS = (uint)reader.ReadInt32() * 1000;
            lStopwatchTime = reader.ReadInt32();

            // actually the header version number is just the file version number
            fHeaderVersionNumber = reader.ReadSingle();
            fHeaderVersionNumber = fFileVersionNumber;

            nFileType = reader.ReadInt16();

            // Add nFileStartMillisecs from group 6
            reader.BaseStream.Seek(366, SeekOrigin.Begin);
            uFileStartTimeMS += (uint)reader.ReadInt16();
        }
    }
}
