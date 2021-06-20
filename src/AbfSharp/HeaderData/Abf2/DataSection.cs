using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.HeaderData.Abf2
{
    public class DataSection : Section
    {
        public DataSection(BinaryReader reader) : base(reader, 236)
        {

        }
    }
}
