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
        public readonly Tags Tags;
        public readonly string FilePath;
        public OperationMode OperationMode => (OperationMode)Header.nOperationMode;
        public string[] DacNames => Header.sDACChannelName.Select(x => x.ToString()).ToArray();
        public string[] DacUnits => Header.sDACChannelUnits.Select(x => x.ToString()).ToArray();
        public string[] AdcNames => Header.sADCChannelName.Select(x => x.ToString()).ToArray();
        public string[] AdcUnits => Header.sADCUnits.Select(x => x.ToString()).ToArray();
        public float SampleRate => 1e6f / Header.fADCSequenceInterval / Header.nADCNumChannels;
        public float SamplePeriod => 1.0f / SampleRate;
        public float SamplePeriodMS => SamplePeriod * 1e3f;

        public ABF(string filePath)
        {
            FilePath = System.IO.Path.GetFullPath(filePath);
            using Wrapper abffio = new(filePath);
            Header = abffio.GetHeader();
            Tags = new Tags(abffio.ReadTags(), Header);
        }

        public override string ToString() =>
            $"{System.IO.Path.GetFileName(FilePath)} " +
            $"ABF (version {Header.fFileVersionNumber}) " +
            $"{(OperationMode)Header.nOperationMode} mode " +
            $"with {Header.nADCNumChannels} channels and {Header.lActualEpisodes} sweeps";

        public float[] GetSweep(int sweepIndex, int channelIndex = 0)
        {
            // TODO: THIS IS VERY SLOW! PRE-LOAD DATA!
            using Wrapper abffio = new(FilePath);
            float[] values = abffio.ReadChannel(sweepIndex + 1, channelIndex);
            return values;
        }

        public float[] GetStimulusWaveform(int sweepIndex, int channelIndex = 0)
        {
            //if (OperationMode != OperationMode.Episodic)
            //throw new InvalidOperationException("stimulus waveform only supported for episodic files");

            // TODO: THIS IS VERY SLOW! PRE-LOAD DATA!
            using Wrapper abffio = new(FilePath);
            float[] values = abffio.GetStimulusWaveform(sweepIndex + 1, channelIndex);
            return values;
        }
    }
}
