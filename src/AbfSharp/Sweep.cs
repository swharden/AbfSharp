namespace AbfSharp;

public class Sweep
{
    public double[] Values { get; private set; }
    public double SampleRate { get; }
    public double SamplePeriod { get; }
    public int SweepIndex { get; }
    public int ChannelIndex { get; }
    public double FileStartTime { get; }
    public double Duration => Values.Length / SampleRate;

    public Sweep(double[] values, double sampleRate, int sweepIndex, int channelIndex, double fileStartTime)
    {
        Values = values;
        SampleRate = sampleRate;
        SamplePeriod = 1.0 / sampleRate;
        SweepIndex = sweepIndex;
        ChannelIndex = channelIndex;
        FileStartTime = fileStartTime;
    }

    public Sweep(AbfSharp.ABF abf, int sweepIndex, int channelIndex = 0)
    {
        float[] valuesF = abf.GetSweepF(sweepIndex, channelIndex);
        double[] values = new double[valuesF.Length];
        for (int i = 0; i < valuesF.Length; i++)
        {
            values[i] = valuesF[i];
        }

        Values = values;
        SampleRate = abf.SampleRate;
        SamplePeriod = abf.SamplePeriod;
        SweepIndex = sweepIndex;
        ChannelIndex = channelIndex;
        FileStartTime = abf.SweepLength * sweepIndex;
    }
}