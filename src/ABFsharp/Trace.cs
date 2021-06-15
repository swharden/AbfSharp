using System;
using System.Collections.Generic;
using System.Text;

namespace AbfSharp
{
    public class Trace
    {
        public double[] Values;
        public double SampleRate;

        public double[] Times
        {
            get
            {
                double[] t = new double[Values.Length];
                for (int i = 0; i < t.Length; i++)
                    t[i] = i / SampleRate;
                return t;
            }
        }

        public override string ToString()
        {
            if (Values is null)
                return "ABF Trace (uninitialized)";
            else
                return $"ABF Trace holding {Values.Length} values";
        }
    }
}
