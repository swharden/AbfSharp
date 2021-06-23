using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfSharpTests
{
    /// <summary>
    /// These tests inspect sweep data using the raw ABF loader and compare it to sweeps
    /// loaded using the ABFFIO to serve as a source of truth.
    /// Values are accurate to 1e-3 (errors are due to floating-point multiplication errors).
    /// </summary>
    class DataValues
    {
        private AbfSharp.ABFFIO.ABF[] OfficialABFs;

        [OneTimeSetUp()]
        public void LoadOfficialABFs() =>
            OfficialABFs = SampleData.GetAllAbfPaths().Select(x => new AbfSharp.ABFFIO.ABF(x)).ToArray();

        [Test]
        public void Test_MatchesOfficial_DataScalingVariables()
        {
            foreach (AbfSharp.ABFFIO.ABF official in OfficialABFs)
            {
                Console.WriteLine($"v={official.Header.fFileVersionNumber:0.0} {System.IO.Path.GetFileName(official.FilePath)}");
                var raw = new AbfSharp.ABF(official.FilePath);

                for (int i = 0; i < 8; i++)
                {
                    Assert.AreEqual(official.Header.fInstrumentOffset[i], raw.Header.fInstrumentOffset[i]);
                    Assert.AreEqual(official.Header.fSignalOffset[i], raw.Header.fSignalOffset[i]);
                    Assert.AreEqual(official.Header.fInstrumentScaleFactor[i], raw.Header.fInstrumentScaleFactor[i]);
                    Assert.AreEqual(official.Header.fSignalGain[i], raw.Header.fSignalGain[i]);
                    Assert.AreEqual(official.Header.fADCProgrammableGain[i], raw.Header.fADCProgrammableGain[i]);
                    Assert.AreEqual(official.Header.lADCResolution, raw.Header.lADCResolution);
                    Assert.AreEqual(official.Header.fADCRange, raw.Header.fADCRange);
                    Assert.AreEqual(official.Header.nTelegraphEnable[i], raw.Header.nTelegraphEnable[i]);
                    Assert.AreEqual(official.Header.fTelegraphAdditGain[i], raw.Header.fTelegraphAdditGain[i]);
                }
            }
        }

        [Test]
        public void Test_MatchesOfficial_SweepValues()
        {
            foreach (AbfSharp.ABFFIO.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.ABF(official.FilePath, preloadData: true);

                // Don't compare ABF1 GapFree ABFs because ABFFIO freaks out
                if (raw.Header.OperationMode == AbfSharp.HeaderData.OperationMode.GapFree && raw.Header.FileVersionNumber < 2)
                    continue;

                // Don't compare EventDriven sweeps because ABFFIO returns fixed-length sweeps
                // whereas I simply return each event's data as a sweep.
                if (raw.Header.OperationMode == AbfSharp.HeaderData.OperationMode.EventDriven)
                    continue;

                Console.WriteLine(raw);
                int channelIndex = raw.Header.ChannelCount - 1;
                int sweepIndex = raw.Header.SweepCount - 1;

                double[] officialValues = official.GetSweep(sweepIndex, channelIndex);
                Assert.IsNotNull(officialValues);
                Assert.IsNotEmpty(officialValues);

                float[] rawValues = raw.GetSweep(sweepIndex, channelIndex);
                Assert.IsNotNull(rawValues);
                Assert.IsNotEmpty(rawValues);

                if (raw.Header.OperationMode != AbfSharp.HeaderData.OperationMode.GapFree)
                    Assert.AreEqual(officialValues.Length, rawValues.Length);

                for (int i = 0; i < 10; i++)
                    Assert.AreEqual(officialValues[i], rawValues[i], 1e-3);
            }
        }

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
                Assert.AreEqual(expectedFirstValues[i], sweepValues[i], 1e-3);
        }
    }
}
