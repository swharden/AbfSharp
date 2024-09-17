namespace AbfSharp;

public static class SweepExtensions
{
    public static Sweep WithValues(this Sweep sweep, double[] newValues)
    {
        return new Sweep(newValues, sweep.SampleRate, sweep.SweepIndex, sweep.ChannelIndex, sweep.FileStartTime);
    }

    public static Sweep WithStartTime(this Sweep sweep, double newStartTime)
    {
        return new Sweep(sweep.Values, sweep.SampleRate, sweep.SweepIndex, sweep.ChannelIndex, newStartTime);
    }

    public static Sweep WithSampleRate(this Sweep sweep, double newSampleRate)
    {
        return new Sweep(sweep.Values, newSampleRate, sweep.SweepIndex, sweep.ChannelIndex, sweep.FileStartTime);
    }

    public static Sweep SubSweepByIndex(this Sweep sweep, int i1, int i2)
    {
        double[] newValues = sweep.Values.Skip(i1).Take(i2 - i1).ToArray();
        double newStartTime = sweep.FileStartTime + i1 * sweep.SamplePeriod;
        return sweep.WithValues(newValues).WithStartTime(newStartTime);
    }

    public static Sweep Derivative(this Sweep sweep, int delta = 1)
    {
        double[] newValues = new double[sweep.Values.Length - delta];
        for (int i = 0; i < newValues.Length; i++)
        {
            newValues[i] = sweep.Values[i + delta] - sweep.Values[i];
        }

        return sweep.WithValues(newValues);
    }

    public static Sweep Rectified(this Sweep sweep)
    {
        double[] newValues = new double[sweep.Values.Length];

        for (int i = 0; i < newValues.Length; i++)
        {
            newValues[i] = Math.Abs(sweep.Values[i]);
        }

        return sweep.WithValues(newValues);
    }

    public static Sweep Smooth(this Sweep sweep, TimeSpan timeSpan)
    {
        int pointCount = (int)(timeSpan.TotalMilliseconds / 1000.0 / sweep.SamplePeriod);
        return sweep.Smooth(pointCount);
    }

    public static Sweep Smooth(this Sweep sweep, int pointCount)
    {
        double[] smooth = new double[sweep.Values.Length];

        // smooth from left to right
        double runningSum = 0;
        int pointsInSum = 0;
        for (int i = 0; i < smooth.Length; i++)
        {
            runningSum += sweep.Values[i];

            if (pointsInSum < pointCount)
            {
                pointsInSum++;
                smooth[i] += runningSum / pointsInSum;
                continue;
            }

            runningSum -= sweep.Values[i - pointCount];
            smooth[i] += runningSum / pointCount;
        }

        // smooth from right to left
        runningSum = 0;
        pointsInSum = 0;
        for (int i = smooth.Length - 1; i >= 0; i--)
        {
            runningSum += sweep.Values[i];

            if (pointsInSum < pointCount)
            {
                pointsInSum++;
                smooth[i] += runningSum / pointsInSum;
                continue;
            }

            runningSum -= sweep.Values[i + pointCount];
            smooth[i] += runningSum / pointCount;
        }

        // average the two directions
        for (int i = 0; i < smooth.Length; i++)
        {
            smooth[i] /= 2;
        }

        return sweep.WithValues(smooth);
    }

    public static Sweep Detrend(this Sweep sweep, int pointCount)
    {
        Sweep trend = sweep.Smooth(pointCount);

        double[] values2 = new double[sweep.Values.Length];
        for (int i = 0; i < values2.Length; i++)
        {
            values2[i] = sweep.Values[i] - trend.Values[i];
        }

        return sweep.WithValues(values2);
    }

    public static Sweep Decimate(this Sweep sweep, int count)
    {
        int length = sweep.Values.Length / count;
        double[] values = new double[length];
        for (int i = 0; i < length; i++)
        {
            values[i] = sweep.Values[i * count];
        }

        return sweep.WithValues(values).WithSampleRate(sweep.SampleRate / count);
    }

    public static Sweep SubTraceByIndex(this Sweep sweep, int i1, int i2)
    {
        int length = i2 - i1;
        double[] values = new double[length];
        Array.Copy(sweep.Values, i1, values, 0, length);
        return sweep.WithValues(values);
    }

    public static Sweep SubTraceByFraction(this Sweep sweep, double frac1, double frac2)
    {
        int i1 = (int)Clamp((int)(frac1 * sweep.Values.Length), 0, sweep.Values.Length - 1);
        int i2 = (int)Clamp((int)(frac2 * sweep.Values.Length), 0, sweep.Values.Length - 1);
        return SubTraceByIndex(sweep, i1, i2);
    }

    private static double Clamp(double value, double min, double max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    public static Sweep SubTraceByEpoch(this Sweep sweep, Epoch epoch)
    {
        return SubTraceByIndex(sweep, epoch.IndexFirst, epoch.IndexLast);
    }

    public static void SubtractInPlace(this Sweep sweep, double value)
    {
        for (int i = 0; i < sweep.Values.Length; i++)
        {
            sweep.Values[i] -= value;
        }
    }

    public static int GetMaximumIndex(this Sweep sweep)
    {
        int maxIndex = 0;
        double maxValue = double.NegativeInfinity;
        for (int i = 0; i < sweep.Values.Length; i++)
        {
            if (sweep.Values[i] > maxValue)
            {
                maxValue = sweep.Values[i];
                maxIndex = i;
            }
        }
        return maxIndex;
    }

    public static int GetFirstIndexBelow(this Sweep sweep, double target, int firstIndex = 0)
    {
        for (int i = firstIndex; i < sweep.Values.Length; i++)
        {
            if (sweep.Values[i] < target)
                return i;
        };

        throw new InvalidOperationException("values never went below target");
    }
}
