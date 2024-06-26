namespace AbfSharp;

public class AbfTagManager
{
    public Tag[] Tags { get; }

    public AbfTagManager(ABFFIO.AbfFileInterface abfInterface, ABFFIO.AbfFileHeader header)
    {
        ABFFIO.TagStruct[] tagStructs = abfInterface.ReadTags();
        float sampleRate = 1e6f / header.fADCSequenceInterval / header.nADCNumChannels;
        float samplePeriod = 1.0f / sampleRate;
        float tagTimeMultiple = (header.fSynchTimeUnit == 0)
        ? samplePeriod / header.nADCNumChannels
            : header.fSynchTimeUnit / 1e6f;
        Tags = tagStructs.Select(x => new Tag(x, tagTimeMultiple)).ToArray();
    }
}
