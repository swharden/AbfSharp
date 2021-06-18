using AbfSharp.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp
{
    /// <summary>
    /// This ABF reader uses native .NET code (not the ABFFIO.DLL) so it can be used on other platforms
    /// like 64-bit Windows, Linux, MacOS, etc. It is more performant than ABFFIO.DLL for many operations,
    /// but not all features are supported.
    /// </summary>
    public class RawABF
    {
        public string Path { get; private set; }

        public Version AbfVersion { get; private set; }
        public OperationMode OperationMode { get; private set; }

        private BinaryReader Reader;

        public RawABF(string abfFilePath)
        {
            // validate file path
            if (!File.Exists(abfFilePath))
                throw new FileNotFoundException($"file does not exist: {abfFilePath}");
            Path = System.IO.Path.GetFullPath(abfFilePath);

            // open the file and locally maintain the reader
            Reader = new(File.Open(Path, FileMode.Open));

            // read the first few bytes of the file to confirm it's an ABF
            byte[] signatureBytes = Reader.ReadBytes(4);
            string signature = Encoding.ASCII.GetString(signatureBytes);
            if (!signature.StartsWith("ABF"))
                throw new FileLoadException($"file does not have expected ABF signature: {Path}");

            // character 4 indicaites if its an ABF1 or ABF2 format.
            // In ABF1 files I've seen byte 4 as 20 and also 50, but always should be a space.
            if (signature == "ABF ")
                ReadHeaderABF1(Reader);
            else if (signature == "ABF2")
                ReadHeaderABF2(Reader);
            else
                throw new FileLoadException($"unsupported ABF version (signature: {BitConverter.ToString(signatureBytes)}");

            // close the file
            Reader.Close();
            Reader.Dispose();
        }

        private UInt32 ReadUInt32(long position = -1)
        {
            if (position >= 0)
                Reader.BaseStream.Seek(position, SeekOrigin.Begin);
            return Reader.ReadUInt32();
        }

        private int ReadUInt16(long position = -1)
        {
            if (position >= 0)
                Reader.BaseStream.Seek(position, SeekOrigin.Begin);
            Reader.BaseStream.Seek(position, SeekOrigin.Begin);
            return Reader.ReadUInt16();
        }

        private void ReadHeaderABF1(BinaryReader reader)
        {
            AbfVersion = new Version(1, 0);

            // operation mode is gap free, episodic, etc.
            // in ABF1 files it's an integer at byte 8.
            reader.BaseStream.Seek(8, SeekOrigin.Begin);
            UInt16 nOperationMode = reader.ReadUInt16();
            OperationMode = (OperationMode)nOperationMode;
        }

        private void ReadHeaderABF2(BinaryReader reader)
        {
            AbfVersion = new Version(2, 0);

            // ABF2 files have different sections that start at different byte locations.
            // The location of each section is stored at fixed byte positions at the start of the file.

            // get section byte location information
            UInt32 protocolSection_firstByte = ReadUInt32(76) * 512;
            UInt32 protocolSection_size = ReadUInt32();
            UInt32 protocolSection_count = ReadUInt32();
            UInt32 adcSection_firstByte = ReadUInt32(92) * 512;
            UInt32 adcSection_size = ReadUInt32();
            UInt32 adcSection_count = ReadUInt32();
            UInt32 stringsSection_firstByte = ReadUInt32(220) * 512;
            UInt32 stringsSection_size = ReadUInt32();
            UInt32 stringsSection_count = ReadUInt32();
            UInt32 dataSection_firstByte = ReadUInt32(236) * 512;
            UInt32 dataSection_size = ReadUInt32();
            UInt32 dataSection_count = ReadUInt32();

            // operation mode is gap free, episodic, etc.
            // in ABF2 files it's an integer at the start of the protocol section.
            int nOperationMode = ReadUInt16(protocolSection_firstByte);
            OperationMode = (OperationMode)nOperationMode;
        }
    }
}
