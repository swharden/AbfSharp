using System;
using System.Collections.Generic;
using System.Text;

namespace AbfSharp
{
    public class Sweep
    {
        /// <summary>
        /// ADC values (typically voltage or current)
        /// </summary>
        public readonly double[] Values;

        [Obsolete("use GetSweepTimes() or similar", true)]
        public readonly double[] Times;

        /// <summary>
        /// Number of data points in this sweep
        /// </summary>
        public int Length => Values.Length;

        /// <summary>
        /// Total length of this sweep (seconds)
        /// </summary>
        public double LengthSeconds => SampleRate / Values.Length;

        /// <summary>
        /// Time between the the start of the ABF and the start of this sweep (seconds)
        /// </summary>
        public readonly double TimeOffset;

        /// <summary>
        /// Absolute date and time of the start of this sweep
        /// </summary>
        public DateTime DateTime => throw new NotImplementedException();

        /// <summary>
        /// Values per second (Hz)
        /// </summary>
        public readonly double SampleRate;

        /// <summary>
        /// Time between each value (seconds)
        /// </summary>
        public double SamplePeriod => 1.0 / SampleRate;

        /// <summary>
        /// Time between each value (milliseconds)
        /// </summary>
        public double SamplePeriodMilliseconds => 1000.0 / SampleRate;

        /// <summary>
        /// Name of the ADC (measurement) channel
        /// </summary>
        public string AdcLabel => throw new NotImplementedException();

        /// <summary>
        /// Units of the ADC (measurement) channel
        /// </summary>
        public string AdcUnits => throw new NotImplementedException();

        /// <summary>
        /// Name of the DAC (measurement) channel
        /// </summary>
        public string DacLabel => throw new NotImplementedException();

        /// <summary>
        /// Units of the DAC (measurement) channel
        /// </summary>
        public string DacUnits => throw new NotImplementedException();

        public Sweep(double[] values, double sampleRate, double offset)
        {
            Values = values;
            SampleRate = sampleRate;
            TimeOffset = offset;
        }

        /// <summary>
        /// Return array of times (same length as Values) starting at zero
        /// </summary>
        public double[] GetSweepTimes()
        {
            double[] times = new double[Values.Length];
            for (int i = 0; i < Values.Length; i++)
                times[i] = i / SampleRate;
            return times;
        }

        /// <summary>
        /// Return array of times (same length as Values) relative to the start of the ABF
        /// </summary>
        public double[] GetAbfTimes()
        {
            double[] times = new double[Values.Length];
            for (int i = 0; i < Values.Length; i++)
                times[i] = i / SampleRate + TimeOffset;
            return times;
        }

        public override string ToString() =>
            $"ABF Sweep holding {Values.Length:N0} values at {SampleRate:N0} Hz ({LengthSeconds} seconds)";
    }
}
