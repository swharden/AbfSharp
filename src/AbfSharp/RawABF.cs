using AbfSharp.AbfReader;
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

        public float FileVersion { get; private set; }
        public OperationMode OperationMode { get; private set; }

        public RawABF(string abfFilePath)
        {
            // validate file path
            if (!File.Exists(abfFilePath))
                throw new FileNotFoundException($"file does not exist: {abfFilePath}");
            Path = System.IO.Path.GetFullPath(abfFilePath);

            // open the file and locally maintain the reader
            BinaryReader reader = new(File.Open(Path, FileMode.Open));

            // read the first few bytes of the file to confirm it's an ABF
            byte[] signatureBytes = reader.ReadBytes(4);
            string signature = Encoding.ASCII.GetString(signatureBytes);
            if (!signature.StartsWith("ABF"))
                throw new FileLoadException($"file does not have expected ABF signature: {Path}");

            // character 4 indicaites if its an ABF1 or ABF2 format.
            // In ABF1 files I've seen byte 4 as 20 and also 50, but always should be a space.
            RawReader abfReader;
            if (signature == "ABF ")
                abfReader = new RawReaderABF1(reader);
            else if (signature == "ABF2")
                abfReader = new RawReaderABF2(reader);
            else
                throw new FileLoadException($"unsupported ABF version (signature: {BitConverter.ToString(signatureBytes)}");

            // look things up in an ABF-type-dependent way
            OperationMode = abfReader.GetOperationMode();
            FileVersion = abfReader.GetFileVersion();

            // close the file
            reader.Close();
            reader.Dispose();
        }
    }
}
