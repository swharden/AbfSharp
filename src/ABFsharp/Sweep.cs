using System;
using System.Collections.Generic;
using System.Text;

namespace ABFsharp
{
    public class Sweep
    {

        private readonly AbfInfo info;

        public int number;
        public int channel;

        public readonly int length;
        public readonly double lengthSec;
        public readonly double lengthMin;
        public readonly double intervalSec;
        public readonly double intervalMin;
        public readonly double[] values;
        public double[] valuesCopy
        {
            get
            {
                double[] cpy = new double[values.Length];
                Array.Copy(values, 0, cpy, 0, values.Length);
                return cpy;
            }
        }

        public Sweep(AbfInfo info)
        {
            this.info = info;
            length = info.sweepLengthPoints;
            lengthSec = info.sweepLengthSec;
            lengthMin = info.sweepLengthMin;
            intervalSec = info.sweepIntervalSec;
            intervalMin = info.sweepIntervalMin;
            values = new double[info.sweepLengthPoints];
        }
    }

}
