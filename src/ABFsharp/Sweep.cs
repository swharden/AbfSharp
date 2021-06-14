using System;
using System.Collections.Generic;
using System.Text;

namespace AbfSharp
{
    [Obsolete("dont use the sweep class", false)]
    public class Sweep
    {
        public readonly int Index;
        public readonly int Number;
        public readonly int Channel;

        public readonly int LengthPoints;
        public readonly double LengthSec;

        public readonly string AdcUnits;
        public readonly string AdcLabel;
        public readonly string DacUnits;
        public readonly string DacLabel;

        public double[] values;

        private readonly AbfHeader info;

        public double[] valuesCopy
        {
            get
            {
                double[] cpy = new double[values.Length];
                Array.Copy(values, 0, cpy, 0, values.Length);
                return cpy;
            }
        }

        public Sweep(AbfHeader info, int sweepIndex, int channelNumber)
        {
            this.info = info;

            Index = sweepIndex;
            Number = sweepIndex + 1;
            Channel = channelNumber;

            AdcUnits = "y";
            AdcLabel = $"ADC ({AdcUnits})";
            DacUnits = "x";
            DacLabel = $"DAC ({AdcUnits})";

            LengthPoints = info.sweepLengthPoints;
            LengthSec = info.sweepLengthSec;
            values = null;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append($"{info.fileName} sweep {Number} of {info.sweepCount} ({LengthPoints} samples over {LengthSec} sec)");
            return s.ToString();
        }
    }

}
