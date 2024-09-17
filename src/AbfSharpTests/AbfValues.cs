using ScottPlot;

namespace AbfSharpTests;

class AbfValues
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
        float[] sweepValues = abf.GetSweepF(0);

        for (int i = 0; i < expectedFirstValues.Length; i++)
            sweepValues[i].Should().BeApproximately((float)expectedFirstValues[i], (float)1e-3);
    }

    [TestCase("File_axon_5.abf")]
    public void Test_Plot_SignalAndStimulus(string abfFilename)
    {
        string abfFilePath = SampleData.GetAbfPath(abfFilename);
        string saveFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, "test_figures");
        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        var abf = new AbfSharp.ABF(abfFilePath);
        Console.WriteLine($"\n{abf}");

        ScottPlot.Plot plot1 = new();
        ScottPlot.Plot plot2 = new();
        for (int i = 0; i < abf.SweepCount; i++)
        {
            float[] adc = abf.GetSweepF(i);
            plot1.Add.Signal(adc);

            float[] dac = abf.GetStimulusWaveform(i);
            plot2.Add.Signal(dac);
        }

        string saveAsBase = Path.Combine(saveFolder, Path.GetFileNameWithoutExtension(abf.FilePath));
        plot1.SavePng(saveAsBase + "_ADC.png", 600, 400);
        plot2.SavePng(saveAsBase + "_DAC.png", 600, 400);
    }

    [Test]
    public void Test_GapFree_MultiChannel()
    {
        string abfFilePath = SampleData.GetAbfPath("14o08011_ic_pair.abf");
        ABF abf = new(abfFilePath);

        ScottPlot.Plot plot = new();
        for (int i = 0; i < abf.SweepCount; i++)
        {
            Sweep ch1 = abf.GetSweep(i, 0);
            var sig1 = plot.Add.Signal(ch1.Values, ch1.SamplePeriod);
            sig1.Data.XOffset = ch1.FileStartTime;
            sig1.Color = Colors.C0;

            Sweep ch2 = abf.GetSweep(i, 1);
            var sig2 = plot.Add.Signal(ch2.Values, ch2.SamplePeriod);
            sig2.Data.XOffset = ch2.FileStartTime;
            sig2.Color = Colors.C1;
        }

        plot.SaveTestImage(600, 400);
    }
}