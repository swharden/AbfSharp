using AbfSharp;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace AbfSharpTests;

class Abffio
{
    [Test]
    public void Test_ABFFIO_DllHash()
    {
        // hash generated from command prompt: `certutil -hashfile ABFFIO.dll MD5`
        // ABFFIO.dll obtained from the ABF FileSupportPack distributed online as part of pCLAMP 11.2
        string expectedHash = "08ddd0757f32ed733f958526693ccd66";

        MD5 md5 = MD5.Create();
        using var stream = File.OpenRead("ABFFIO.dll");
        string dllHash = string.Join("", md5.ComputeHash(stream).Select(x => x.ToString("x2")));
        dllHash.Should().Be(expectedHash);
    }

    [Test]
    public void Test_AllABfs_Load()
    {
        foreach (string abfPath in SampleData.GetAllAbfPaths())
        {
            var abf = new ABF(abfPath, preloadSweepData: false);
            Console.WriteLine($"\n{abf}");
            if (abf.Tags.Count > 0)
                Console.WriteLine($"TAGS: {abf.Tags}");

            float[] adc = abf.GetSweep(0);
            Console.WriteLine("SWEEP: " + string.Join(", ", adc.Take(10).Select(x => x.ToString())));

            float[] dac = abf.GetStimulusWaveform(0);
            Console.WriteLine("STIM: " + string.Join(", ", dac.Take(10).Select(x => x.ToString())));

            if (abf.OperationMode != OperationMode.EventDriven)
                adc.Should().HaveSameCount(dac);
        }
    }

    [TestCase("File_axon_5.abf")]
    public void Test_Plot_SignalAndStimulus(string abfFilename)
    {
        string abfFilePath = SampleData.GetAbfPath(abfFilename);
        string saveFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, "test_figures");
        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        var abf = new ABF(abfFilePath);
        Console.WriteLine($"\n{abf}");

        ScottPlot.Plot plot1 = new();
        ScottPlot.Plot plot2 = new();
        for (int i = 0; i < abf.SweepCount; i++)
        {
            float[] adc = abf.GetSweep(i);
            plot1.Add.Signal(adc);

            float[] dac = abf.GetStimulusWaveform(i);
            plot2.Add.Signal(dac);
        }

        string saveAsBase = Path.Combine(saveFolder, Path.GetFileNameWithoutExtension(abf.FilePath));
        plot1.SavePng(saveAsBase + "_ADC.png", 600, 400);
        plot2.SavePng(saveAsBase + "_DAC.png", 600, 400);
    }

    [TestCase("16921011-vc-memtest-tags.abf", "'+TGOT' @ 6.66 min, '-TGOT' @ 8.68 min")]
    [TestCase("16d05007_vc_tags.abf", "'+TGOT' @ 2.90 min, '-TGOT' @ 4.91 min")]
    [TestCase("19122043.abf", "'C9, L3,  RMP -66.8 mv' @ 0.03 min")]
    [TestCase("2018_11_16_sh_0006.abf", "'+drug at 3min' @ 3.01 min")]
    [TestCase("2020_07_29_0062.abf", "'Digital Outputs => 00000001' @ 0.03 min")]
    [TestCase("abf1_with_tags.abf", "'APV+CGP+DNQX+ON@6' @ 6.25 min")]
    [TestCase("ch121219_1_0001.abf", "External @ 0.26 min, External @ 0.59 min, External @ 0.92 min, External @ 1.26 min, External @ 1.59 min, External @ 1.92 min, External @ 2.26 min, External @ 2.59 min, External @ 2.92 min, External @ 3.26 min, External @ 3.59 min, External @ 3.92 min, External @ 4.26 min, External @ 4.59 min, External @ 4.92 min, External @ 5.26 min")]
    [TestCase("File_axon_2.abf", "'Clampex start acquisition' @ 0.45 min, 4 @ 7.11 min, 'Clampex end (1)' @ 7.11 min, 'Clampex start acquisition' @ 10.42 min")]
    [TestCase("File_axon_4.abf", "'drogue on' @ 2.44 min")]
    [TestCase("File_axon_3.abf", "no tags")]
    public void Test_Tag_MatchesKnown(string abfFileName, string expectedCommentSummary)
    {
        ABF abf = new(SampleData.GetAbfPath(abfFileName));
        abf.Tags.ToString().Should().Be(expectedCommentSummary);
    }

    [TestCase("File_axon_4.abf", "DEF0C2D9-9817-42F7-B139-526A4AA9397A")]
    public void Test_Guid_MatchesKnown(string abfFileName, string expectedGuid)
    {
        ABF abf = new(SampleData.GetAbfPath(abfFileName));
        abf.Header.FileGUID.ToString().ToUpper().Should().Be(expectedGuid);
    }
}
