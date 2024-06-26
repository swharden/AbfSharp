namespace AbfSharpTests;

class AbfIO
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
            if (abf.Tags.Length > 0)
                Console.WriteLine($"TAGS: {abf.TagSummaries}");

            float[] adc = abf.GetSweep(0);
            Console.WriteLine("SWEEP: " + string.Join(", ", adc.Take(10).Select(x => x.ToString())));

            float[] dac = abf.GetStimulusWaveform(0);
            Console.WriteLine("STIM: " + string.Join(", ", dac.Take(10).Select(x => x.ToString())));

            if (abf.Header.OperationMode == OperationMode.EventDriven)
                adc.Should().HaveSameCount(dac);
        }
    }

    [Test]
    public void Test_Version_DLL()
    {
        AbfSharp.Version.DllVersion.Should().Be("2.2.0.1");
    }
}
