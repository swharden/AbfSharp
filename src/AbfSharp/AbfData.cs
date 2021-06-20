using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbfSharp
{
    public class AbfData
    {
        readonly int sweepCount;
        readonly int channelCount;
        readonly int sweepsInFile;
        readonly int sweepPointCount;
        List<double[]> values;
        public int sweepsInMemory { get { return values.Select(x => x != null).Count(); } }

        public AbfData(int sweepCount, int channelCount, int sweepPointCount)
        {
            this.sweepCount = sweepCount;
            this.channelCount = channelCount;
            this.sweepPointCount = sweepPointCount;
            sweepsInFile = sweepCount * channelCount;
            values = new List<double[]>(sweepCount * channelCount);
            for (int i = 0; i < sweepsInFile; i++)
                values.Add(null);
        }

        public double[] GetValues(int sweepIndex, int channelIndex)
        {
            int valueIndex = channelIndex * sweepCount + sweepIndex;
            return values[valueIndex];
        }

        public double[] GetAllValues(int channelIndex)
        {
            double[] allValues = new double[sweepPointCount * sweepCount];
            for (int i=0; i<sweepCount; i++)
            {
                var sweep = GetValues(i, channelIndex);
                var offset = i * sweepPointCount;
                Array.Copy(sweep, 0, allValues, offset, sweepPointCount);
            }
            return allValues;
        }

        public void SetValues(int sweepIndex, int channelIndex, double[] sweepValues)
        {
            int valueIndex = channelIndex * sweepCount + sweepIndex;
            values[valueIndex] = sweepValues;
        }

        public bool HasValues(int sweepIndex, int channelIndex)
        {
            int valueIndex = channelIndex * sweepCount + sweepIndex;
            return (values[valueIndex] is null) ? false : true;
        }
    }
}
