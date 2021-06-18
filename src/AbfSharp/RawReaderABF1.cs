using AbfSharp.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp
{
    /// <summary>
    /// Methods to extract information from the ABF1 header
    /// https://swharden.com/pyabf/abf1-file-format
    /// </summary>
    public class RawReaderABF1 : RawReader
    {
        public RawReaderABF1(BinaryReader reader) : base(reader)
        {
        }

        public override Version GetFileVersion()
        {
            return new Version(1, 0);
        }

        public override OperationMode GetOperationMode()
        {
            // Operation mode is gap free, episodic, etc.
            // Integer at byte 8 in the file.
            Reader.BaseStream.Seek(8, SeekOrigin.Begin);
            UInt16 nOperationMode = Reader.ReadUInt16();
            return (OperationMode)nOperationMode;
        }
    }
}
