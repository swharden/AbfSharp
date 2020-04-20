using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ABFsharp.Tests
{
    class Values
    {
        const string abfFolderPath = "../../../../../dev/abfs/";

        private readonly string bigAbfFilePath = System.IO.Path.Combine(abfFolderPath, "vc_drug_memtest.abf");
        private readonly string smallAbfFilePath = System.IO.Path.Combine(abfFolderPath, "18808025.abf");

        [Test]
        public void Test_FirstValue_MatchesExpected()
        {
            // tests values against those observed in ClampFit

            // TODO: paramaterize this and use values from pyABF
            // https://github.com/swharden/pyABF/blob/master/tests/test_values.py

            var abf = new ABF(smallAbfFilePath);

            var firstSweep = abf.GetSweep(0);
            Assert.AreEqual(-14.771, firstSweep.values[0], .1);

            var secondSweep = abf.GetSweep(1);
            Assert.AreEqual(-12.939, secondSweep.values[0], .1);

            var lastSweep = abf.GetSweep(-1);
            Assert.AreEqual(-18.555, lastSweep.values[0], .1);
        }
    }
}
