using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.Sections
{
    class ScopeSection : Section
    {
        public ScopeSection(BinaryReader reader) : base(reader, 268)
        {
        }
    }
}
