using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.Sections
{
    public class DataSection : Section
    {
        public DataSection(BinaryReader reader) : base(reader, 236)
        {

        }
    }
}
