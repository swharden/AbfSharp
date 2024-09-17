namespace AbfSharp;

public class Epoch
{
    public int EpochIndex { get; }
    public string EpochName => ((char)('A' + (char)EpochIndex)).ToString();
    public int EpochTypeCode { get; }
    public EpochType EpochType { get; }
    public double Level { get; }
    public double LevelDelta { get; }
    public int IndexFirst { get; }
    public int IndexLast { get; }
    public int PulsePeriodSamples { get; }
    public int PulseWidthSamples { get; }
    public double SamplePeriod { get; }
    public double StartTime => SamplePeriod * IndexFirst;
    public double EndTime => SamplePeriod * IndexLast;
    public double Duration => EndTime - StartTime;
    private int SweepCount { get; }

    public Epoch(AbfSharp.ABF abf, int epochIndex)
    {
        EpochIndex = epochIndex;

        int totalRecordingLength = abf.Header.AbfFileHeader.lActualAcqLength;
        SweepCount = abf.Header.AbfFileHeader.lActualEpisodes;
        int sweepLength = totalRecordingLength / SweepCount;
        int firstEpochIndex = sweepLength / 64;

        int previousEpochDurations = Enumerable
            .Range(0, epochIndex)
            .Select(x => abf.Header.AbfFileHeader.lEpochInitDuration[x])
            .Sum();

        int i1 = firstEpochIndex + previousEpochDurations;
        int i2 = i1 + abf.Header.AbfFileHeader.lEpochInitDuration[epochIndex];

        IndexFirst = i1;
        IndexLast = i2;
        Level = abf.Header.AbfFileHeader.fEpochInitLevel[epochIndex];
        LevelDelta = abf.Header.AbfFileHeader.fEpochLevelInc[EpochIndex];

        SamplePeriod = abf.SamplePeriod;
        EpochTypeCode = abf.Header.AbfFileHeader.nEpochType[epochIndex];
        EpochType = (EpochType)EpochTypeCode;

        PulsePeriodSamples = abf.Header.AbfFileHeader.lEpochPulsePeriod[epochIndex];
        PulseWidthSamples = abf.Header.AbfFileHeader.lEpochPulseWidth[epochIndex];
    }

    public double[] GetLevelsBySweep()
    {
        return Enumerable.Range(0, SweepCount).Select(x => Level + LevelDelta * x).ToArray();
    }
}