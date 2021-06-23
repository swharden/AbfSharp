using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AbfSharp.Sections
{
    public class TagSection : Section
    {
        public readonly Int32[] lTagTime;
        public readonly string[] sComment;
        public readonly Int16[] nTagType;
        public readonly Int16[] nVoiceTagNumber;
        public TagSection(BinaryReader reader) : base(reader, 252)
        {
            lTagTime = new Int32[SectionCount];
            sComment = new string[SectionCount];
            nTagType = new Int16[SectionCount];
            nVoiceTagNumber = new Int16[SectionCount];

            for (int i = 0; i < SectionCount; i++)
            {
                reader.BaseStream.Seek(SectionStart + SectionSize * i, SeekOrigin.Begin);
                lTagTime[i] = reader.ReadInt32();
                sComment[i] = Encoding.ASCII.GetString(reader.ReadBytes(56)).Trim();
                nTagType[i] = reader.ReadInt16();
                nVoiceTagNumber[i] = reader.ReadInt16();
            }
        }

        public Tag[] GetTags(float tagTimeMult) => Enumerable.Range(0, (int)SectionCount)
            .Select(i => new Tag(lTagTime[i], sComment[i], nTagType[i], nVoiceTagNumber[i], tagTimeMult))
            .ToArray();
    }
}
