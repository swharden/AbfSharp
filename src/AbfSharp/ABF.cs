namespace AbfSharp;

/// <summary>
/// A simple interface to ABF file header and sweep data provided by ABFFIO.DLL
/// </summary>
public class ABF
{
    public AbfHeader Header { get; }
    public Tag[] Tags { get; }
    public string TagSummaries => string.Join(", ", Tags.Select(x => x.Summary));
    public string FilePath { get; }
    public string Filename => Path.GetFileName(FilePath);
    public string AbfID => Path.GetFileNameWithoutExtension(FilePath);
    public float SampleRate { get; }
    public float SamplePeriod => 1.0f / SampleRate;
    public int SweepCount { get; }
    public int ChannelCount { get; }
    public double AbfLength => Header.AbfLength;
    public double SweepLength => Header.SweepLength;
    public double[] SweepStartTimes => Enumerable.Range(0, SweepCount).Select(x => SweepLength * x).ToArray();
    public AbfReport Report { get; }
    public Epoch[] Epochs { get; }

    // data by sweep (interleaved by channel)
    private float[][]? SweepData = null; // TODO: make double

    public ABF(string filePath, bool preloadSweepData = true)
    {
        FilePath = Path.GetFullPath(filePath);

        using ABFFIO.AbfFileInterface abfInterface = new(filePath);
        Header = new AbfHeader(abfInterface);
        SampleRate = Header.SampleRate;
        SweepCount = Header.SweepCount;
        ChannelCount = Header.ChannelCount;
        Tags = new AbfTagManager(abfInterface, Header.AbfFileHeader).Tags;
        Report = new(this);
        Epochs = Enumerable.Range(0, Header.EpochCount).Select(x => new Epoch(this, x)).ToArray();
        if (preloadSweepData)
            LoadAllSweeps(abfInterface);
    }

    private void LoadAllSweeps(ABFFIO.AbfFileInterface wrapper)
    {
        SweepData ??= new float[SweepCount * ChannelCount][];

        for (int sweepIndex = 0; sweepIndex < SweepCount; sweepIndex++)
        {
            for (int channelIndex = 0; channelIndex < ChannelCount; channelIndex++)
            {
                int sweepDataIndex = sweepIndex * ChannelCount + channelIndex;

                if (SweepData[sweepDataIndex] is null)
                    SweepData[sweepDataIndex] = wrapper.ReadChannel(sweepIndex + 1, channelIndex);
            }
        }
    }

    public override string ToString() =>
        $"{Path.GetFileName(FilePath)} " +
        $"ABF (version {Header.AbfFileHeader.fFileVersionNumber}) " +
        $"{(OperationMode)Header.OperationMode} mode " +
        $"with {ChannelCount} channels and {SweepCount} sweeps";

    public float[] GetSweepF(int sweepIndex, int channelIndex = 0)
    {
        SweepData ??= new float[SweepCount * ChannelCount][];

        int sweepDataIndex = sweepIndex * ChannelCount + channelIndex;
        if (SweepData[sweepDataIndex] is null)
        {
            using ABFFIO.AbfFileInterface wrapper = new(FilePath);
            float[] values = wrapper.ReadChannel(sweepIndex + 1, channelIndex);
            SweepData[sweepDataIndex] = values;
        }

        return SweepData[sweepDataIndex];
    }

    public double[] GetSweepD(int sweepIndex, int channelIndex = 0)
    {
        float[] values = GetSweepF(sweepIndex, channelIndex);
        double[] values2 = new double[values.Length];
        for (int i = 0; i < values2.Length; i++)
        {
            values2[i] = values[i];
        }
        return values2;
    }

    public Sweep GetSweep(int sweepIndex, int channelIndex = 0)
    {
        return new Sweep(this, sweepIndex, channelIndex);
    }

    public IEnumerable<Sweep> GetSweeps(int channelIndex = 0)
    {
        for (int i = 0; i < SweepCount; i++)
        {
            yield return GetSweep(i, channelIndex);
        }
    }

    public float[] GetStimulusWaveform(int sweepIndex, int channelIndex = 0)
    {
        using ABFFIO.AbfFileInterface abfInterface = new(FilePath);
        float[] values = abfInterface.GetStimulusWaveform(sweepIndex + 1, channelIndex);
        return values;
    }

    public Sweep GetAllData(int channelIndex = 0)
    {
        int samplesPerSweep = Header.AbfFileHeader.lNumSamplesPerEpisode / Header.AbfFileHeader.nADCNumChannels;
        int sweepCount = Header.AbfFileHeader.lActualEpisodes;
        double[] values = new double[samplesPerSweep * sweepCount];

        int offset = 0;
        for (int sweepIndex = 0; sweepIndex < SweepCount; sweepIndex++)
        {
            float[] sweepValues = GetSweepF(sweepIndex, channelIndex);
            for (int i = 0; i < sweepValues.Length; i++)
            {
                values[offset++] = sweepValues[i];
            }
        }

        return new Sweep(values, SampleRate, 0, channelIndex, 0);
    }

    public Sweep GetAllDataDecimated(int channelIndex = 0, int decimation = 100)
    {
        int samplesPerSweep = Header.AbfFileHeader.lNumSamplesPerEpisode / Header.AbfFileHeader.nADCNumChannels;
        int sweepCount = Header.AbfFileHeader.lActualEpisodes;
        double[] values = new double[samplesPerSweep * sweepCount / decimation];

        int offset = 0;
        for (int sweepIndex = 0; sweepIndex < SweepCount; sweepIndex++)
        {
            float[] sweepValues = GetSweepF(sweepIndex, channelIndex);
            for (int i = 0; i < sweepValues.Length; i += decimation)
            {
                values[offset++] = sweepValues[i];
            }
        }

        return new Sweep(values, SampleRate / decimation, 0, channelIndex, 0);
    }
}
