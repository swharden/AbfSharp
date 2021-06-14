using System;
using System.Collections.Generic;
using System.Text;

namespace AbfSharp
{
    public class Trace
    {
        public double[] values;
        public double sampleRate;

        public override string ToString()
        {
            if (values is null)
                return "ABF Trace (uninitialized)";
            else
                return $"ABF Trace holding {values.Length} values";
        }
    }
}
