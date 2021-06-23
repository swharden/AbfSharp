using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.Sections
{
    class VoiceTagSection : Section
    {
        public VoiceTagSection(BinaryReader reader) : base(reader, 300)
        {
        }
    }
}
