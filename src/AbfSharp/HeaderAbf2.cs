using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AbfSharp.HeaderData.Abf2;

namespace AbfSharp
{
    public class HeaderAbf2 : HeaderBase
    {
        private HeaderSection HeaderSection;
        private ProtocolSection ProtocolSection;
        private AdcSection AdcSection;
        private DacSection DacSection;
        private StringsSection StringsSection;
        private TagSection TagSection;
        private DataSection DataSection;
        private SynchSection SynchSection;

        public HeaderAbf2(BinaryReader reader, string filePath)
        {
            Path = System.IO.Path.GetFullPath(filePath);
            Read(reader);
        }

        public HeaderAbf2(string filePath)
        {
            Path = System.IO.Path.GetFullPath(filePath);
            using Stream fs = File.Open(filePath, FileMode.Open);
            using BinaryReader reader = new(fs);
            Read(reader);
        }

        private void Read(BinaryReader reader)
        {
            HeaderSection = new(reader);
            ProtocolSection = new(reader);
            AdcSection = new AdcSection(reader);
            DacSection = new DacSection(reader);
            StringsSection = new StringsSection(reader);
            TagSection = new TagSection(reader);
            DataSection = new DataSection(reader);
            SynchSection = new SynchSection(reader);

            ReadGroup1();
        }

        private void ReadGroup1()
        {
            SignatureBytes = HeaderSection.SignatureBytes;
            fFileVersionNumber = HeaderSection.fFileVersionNumber;
            nOperationMode = ProtocolSection.nOperationMode;
            lActualAcqLength = (int)DataSection.SectionCount;
            lActualEpisodes = (int)HeaderSection.lActualEpisodes;
            uFileStartDate = HeaderSection.uFileStartDate;
            uFileStartTimeMS = HeaderSection.uFileStartTimeMS;
            lStopwatchTime = (int)HeaderSection.uStopwatchTime;
            nFileType = (short)HeaderSection.nFileType;
        }
    }
}
