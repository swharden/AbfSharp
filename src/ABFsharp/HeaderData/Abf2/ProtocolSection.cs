using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.HeaderData.Abf2
{
    public class ProtocolSection
    {
        private const int BLOCKSIZE = 512;

        public uint nOperationMode { get; private set; }

        public ProtocolSection(BinaryReader reader)
        {
            reader.BaseStream.Seek(76, SeekOrigin.Begin);
            long firstByte = reader.ReadUInt32() * BLOCKSIZE;
            long size = reader.ReadUInt32();
            long count = reader.ReadUInt32();

            reader.BaseStream.Seek(firstByte, SeekOrigin.Begin);
            nOperationMode = reader.ReadUInt32();
        }
    }
}
