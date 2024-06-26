namespace AbfSharp;

// TODO: cut all references to ABFFIO namespace

/// <summary>
/// This class provides a simple .NET interface to ABF file header and sweep data provided by ABFFIO.DLL
/// </summary>
public class ABF : IDisposable
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
    public AbfReport Report { get; }

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

    // TODO: pre-load this into memory too
    public float[] GetStimulusWaveform(int sweepIndex, int channelIndex = 0)
    {
        using ABFFIO.AbfFileInterface abfInterface = new(FilePath);
        float[] values = abfInterface.GetStimulusWaveform(sweepIndex + 1, channelIndex);
        return values;
    }

    public void Dispose()
    {

    }
}
