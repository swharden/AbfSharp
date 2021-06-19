using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace AbfSharp.HeaderData.Abf2
{
    public class StringsSection
    {
        private const int BLOCKSIZE = 512;

        public readonly string[] Strings;

        public StringsSection(BinaryReader reader)
        {
            reader.BaseStream.Seek(220, SeekOrigin.Begin);
            long firstByte = reader.ReadUInt32() * BLOCKSIZE;
            long size = reader.ReadUInt32();
            long count = reader.ReadUInt32();

            int offset = 44; // measured with hex editor
            reader.BaseStream.Seek(firstByte + offset, SeekOrigin.Begin);
            while (reader.ReadByte() == 0) { };
            reader.BaseStream.Seek(-1, SeekOrigin.Current);

            StringBuilder sb = new();
            sb.Append("first\n");
            char last = '\0';
            for (int i = 0; i < size; i++)
            {
                char c = reader.ReadChar();
                if (c == '\0')
                {
                    sb.Append('\n');
                    if (last == '\0')
                        break;
                }
                else
                {
                    sb.Append(c);
                }
                last = c;
            }
            Strings = sb.ToString().Split('\n');
            for (int i = 0; i < Strings.Length; i++)
                Strings[i] = Strings[i].Replace("\0", "").Trim();
        }
    }
}