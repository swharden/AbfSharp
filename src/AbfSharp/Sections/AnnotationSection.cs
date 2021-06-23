using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.Sections
{
    class AnnotationSection : Section
    {
        public AnnotationSection(BinaryReader reader) : base(reader, 332)
        {
        }
    }
}
