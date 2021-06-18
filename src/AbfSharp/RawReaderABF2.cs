using AbfSharp.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp
{
    /// <summary>
    /// Methods to extract information from the ABF2 header
    /// https://swharden.com/pyabf/abf2-file-format
    /// </summary>
    public class RawReaderABF2 : RawReader
    {
        UInt32 protocolSection_firstByte;
        UInt32 protocolSection_size;
        UInt32 protocolSection_count;
        UInt32 adcSection_firstByte;
        UInt32 adcSection_size;
        UInt32 adcSection_count;
        UInt32 stringsSection_firstByte;
        UInt32 stringsSection_size;
        UInt32 stringsSection_count;
        UInt32 dataSection_firstByte;
        UInt32 dataSection_size;
        UInt32 dataSection_count;

        public RawReaderABF2(BinaryReader reader) : base(reader)
        {
            // ABF2 files have different sections that start at different byte locations.
            // The location of each section is stored at fixed byte positions at the start of the file.

            // get section byte location information
            protocolSection_firstByte = ReadUInt32(76) * 512;
            protocolSection_size = ReadUInt32();
            protocolSection_count = ReadUInt32();
            adcSection_firstByte = ReadUInt32(92) * 512;
            adcSection_size = ReadUInt32();
            adcSection_count = ReadUInt32();
            stringsSection_firstByte = ReadUInt32(220) * 512;
            stringsSection_size = ReadUInt32();
            stringsSection_count = ReadUInt32();
            dataSection_firstByte = ReadUInt32(236) * 512;
            dataSection_size = ReadUInt32();
            dataSection_count = ReadUInt32();
        }

        public override Version GetFileVersion()
        {
            return new Version(2, 0);
        }

        public override OperationMode GetOperationMode()
        {
            // operation mode is gap free, episodic, etc.
            // in ABF2 files it's an integer at the start of the protocol section.
            int nOperationMode = ReadUInt16(protocolSection_firstByte);
            return (OperationMode)nOperationMode;
        }
    }
}
