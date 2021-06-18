using AbfSharp.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.AbfReader
{
    public abstract class RawReader
    {
        public readonly BinaryReader Reader;

        public RawReader(BinaryReader reader) => Reader = reader;

        public void Seek(long position) => Reader.BaseStream.Seek(position, SeekOrigin.Begin);
        public long ReadUInt32() => Reader.ReadUInt32();
        public int ReadUInt16() => Reader.ReadUInt16();
        public abstract OperationMode GetOperationMode();
        public abstract float GetFileVersion();
    }
}
