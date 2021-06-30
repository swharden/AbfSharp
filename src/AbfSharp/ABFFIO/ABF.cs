using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AbfSharp.ABFFIO
{
    /// <summary>
    /// This class provides a simple .NET interface to ABF file header and sweep data provided by ABFFIO.DLL
    /// </summary>
    public class ABF
    {
        public readonly AbfFileHeader Header;

        public readonly string FilePath;

        public ABF(string filePath)
        {
            FilePath = System.IO.Path.GetFullPath(filePath);
            using Wrapper abffio = new(filePath);
            Header = abffio.header;
        }

        public override string ToString() =>
            $"{System.IO.Path.GetFileName(FilePath)} " +
            $"ABF (version {Header.fFileVersionNumber}) " +
            $"{(OperationMode)Header.nOperationMode} mode " +
            $"with {Header.nADCNumChannels} channels and {Header.lActualEpisodes} sweeps";

        public double[] GetSweep(int sweepIndex, int channelIndex = 0)
        {
            using Wrapper abffio = new(FilePath);
            abffio.ReadChannel(sweepIndex + 1, channelIndex);
            double[] values = new double[abffio.buffer.Length];
            Array.Copy(abffio.buffer, 0, values, 0, abffio.buffer.Length);
            return values;
        }
    }
}
