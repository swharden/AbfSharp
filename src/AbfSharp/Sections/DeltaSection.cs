using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.Sections
{
    class DeltaSection : Section
    {
        public DeltaSection(BinaryReader reader) : base(reader, 284)
        {
        }
    }
}
