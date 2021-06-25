using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.Sections
{
    class UserListSection : Section
    {
        public short[] nULEnable = new short[8];
        public short[] nULParamToVary = new short[8];
        public short[] nULRepeat = new short[8];
        public short[] nStringIndex = new short[8];

        public UserListSection(BinaryReader reader) : base(reader, 172)
        {
            System.Diagnostics.Debug.WriteLine($"UL: {SectionStart} {SectionCount} {SectionSize}");

            for (int i = 0; i < SectionCount; i++)
            {

                reader.BaseStream.Seek(SectionStart + i * SectionSize, SeekOrigin.Begin);
                _ = reader.ReadInt16();
                _ = reader.ReadInt16();
                Int16 param = reader.ReadInt16();
                Int16 repeat = reader.ReadInt16();
                Int16 stringIndex = reader.ReadInt16();

                // only populate values if enabled
                if (param > 0)
                {
                    nULEnable[i] = 1;
                    nULParamToVary[i] = param;
                    nULRepeat[i] = repeat;
                    nStringIndex[i] = stringIndex;
                }
            }
        }
    }
}
