namespace AbfSharp;

public class Sweep
{
    public int Index { get; }
    public int Channel { get; }
    public double[] Values { get; }
    public double SampleRate { get; }
    public double SamplePeriod => 1 / SampleRate;
    public double LengthSec => Values.Length / SampleRate;
    public double StartTime { get; }

    public Sweep(ABF abf, int sweepIndex, int channelIndex)
    {
        Index = sweepIndex;
        Channel = channelIndex;
        SampleRate = abf.SampleRate;
        StartTime = abf.Header.SweepLength * sweepIndex;

        float[] values = abf.GetSweepF(sweepIndex, channelIndex);
        Values = new double[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            Values[i] = values[i];
        }
    }
}
