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
        public Abf2Header(BinaryReader reader)
        {
            HeaderSection = new(reader);
            ProtocolSection = new(reader);
        }
    }
}
