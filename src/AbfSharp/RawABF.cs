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

        public override string ToString() =>
            $"{System.IO.Path.GetFileName(Path)} " +
            $"ABF (version {Header.FileVersionNumber}) " +
            $"{Header.OperationMode} mode " +
            $"with {Header.ChannelCount} channels and {Header.SweepCount} sweeps";

        public double[] GetSweep(int sweepIndex, int channelIndex = 0)
        {
            // determine the location of data bytes in the file
            int sweepFirstByte;
            int sweepPointCount;
            if (Header.SynchStartTimes.Length > 0)
            {
                sweepFirstByte = Header.lDataSectionPtr * 512;
                for (int i = 1; i < sweepIndex; i++)
                    sweepFirstByte += Header.SynchLengths[i - 1];
                sweepPointCount = Header.SynchLengths[sweepIndex] / Header.nADCNumChannels;
            }
            else if (Header.OperationMode == HeaderData.OperationMode.Episodic)
            {
                int valuesPerSweep = Header.lActualAcqLength / Header.lActualEpisodes;
                sweepFirstByte = Header.lDataSectionPtr * 512 + sweepIndex * valuesPerSweep * Header.BytesPerValue;
                sweepPointCount = valuesPerSweep / Header.nADCNumChannels;
            }
            else
            {
                sweepFirstByte = Header.lDataSectionPtr * 512;
                sweepPointCount = Header.lActualAcqLength / Header.nADCNumChannels;
            }

            int channelByteOffset = Header.BytesPerValue * channelIndex;
            double[] values = new double[sweepPointCount];

            // TODO: store this at the header level
            // read data out of the file
            BinaryReader reader = new(File.Open(Path, FileMode.Open));
            reader.BaseStream.Seek(sweepFirstByte, SeekOrigin.Begin);
            byte[] data = reader.ReadBytes(Header.lActualAcqLength * Header.BytesPerValue);
            reader.Close();
            reader.Dispose();

            if (Header.nDataFormat == 0)
            {
                int adcIndex = Header.nADCSamplingSeq[channelIndex];

                double gain = 1;
                gain /= Header.fInstrumentScaleFactor[adcIndex];
                gain /= Header.fSignalGain[adcIndex];
                gain /= Header.fADCProgrammableGain[adcIndex];
                gain *= Header.fADCRange;
                gain /= Header.lADCResolution;
                if (Header.nTelegraphEnable[adcIndex] > 0)
                    gain /= Header.fTelegraphAdditGain[adcIndex];

                double offset = 0;
                offset += Header.fInstrumentOffset[adcIndex];
                offset -= Header.fSignalOffset[adcIndex];

                int bytesPerSample = Header.BytesPerSample;
                for (int i = 0; i < sweepPointCount; i++)
                    values[i] = BitConverter.ToInt16(data, i * bytesPerSample + channelByteOffset) * gain + offset;
            }
            else if (Header.nDataFormat == 1)
            {
                int bytesPerSample = Header.BytesPerSample;
                for (int i = 0; i < sweepPointCount; i++)
                    values[i] = BitConverter.ToSingle(data, i * bytesPerSample + channelByteOffset);
            }
            else
                throw new NotImplementedException($"unsupported nDataFormat: {Header.nDataFormat}");


            return values;
        }
    }
}
