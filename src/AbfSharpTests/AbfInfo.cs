namespace AbfSharpTests;

public class AbfInfo
{
    [Test]
    public void Test_AbfFileHeader()
    {
        foreach (string abfFilePath in SampleData.GetAllAbfPaths())
        {
            AbfSharp.Reports.AbfHeaderReport report = new(abfFilePath);

            string abfFolder = System.IO.Path.GetDirectoryName(abfFilePath);
            string abfID = System.IO.Path.GetFileNameWithoutExtension(abfFilePath);
            string mdPath = System.IO.Path.Combine(abfFolder, $"{abfID}.md");
            string htmlPath = System.IO.Path.Combine(abfFolder, $"{abfID}.html");
            System.IO.File.WriteAllText(mdPath, report.GetMarkdown());
            System.IO.File.WriteAllText(htmlPath, report.GetHtml());

            Console.WriteLine(mdPath);
        }
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
    [TestCase("File_axon_3.abf", "")]
    public void Test_Tag_MatchesKnown(string abfFileName, string expectedCommentSummary)
    {
        AbfSharp.ABF abf = new(SampleData.GetAbfPath(abfFileName));
        abf.TagSummaries.Should().Be(expectedCommentSummary);
    }

    [TestCase("File_axon_4.abf", "DEF0C2D9-9817-42F7-B139-526A4AA9397A")]
    public void Test_Guid_MatchesKnown(string abfFileName, string expectedGuid)
    {
        AbfSharp.ABF abf = new(SampleData.GetAbfPath(abfFileName));
        abf.Header.AbfFileHeader.FileGUID.ToString().ToUpper().Should().Be(expectedGuid);
    }

    [TestCase("14o08011_ic_pair.abf", 10_000)]
    [TestCase("17n16016-ic-steps.abf", 20_000)]
    [TestCase("pclamp11_4ch.abf", 20_000)]
    [TestCase("File_axon_6.abf", 20_000)]
    public void Test_SampleRate_MatchesKnown(string abfFileName, int expectedSampleRate)
    {
        AbfSharp.ABF abf = new(SampleData.GetAbfPath(abfFileName));
        abf.SampleRate.Should().Be(expectedSampleRate);
    }

    [Test]
    public void Test_SweepLength()
    {
        foreach (string abfFilePath in SampleData.GetAllAbfPaths())
        {
            AbfSharp.ABF abf = new(abfFilePath);

            // ignore ABFs with variable length sweeps
            if (abfFilePath.Contains("2020_06_16_0000")) continue;

            Sweep sw = abf.GetSweep(0);
            int expectedLength = (int)Math.Round(abf.SweepLength * abf.SampleRate);
            if (sw.Values.Length != expectedLength)
            {
                Assert.Fail($"actual length {sw.Values.Length} but expected {expectedLength} {abfFilePath}");
            }
        }
    }
}
