using System;
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
        public Abf2Header(BinaryReader reader)
        {
            HeaderSection = new(reader);
            ProtocolSection = new(reader);
            AdcSection = new AdcSection(reader);
        }
    }
}
