using System;
using System.Collections.Generic;
using System.Text;

namespace ABFsharp
{
    public class Sweep
    {
        public readonly int index;
        public readonly int number;
        public readonly int channel;

        public readonly int length;
        public readonly double lengthSec;

        public double[] values;

        public double[] valuesCopy
        {
            get
            {
                double[] cpy = new double[values.Length];
                Array.Copy(values, 0, cpy, 0, values.Length);
                return cpy;
            }
        }

        public Sweep(AbfInfo info, int sweepIndex, int channelNumber)
        {
            index = sweepIndex;
            number = sweepIndex + 1;
            channel = channelNumber;

            length = info.sweepLengthPoints;
            lengthSec = info.sweepLengthSec;
            values = null;
        }
    }

}
