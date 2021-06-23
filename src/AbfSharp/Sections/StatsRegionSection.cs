using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.Sections
{
    class StatsRegionSection : Section
    {
        public StatsRegionSection(BinaryReader reader) : base(reader, 188)
        {
        }
    }
}
