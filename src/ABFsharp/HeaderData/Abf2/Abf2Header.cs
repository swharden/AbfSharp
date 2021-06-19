using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.HeaderData.Abf2
{
    public class Abf2Header
    {
        public HeaderSection HeaderSection { get; private set; }
        public ProtocolSection ProtocolSection { get; private set; }
        public Abf2Header(BinaryReader reader)
        {
            HeaderSection = new(reader);
            ProtocolSection = new(reader);
        }
    }
}
