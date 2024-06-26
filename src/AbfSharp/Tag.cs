namespace AbfSharp;

// TODO: make a struct
public class Tag
{
    public readonly long lTagTime;
    public readonly string sComment;
    public readonly short nTagType;
    public readonly short nVoiceTagNumber;

    /// <summary>
    /// Describes what type of tag this is
    /// </summary>
    public TagType Type => (TagType)nTagType;

    /// <summary>
    /// Tag time (seconds)
    /// </summary>
    public readonly float Time;

    /// <summary>
    /// Tag time (minutes)
    /// </summary>
    public float TimeMin => Time / 60.0f;

    /// <summary>
    /// Tag comment (or empty string if no comment)
    /// </summary>
    public string Comment => sComment.Trim();

    /// <summary>
    /// Description
    /// </summary>
    public string Summary
    {
        get
        {
            string description = Type == TagType.Comment ? $"'{Comment}'" : Type.ToString();
            return $"{description} @ {TimeMin:0.00} min";
        }
    }

    public Tag(ABFFIO.TagStruct tagStruct, float tagTimeMultiple)
    {
        lTagTime = tagStruct.lTagTime;
        sComment = tagStruct.sComment;
        nTagType = tagStruct.nTagType;
        nVoiceTagNumber = tagStruct.nVoiceTagNumber;
        Time = lTagTime * tagTimeMultiple;
    }

    public Tag(long lTagTime, string sComment, short nTagType, short nVoiceTagNumber, float tagTimeMult)
    {
        this.lTagTime = lTagTime;
        this.sComment = sComment;
        this.nTagType = nTagType;
        this.nVoiceTagNumber = nVoiceTagNumber;
        Time = lTagTime * tagTimeMult;
    }
}
