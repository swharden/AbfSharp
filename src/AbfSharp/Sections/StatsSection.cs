using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.Sections
{
    class StatsSection : Section
    {
        public StatsSection(BinaryReader reader) : base(reader, 348)
        {
        }
    }
}
