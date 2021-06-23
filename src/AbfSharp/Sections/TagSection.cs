using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.Sections
{
    public class TagSection : Section
    {
        public readonly Int32[] lTagTime;
        public readonly string[] sComment;
        public readonly Int16[] nTagType;

        // voice tags intentionally not supported at this time
        // https://swharden.com/pyabf/abf1-file-format/#the-abf-tag-section

        public TagSection(BinaryReader reader) : base(reader, 252)
        {
            lTagTime = new Int32[SectionCount];
            sComment = new string[SectionCount];
            nTagType = new Int16[SectionCount];

            for (int i = 0; i < SectionCount; i++)
            {
                reader.BaseStream.Seek(SectionStart + SectionSize * i, SeekOrigin.Begin);
                lTagTime[i] = reader.ReadInt32();
                sComment[i] = Encoding.ASCII.GetString(reader.ReadBytes(56)).Trim();
                nTagType[i] = reader.ReadInt16();
            }
        }
    }
}
