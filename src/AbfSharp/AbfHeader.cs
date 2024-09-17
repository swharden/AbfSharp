namespace AbfSharp;

public class AbfHeader(ABFFIO.AbfFileInterface abfInterface)
{
    public ABFFIO.AbfFileHeader AbfFileHeader { get; } = abfInterface.GetHeader();

    public OperationMode OperationMode => (OperationMode)AbfFileHeader.nOperationMode;
    public float SampleRate => 1e6f / AbfFileHeader.fADCSequenceInterval / AbfFileHeader.nADCNumChannels;
    public float GetFileVersion => AbfFileHeader.fFileVersionNumber;
    public int SweepCount => Math.Max(1, AbfFileHeader.lActualEpisodes);
    public int ChannelCount => AbfFileHeader.nADCNumChannels;
    public double PointsPerSweep => AbfFileHeader.lNumSamplesPerEpisode / ChannelCount;
    public double SweepLength => PointsPerSweep / SampleRate;
    public double AbfLength => SweepLength * SweepCount;
    public string ProtocolPath => AbfFileHeader.sProtocolPath;
    public string Protocol => Path.GetFileNameWithoutExtension(ProtocolPath);
    public int EpochCount => AbfFileHeader.fEpochInitLevel.Length;

    // TODO: create DAC and ADC classes
    public string[] GetDacNames() => AbfFileHeader.sDACChannelName.Select(x => x.ToString()).ToArray();
    public string[] GetDacUnits() => AbfFileHeader.sDACChannelUnits.Select(x => x.ToString()).ToArray();
    public string[] GetAdcNames() => AbfFileHeader.sADCChannelName.Select(x => x.ToString()).ToArray();
    public string[] GetAdcUnits() => AbfFileHeader.sADCUnits.Select(x => x.ToString()).ToArray();
}
