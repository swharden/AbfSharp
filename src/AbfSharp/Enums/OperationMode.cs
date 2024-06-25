namespace AbfSharp;

public enum OperationMode
{
    /// <summary>
    /// Variable length sweeps initiated when a threshold-crossing event is detected. 
    /// There is no stimulus waveform associated with these two operation modes.
    /// </summary>
    EventDriven,

    /// <summary>
    /// Loss free (Same as Event-driven, fixed length)
    /// </summary>
    Oscilloscope,

    /// <summary>
    /// Gap-free ABF files contain a single sweep of up to 4 GB of multiplexed data. 
    /// A uniform sampling interval is used throughout. There is no stimulus waveform
    /// associated with gap-free data. Gap-free mode is usually used for the continuous
    /// acquisition of data in which there is a fairly uniform activity over time.
    /// </summary>
    GapFree,

    /// <summary>
    /// I'm unsure what this is typically used for...
    /// </summary>
    OscilloscopeHighSpeed,

    /// <summary>
    /// Fixed-length sweeps optionally using the stimulus waveform (Clampex only)
    /// </summary>
    Episodic,
}
