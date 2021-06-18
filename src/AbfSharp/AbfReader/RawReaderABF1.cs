using AbfSharp.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.AbfReader
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

        public override float GetFileVersion()
        {
            Seek(4);
            return Reader.ReadSingle();
        }

        public override OperationMode GetOperationMode()
        {
            // Operation mode is gap free, episodic, etc.
            // Integer at byte 8 in the file.
            Seek(8);
            int nOperationMode = ReadUInt16();
            return (OperationMode)nOperationMode;
        }
    }
}
