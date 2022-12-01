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
    public class ABF
    {
        /// <summary>
        /// Full path to this ABF file on disk
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// This array stores the entire DataSection of the ABF in memory.
        /// This is used to generate sweeps later without requiring the file to be re-opened (or kept open).
        /// </summary>
        private byte[] DataBytes;

        /// <summary>
        /// ABF header information with only the most important fields.
        /// Field names are commonly identical to those in the ABFFIO header.
        /// </summary>
        public NativeReader.Header Header;

        public ABF(string abfFilePath, bool preloadData = false)
        {
            if (!File.Exists(abfFilePath))
                throw new FileNotFoundException($"file does not exist: {abfFilePath}");
            Path = System.IO.Path.GetFullPath(abfFilePath);

            using FileStream fs = File.Open(Path, FileMode.Open);
            using BinaryReader reader = new(fs);

            string signature = Encoding.ASCII.GetString(reader.ReadBytes(4));
            if (signature == "ABF ")
                Header = new NativeReader.HeaderAbf1(reader, Path);
            else if (signature == "ABF2")
                Header = new NativeReader.HeaderAbf2(reader, Path);
            else
                throw new FormatException($"unknown file signature '{signature}': {Path}");

            if (preloadData)
                LoadData(reader);
        }

        public override string ToString() =>
            $"{System.IO.Path.GetFileName(Path)} " +
            $"ABF (version {Header.Version}) " +
            $"{Header.OperationMode} mode " +
            $"with {Header.ChannelCount} channels and {Header.SweepCount} sweeps";

        /// <summary>
        /// Load ADC data into memory as a byte array.
        /// This overload reads data using an already-open file reader. 
        /// </summary>
        private void LoadData(BinaryReader reader)
        {
            reader.BaseStream.Seek(Header.DataPosition, SeekOrigin.Begin);
            DataBytes = reader.ReadBytes(Header.DataSize * Header.BytesPerValue);
        }

        /// <summary>
        /// Load ADC data into memory as a byte array.
        /// This overload opens the file, reads the data, and closes the file. 
        /// </summary>
        private void LoadData()
        {
            using FileStream fs = File.Open(Path, FileMode.Open);
            using BinaryReader reader = new(fs);
            LoadData(reader);
        }

        /// <summary>
        /// ADC values for a single sweep/channel
        /// </summary>
        /// <param name="sweepIndex">sweep index (starts at 0)</param>
        /// <param name="channelIndex">channel index (starts at 0)</param>
        /// <returns></returns>
        public float[] GetSweep(int sweepIndex, int channelIndex = 0)
        {
            if (DataBytes is null)
                LoadData();

            (int sweepFirstByte, int sweepSampleCount) = GetSweepLocation(sweepIndex);

            return Header.nDataFormat switch
            {
                0 => GetSweepInt16(channelIndex, sweepFirstByte, sweepSampleCount),
                1 => GetSweepValues_FloatingPoint(channelIndex, sweepFirstByte, sweepSampleCount),
                _ => throw new NotImplementedException($"unsupported nDataFormat: {Header.nDataFormat}")
            };
        }

        /// <summary>
        /// Return all ADC values for the given channel
        /// </summary>
        public float[] GetAllData(int channelIndex = 0)
        {
            const int fixedLengthEventDrivenMode = 2;
            const int gapFreeMode = 3;
            const int highSpeedOscilloscopeMode = 4;
            const int episodicStimulationMode = 5;

            if (Header.nOperationMode == gapFreeMode || Header.nOperationMode == highSpeedOscilloscopeMode)
            {
                return GetSweep(0, channelIndex);
            }
            else if (Header.nOperationMode == fixedLengthEventDrivenMode || Header.nOperationMode == episodicStimulationMode)
            {
                int pointCount = Header.SweepCount * Header.lNumSamplesPerEpisode;
                float[] values = new float[pointCount];
                for (int i = 0; i < Header.SweepCount; i++)
                {
                    float[] episodeValues = GetSweep(i, channelIndex);
                    int offset = i * Header.lNumSamplesPerEpisode;
                    Array.Copy(episodeValues, 0, values, offset, episodeValues.Length);
                }
                return values;
            }
            else
            {
                throw new NotSupportedException($"nOperationMode: {Header.nOperationMode}");
            }
        }

        /// <summary>
        /// Return the location in memory for a given sweep
        /// </summary>
        private (int byteOffset, int pointCount) GetSweepLocation(int sweepIndex)
        {
            if (Header.OperationMode == OperationMode.Episodic)
            {
                // assume a fixed-length sleep (the total length divided by the number of episodes)
                int valuesPerSweep = Header.lActualAcqLength / Header.lActualEpisodes;
                int sweepPointCount = valuesPerSweep / Header.nADCNumChannels;
                int sweepByteOffset = sweepIndex * sweepPointCount * Header.BytesPerSample;
                return (sweepByteOffset, sweepPointCount);
            }
            else if (Header.OperationMode == OperationMode.EventDriven)
            {
                // measure the length of all previous events to determine where this one starts
                int sweepByteOffset = 0;
                for (int i = 0; i < sweepIndex; i++)
                    sweepByteOffset += Header.SynchLengths[i] * Header.BytesPerSample;
                int sweepPointCount = Header.SynchLengths[sweepIndex] / Header.nADCNumChannels;
                return (sweepByteOffset, sweepPointCount);
            }
            else
            {
                // treat other types (gap-free, oscilloscope, etc.) as one large sweep the entire length of the file
                int sweepPointCount = Header.lActualAcqLength / Header.nADCNumChannels;
                int sweepByteOffset = 0;
                return (sweepByteOffset, sweepPointCount);
            }
        }

        /// <summary>
        /// Return an array of sweep values from data stored in Int16 format.
        /// This method uses scaling information stored in the header to perform the conversion.
        /// </summary>
        private float[] GetSweepInt16(int channelIndex, int sweepByteOffset, int sweepPointCount)
        {
            float[] values = new float[sweepPointCount];
            int adcIndex = Header.nADCSamplingSeq[channelIndex];
            int channelByteOffset = Header.BytesPerValue * channelIndex;

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

            return values;
        }

        /// <summary>
        /// Return an array of sweep values from data stored using floating-point format.
        /// </summary>
        private float[] GetSweepValues_FloatingPoint(int channelIndex, int sweepByteOffset, int sweepPointCount)
        {
            int bytesPerSample = Header.BytesPerSample;
            int offset = Header.BytesPerValue * channelIndex + sweepByteOffset;

            float[] values = new float[sweepPointCount];
            for (int i = 0; i < sweepPointCount; i++)
                values[i] = BitConverter.ToSingle(DataBytes, i * bytesPerSample + offset);

            return values;
        }
    }
}
