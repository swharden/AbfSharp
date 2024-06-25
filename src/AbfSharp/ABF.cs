using System;
using System.Linq;
using AbfSharp.ABFFIO;

namespace AbfSharp
{
    /// <summary>
    /// This class provides a simple .NET interface to ABF file header and sweep data provided by ABFFIO.DLL
    /// </summary>
    public class ABF
    {
        public readonly AbfFileHeader Header;
        public Tags Tags { get; private set; }
        public string FilePath { get; private set; }
        public OperationMode OperationMode => (OperationMode)Header.nOperationMode;
        public string[] DacNames => Header.sDACChannelName.Select(x => x.ToString()).ToArray();
        public string[] DacUnits => Header.sDACChannelUnits.Select(x => x.ToString()).ToArray();
        public string[] AdcNames => Header.sADCChannelName.Select(x => x.ToString()).ToArray();
        public string[] AdcUnits => Header.sADCUnits.Select(x => x.ToString()).ToArray();
        public float SampleRate => 1e6f / Header.fADCSequenceInterval / Header.nADCNumChannels;
        public float SamplePeriod => 1.0f / SampleRate;
        public float SamplePeriodMS => SamplePeriod * 1e3f;
        public float FileVersion => Header.fFileVersionNumber;
        public int SweepCount => Math.Max(1, Header.lActualEpisodes);
        public int ChannelCount => Header.nADCNumChannels;

        /// <summary>
        /// Sweep data loaded from the ABF is stored in memory for faster access later.
        /// Primary index is sweep data interleaved by channel (S1C1, S1C2, S2C1, S2C2, S3C1, etc...)
        /// </summary>
        private readonly float[][] SweepData;

        /// <summary>
        /// Load an ABF using the official library (ABFFIO.DLL)
        /// </summary>
        /// <param name="filePath">Path to the ABF</param>
        /// <param name="preloadSweepData">If True, sweep data will be loaded into memory up-front for faster (filesystem-free) access later</param>
        public ABF(string filePath, bool preloadSweepData = true)
        {
            FilePath = System.IO.Path.GetFullPath(filePath);
            using Wrapper abffio = new(filePath);
            Header = abffio.GetHeader();
            Tags = new Tags(abffio.ReadTags(), Header);
            SweepData = new float[SweepCount * ChannelCount][];
            if (preloadSweepData)
                LoadAllSweeps(abffio);
        }

        private void LoadAllSweeps(Wrapper abffio)
        {
            for (int sweepIndex = 0; sweepIndex < SweepCount; sweepIndex++)
            {
                for (int channelIndex = 0; channelIndex < ChannelCount; channelIndex++)
                {
                    int sweepDataIndex = sweepIndex * ChannelCount + channelIndex;

                    if (SweepData[sweepDataIndex] is null)
                        SweepData[sweepDataIndex] = abffio.ReadChannel(sweepIndex + 1, channelIndex);
                }
            }
        }

        public override string ToString() =>
            $"{System.IO.Path.GetFileName(FilePath)} " +
            $"ABF (version {Header.fFileVersionNumber}) " +
            $"{(OperationMode)Header.nOperationMode} mode " +
            $"with {Header.nADCNumChannels} channels and {Header.lActualEpisodes} sweeps";

        public float[] GetSweep(int sweepIndex, int channelIndex = 0)
        {
            int sweepDataIndex = sweepIndex * ChannelCount + channelIndex;
            if (SweepData[sweepDataIndex] is null)
            {
                using Wrapper abffio = new(FilePath);
                float[] values = abffio.ReadChannel(sweepIndex + 1, channelIndex);
                SweepData[sweepDataIndex] = values;
            }
            return SweepData[sweepDataIndex];
        }

        public float[] GetStimulusWaveform(int sweepIndex, int channelIndex = 0)
        {
            using Wrapper abffio = new(FilePath);
            float[] values = abffio.GetStimulusWaveform(sweepIndex + 1, channelIndex);
            return values;
        }
    }
}
