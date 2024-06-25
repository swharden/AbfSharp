using FluentAssertions;
using NUnit.Framework;

namespace AbfSharpTests;

class DataValues
{
    // values obtained from Python and pyABF (script in dev folder)
    [TestCase("16921011-vc-memtest-tags.abf", new double[] { -7.202148, -6.7138667, -5.737304, -4.15039, -3.4179685 })]
    [TestCase("17n16012-vc-steps.abf", new double[] { -120.23925, -119.26269, -118.164055, -119.01855, -120.11718 })]
    [TestCase("17n16016-ic-ramp.abf", new double[] { -61.431885, -61.553955, -61.401367, -61.584473, -61.431885 })]
    [TestCase("17n16016-ic-steps.abf", new double[] { -62.469482, -62.316895, -62.438965, -62.438965, -62.316895 })]
    [TestCase("18808025-memtest.abf", new double[] { -14.770507, -15.502929, -16.23535, -16.35742, -15.86914 })]
    public void Test_FirstSweep_FirstChannel_FirstFiveValues(string filename, double[] expectedFirstValues)
    {
        string abfPath = SampleData.GetAbfPath(filename);
        var abf = new AbfSharp.ABF(abfPath);
        float[] sweepValues = abf.GetSweep(0);

        for (int i = 0; i < expectedFirstValues.Length; i++)
            sweepValues[i].Should().BeApproximately((float)expectedFirstValues[i], (float)1e-3);
    }
}