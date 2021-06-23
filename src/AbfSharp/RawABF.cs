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

        private byte[] DataBytes;

        public HeaderData.Header Header { get; private set; }

        public RawABF(string abfFilePath, bool preloadData = false)
        {
            // validate file path
            if (!File.Exists(abfFilePath))
                throw new FileNotFoundException($"file does not exist: {abfFilePath}");
            Path = System.IO.Path.GetFullPath(abfFilePath);

            // open the file and locally maintain the reader
            using FileStream fs = File.Open(Path, FileMode.Open);
            using BinaryReader reader = new(fs);

            // read the header into memory
            Header = new(reader);

            // read all sweep data into memory
            if (preloadData)
                LoadData(reader);
        }

        public override string ToString() =>
            $"{System.IO.Path.GetFileName(Path)} " +
            $"ABF (version {Header.FileVersionNumber}) " +
            $"{Header.OperationMode} mode " +
            $"with {Header.ChannelCount} channels and {Header.SweepCount} sweeps";

        private void LoadData(BinaryReader reader)
        {
            reader.BaseStream.Seek(Header.DataPosition, SeekOrigin.Begin);
            DataBytes = reader.ReadBytes(Header.DataSize * Header.BytesPerValue);
        }

        private void LoadData()
        {
            using FileStream fs = File.Open(Path, FileMode.Open);
            using BinaryReader reader = new(fs);
            LoadData(reader);
        }

        public float[] GetSweep(int sweepIndex, int channelIndex = 0)
        {
            if (DataBytes is null)
                LoadData();

            // determine the location of this sweep in the data byte array
            int sweepByteOffset;
            int sweepPointCount;
            if (Header.OperationMode == HeaderData.OperationMode.Episodic)
            {
                int valuesPerSweep = Header.lActualAcqLength / Header.lActualEpisodes;
                sweepPointCount = valuesPerSweep / Header.nADCNumChannels;
                sweepByteOffset = sweepIndex * sweepPointCount * Header.BytesPerSample;
            }
            else if (Header.OperationMode == HeaderData.OperationMode.EventDriven)
            {
                sweepByteOffset = 0;
                for (int i = 0; i < sweepIndex; i++)
                    sweepByteOffset += Header.SynchLengths[i] * Header.BytesPerSample;
                sweepPointCount = Header.SynchLengths[sweepIndex] / Header.nADCNumChannels;
                Console.WriteLine("EventDriven");
                Console.WriteLine(string.Join(",", Header.SynchLengths));
                Console.WriteLine($"Sweep {sweepIndex} offset: {sweepByteOffset}");
            }
            else
            {
                // assume a single sweep the entire length of the file
                Console.WriteLine("SINGLESWEEP");
                sweepPointCount = Header.lActualAcqLength / Header.nADCNumChannels;
                sweepByteOffset = 0;
            }

            float[] values = new float[sweepPointCount];
            int channelByteOffset = Header.BytesPerValue * channelIndex;
            if (Header.nDataFormat == 0)
            {
                int adcIndex = Header.nADCSamplingSeq[channelIndex];

                float gain = 1;
                gain /= Header.fInstrumentScaleFactor[adcIndex];
                gain /= Header.fSignalGain[adcIndex];
                gain /= Header.fADCProgrammableGain[adcIndex];
                gain *= Header.fADCRange;
                gain /= Header.lADCResolution;
                if (Header.nTelegraphEnable[adcIndex] > 0)
                    gain /= Header.fTelegraphAdditGain[adcIndex];

                float offset = 0;
                offset += Header.fInstrumentOffset[adcIndex];
                offset -= Header.fSignalOffset[adcIndex];

                int bytesPerSample = Header.BytesPerSample;
                for (int i = 0; i < sweepPointCount; i++)
                    values[i] = BitConverter.ToInt16(DataBytes, sweepByteOffset + i * bytesPerSample + channelByteOffset) * gain + offset;
            }
            else if (Header.nDataFormat == 1)
            {
                int bytesPerSample = Header.BytesPerSample;
                for (int i = 0; i < sweepPointCount; i++)
                    values[i] = BitConverter.ToSingle(DataBytes, sweepByteOffset + i * bytesPerSample + channelByteOffset);
            }
            else
                throw new NotImplementedException($"unsupported nDataFormat: {Header.nDataFormat}");

            return values;
        }
    }
}
