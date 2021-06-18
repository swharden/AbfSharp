using AbfSharp.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp
{
    public abstract class RawReader
    {
        public readonly BinaryReader Reader;

        public RawReader(BinaryReader reader)
        {
            Reader = reader;
        }

        public UInt32 ReadUInt32(long position = -1)
        {
            if (position >= 0)
                Reader.BaseStream.Seek(position, SeekOrigin.Begin);
            return Reader.ReadUInt32();
        }

        public int ReadUInt16(long position = -1)
        {
            if (position >= 0)
                Reader.BaseStream.Seek(position, SeekOrigin.Begin);
            Reader.BaseStream.Seek(position, SeekOrigin.Begin);
            return Reader.ReadUInt16();
        }

        public abstract OperationMode GetOperationMode();
        public abstract Version GetFileVersion();
    }
}
