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

        public readonly HeaderData.Header Header;

        public RawABF(string abfFilePath)
        {
            // validate file path
            if (!File.Exists(abfFilePath))
                throw new FileNotFoundException($"file does not exist: {abfFilePath}");
            Path = System.IO.Path.GetFullPath(abfFilePath);

            // open the file and locally maintain the reader
            BinaryReader reader = new(File.Open(Path, FileMode.Open));

            // read the header into memory
            Header = new(reader);

            // read the sweeps into memory
            // TODO: read sweeps

            // close the file
            reader.Close();
            reader.Dispose();
        }
    }
}
