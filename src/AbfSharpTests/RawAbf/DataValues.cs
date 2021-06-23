using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfSharpTests.RawAbf
{
    class DataValues
    {
        private AbfSharp.ABF[] OfficialABFs;

        [OneTimeSetUp()]
        public void LoadOfficialABFs()
        {
            OfficialABFs = SampleData.GetAllAbfPaths().Select(x => new AbfSharp.ABF(x)).ToArray();
        }

        [Test]
        public void Test_MatchesOfficial_DataScalingVariables()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                Console.WriteLine($"v={official.Header.HeaderStruct.fFileVersionNumber:0.0} {System.IO.Path.GetFileName(official.FilePath)}");
                var raw = new AbfSharp.RawABF(official.FilePath);

                for (int i = 0; i < 8; i++)
                {
                    Assert.AreEqual(official.Header.HeaderStruct.fInstrumentOffset[i], raw.Header.fInstrumentOffset[i]);
                    Assert.AreEqual(official.Header.HeaderStruct.fSignalOffset[i], raw.Header.fSignalOffset[i]);
                    Assert.AreEqual(official.Header.HeaderStruct.fInstrumentScaleFactor[i], raw.Header.fInstrumentScaleFactor[i]);
                    Assert.AreEqual(official.Header.HeaderStruct.fSignalGain[i], raw.Header.fSignalGain[i]);
                    Assert.AreEqual(official.Header.HeaderStruct.fADCProgrammableGain[i], raw.Header.fADCProgrammableGain[i]);
                    Assert.AreEqual(official.Header.HeaderStruct.lADCResolution, raw.Header.lADCResolution);
                    Assert.AreEqual(official.Header.HeaderStruct.fADCRange, raw.Header.fADCRange);
                    Assert.AreEqual(official.Header.HeaderStruct.nTelegraphEnable[i], raw.Header.nTelegraphEnable[i]);
                    Assert.AreEqual(official.Header.HeaderStruct.fTelegraphAdditGain[i], raw.Header.fTelegraphAdditGain[i]);
                }
            }
        }

        [Ignore("takes too much memory")]
        [Test]
        public void Test_MatchesOfficial_SweepLength()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.FilePath);

                Console.WriteLine(raw);

                double[] officialValues = official.GetSweep(0, 0).Values;
                Assert.IsNotNull(officialValues);
                Assert.IsNotEmpty(officialValues);

                float[] rawValues = raw.GetSweep(0, 0);
                Assert.IsNotNull(rawValues);
                Assert.IsNotEmpty(rawValues);

                if (raw.Header.OperationMode == AbfSharp.HeaderData.OperationMode.Episodic)
                {
                    Assert.AreEqual(officialValues.Length, rawValues.Length);
                }
                else if (raw.Header.OperationMode == AbfSharp.HeaderData.OperationMode.EventDriven)
                {
                    // skip this because the official reader returns the full file as one sweep
                    // but I think it makes more sense to return events as sweeps
                    continue;
                }
                else
                {
                    Assert.AreEqual(raw.Header.lActualAcqLength / raw.Header.nADCNumChannels, rawValues.Length);
                }
            }
        }

        [Test]
        public void Test_MatchesOfficial_SweepValues()
        {
            foreach (AbfSharp.ABF official in OfficialABFs)
            {
                var raw = new AbfSharp.RawABF(official.FilePath, preloadData: true);
                if (raw.Header.FileVersionNumber < 2)
                    continue;
                if (raw.Header.OperationMode == AbfSharp.HeaderData.OperationMode.GapFree)
                    continue;
                if (raw.Header.OperationMode == AbfSharp.HeaderData.OperationMode.EventDriven)
                    continue;
                if (raw.Path.Contains("_modified"))
                    continue;

                Console.WriteLine(raw);
                int channelIndex = raw.Header.ChannelCount - 1;
                int sweepIndex = raw.Header.SweepCount - 1;

                double[] officialValues = official.GetSweep(sweepIndex, channelIndex).Values;
                Assert.IsNotNull(officialValues);
                Assert.IsNotEmpty(officialValues);

                float[] rawValues = raw.GetSweep(sweepIndex, channelIndex);
                Assert.IsNotNull(rawValues);
                Assert.IsNotEmpty(rawValues);

                for (int i = 0; i < 10; i++)
                {
                    Assert.AreEqual(officialValues[i], rawValues[i], 1e-3);
                }
            }
        }
    }
}
