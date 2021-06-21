﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.HeaderData.Abf2
{
    public class Abf2Header
    {
        public readonly HeaderSection HeaderSection;
        public readonly ProtocolSection ProtocolSection;
        public readonly AdcSection AdcSection;
        public readonly DacSection DacSection;
        public readonly StringsSection StringsSection;
        public readonly TagSection TagSection;
        public readonly DataSection DataSection;
        public readonly SynchSection SynchSection;

        public Abf2Header(BinaryReader reader)
        {
            HeaderSection = new(reader);
            ProtocolSection = new(reader);
            AdcSection = new AdcSection(reader);
            DacSection = new DacSection(reader);
            StringsSection = new StringsSection(reader);
            TagSection = new TagSection(reader);
            DataSection = new DataSection(reader);
            SynchSection = new SynchSection(reader);
        }
    }
}