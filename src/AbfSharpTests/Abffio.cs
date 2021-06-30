﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AbfSharpTests
{
    class Abffio
    {
        private static string GetMD5Hash(string filePath)
        {
            var md5 = MD5.Create();
            using var stream = File.OpenRead(filePath);
            return string.Join("", md5.ComputeHash(stream).Select(x => x.ToString("x2")));
        }

        [Test]
        public void Test_ABFFIO_DllHash()
        {
            // hash generated from Windows command line: certutil -hashfile ABFFIO.dll MD5
            // tested on ABFFIO.dll in the ABF FileSupportPack distributed with pCLAMP11.2
            Assert.AreEqual("08ddd0757f32ed733f958526693ccd66", GetMD5Hash("ABFFIO.dll"));
        }

        private static double[] ToDoubles(float[] f)
        {
            double[] d = new double[f.Length];
            for (int i = 0; i < f.Length; i++)
                d[i] = f[i];
            return d;
        }

        [Test]
        public void Test_AllABfs_Load()
        {
            foreach (string abfPath in SampleData.GetAllAbfPaths())
            {
                var abf = new AbfSharp.ABFFIO.ABF(abfPath);
                Console.WriteLine($"\n{abf}");
                if (abf.Tags.Count > 0)
                    Console.WriteLine($"TAGS: {abf.Tags}");

                float[] adc = abf.GetSweep(0);
                Console.WriteLine("SWEEP: " + string.Join(", ", adc.Take(10).Select(x => x.ToString())));

                float[] dac = abf.GetStimulusWaveform(0);
                Console.WriteLine("STIM: " + string.Join(", ", dac.Take(10).Select(x => x.ToString())));

                if (abf.OperationMode != AbfSharp.OperationMode.EventDriven)
                    Assert.AreEqual(adc.Length, dac.Length);

                bool plotToo = false;
                if (plotToo)
                {
                    string saveAs = Path.Combine(Path.GetDirectoryName(abf.FilePath), Path.GetFileNameWithoutExtension(abf.FilePath) + ".png");
                    ScottPlot.MultiPlot mp = new(600, 400, 2, 1);
                    mp.subplots[0].AddSignal(ToDoubles(adc));
                    mp.subplots[1].AddSignal(ToDoubles(dac));
                    mp.SaveFig(saveAs);
                    Console.WriteLine($"SAVED: {saveAs}");
                }
            }
        }

        [Test]
        public void Test_NonAbf_ThrowsOnLoad()
        {
            string nonAbfPath = Path.GetFullPath("ABFFIO.DLL");
            Assert.Throws<InvalidOperationException>(() => { var abf = new AbfSharp.ABFFIO.ABF(nonAbfPath); });
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
        public void Test_Tag_Summaries(string abfFileName, string expectedCommentSummary)
        {
            AbfSharp.ABFFIO.ABF abf = new(SampleData.GetAbfPath(abfFileName));
            Assert.AreEqual(expectedCommentSummary, abf.Tags.ToString());
        }

        [TestCase("File_axon_4.abf", "DEF0C2D9-9817-42F7-B139-526A4AA9397A")]
        public void Test_Guid_MatchesExpected(string abfFileName, string expectedGuid)
        {
            AbfSharp.ABFFIO.ABF abf = new(SampleData.GetAbfPath(abfFileName));
            Assert.AreEqual(expectedGuid, abf.Header.FileGUID.ToString().ToUpper());
        }
    }
}
