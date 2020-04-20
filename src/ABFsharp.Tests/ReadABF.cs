using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ABFsharp.Tests
{
    public class Tests
    {
        static string[] GetAbfPaths()
        {
            var folderPath = System.IO.Path.GetFullPath("../../../../../dev/abfs");
            return System.IO.Directory.GetFiles(folderPath, "*.abf");
        }

        static IEnumerable<TestCaseData> abfPaths
        {
            get
            {
                foreach (var abfPath in GetAbfPaths())
                    yield return new TestCaseData(abfPath);
            }
        }

        [Test]
        public void Test_AbfTestFiles_WereFound()
        {
            var paths = GetAbfPaths();
            Console.WriteLine($"Found {paths.Length} test ABF files");
            Assert.NotZero(paths.Length);
        }

        [TestCaseSource("abfPaths")]
        public void Test_AbfTestFiles_HeaderOnly(string abfPath)
        {
            var abf = new ABF(abfPath, ABF.Preload.HeaderOnly);
            Console.WriteLine(abf);
        }

        [TestCaseSource("abfPaths")]
        public void Test_AbfTestFiles_FirstSweep(string abfPath)
        {
            var abf = new ABF(abfPath, ABF.Preload.FirstSweep);
            Console.WriteLine(abf);
        }

        [TestCaseSource("abfPaths")]
        public void Test_AbfTestFiles_AllSweeps(string abfPath)
        {
            var abf = new ABF(abfPath, ABF.Preload.AllSweeps);
            Console.WriteLine(abf);
        }
    }
}