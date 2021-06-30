namespace AbfSharp
{
    public interface IABF
    {
        /// <summary>
        /// Full path to the ABF file on the local computer.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Number of ADC samples per second (Hz).
        /// One sample may contain multiple values if multiple ADC channels are in use.
        /// </summary>
        public float SampleRate { get; }

        /// <summary>
        /// Time (seconds) between recorded samples.
        /// One sample may contain multiple values if multiple ADC channels are in use.
        /// </summary>
        public float SamplePeriod { get; }

        /// <summary>
        /// Time (milliseconds) between recorded samples.
        /// One sample may contain multiple values if multiple ADC channels are in use.
        /// </summary>
        public float SamplePeriodMS { get; }

        /// <summary>
        /// Describes what type of experimental recording this is (episodic, gap-free, etc.)
        /// </summary>
        public OperationMode OperationMode { get; }

        /// <summary>
        /// Name of each DAC channel (by channel index, not logical index)
        /// </summary>
        public string[] DacNames { get; }

        /// <summary>
        /// Units of each DAC channel (by channel index, not logical index)
        /// </summary>
        public string[] DacUnits { get; }

        /// <summary>
        /// Name of each ADC channel (by channel index, not logical index)
        /// </summary>
        public string[] AdcNames { get; }

        /// <summary>
        /// Units of each ADC channel (by channel index, not logical index)
        /// </summary>
        public string[] AdcUnits { get; }

        /// <summary>
        /// Information about tags in the ABF
        /// </summary>
        public Tags Tags { get; }

        /// <summary>
        /// File format version stored in the data file during acquisition
        /// </summary>
        public float FileVersion { get; }

        /// <summary>
        /// Number of sweeps (episodes).
        /// This will be 1 for gap-free files (they are treated as episodic files with a single sweep).
        /// </summary>
        public int SweepCount { get; }

        /// <summary>
        /// Total number of ADC channels
        /// </summary>
        public int ChannelCount { get; }

        /// <summary>
        /// Return recorded ADC values for the given sweep and channel
        /// </summary>
        /// <param name="sweepIndex">sweep index (starting at zero)</param>
        /// <param name="channelIndex">channel index (starting at zero)</param>
        /// <returns></returns>
        public float[] GetSweep(int sweepIndex, int channelIndex = 0);

        /// <summary>
        /// Return DAC command values for the given sweep and channel
        /// </summary>
        /// <param name="sweepIndex">sweep index (starting at zero)</param>
        /// <param name="channelIndex">channel index (starting at zero)</param>
        /// <returns></returns>
        public float[] GetStimulusWaveform(int sweepIndex, int channelIndex = 0);
    }
}
