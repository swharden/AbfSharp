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
        public readonly string Path;

        public readonly Version AbfVersion;

        public RawABF(string abfFilePath)
        {
            // validate file path
            if (!File.Exists(abfFilePath))
                throw new FileNotFoundException($"file does not exist: {abfFilePath}");
            Path = System.IO.Path.GetFullPath(abfFilePath);

            // open the file and locally maintain the reader
            using BinaryReader reader = new(File.Open(Path, FileMode.Open));

            // read the first few bytes of the file to confirm it's an ABF
            byte[] signatureBytes = reader.ReadBytes(4);
            string signature = Encoding.ASCII.GetString(signatureBytes);
            if (!signature.StartsWith("ABF"))
                throw new FileLoadException($"file does not have expected ABF signature: {Path}");

            // character 4 indicaites if its an ABF1 or ABF2 format.
            // In ABF1 files I've seen byte 4 as 20 and also 50
            if (signature == "ABF ")
                AbfVersion = new Version(1, 0);
            else if (signature == "ABF2")
                AbfVersion = new Version(2, 0);
            else
                throw new FileLoadException($"unsupported ABF version (signature: {BitConverter.ToString(signatureBytes)}");

            // close the file
            reader.Close();
        }
    }
}
