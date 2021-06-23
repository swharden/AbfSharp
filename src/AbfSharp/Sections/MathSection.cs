using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.Sections
{
    class MathSection : Section
    {
        public MathSection(BinaryReader reader) : base(reader, 204)
        {
        }
    }
}
