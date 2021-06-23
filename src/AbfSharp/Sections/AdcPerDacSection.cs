using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.Sections
{
    class AdcPerDacSection : Section
    {
        public AdcPerDacSection(BinaryReader reader) : base(reader, 140)
        {
        }
    }
}
