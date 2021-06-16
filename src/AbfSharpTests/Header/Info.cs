using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AbfSharpTests.Header
{
    class Info
    {
        [TestCase("16921011-vc-memtest-tags.abf", "2016-09-21 14:43:46.434000")]
        [TestCase("17n16012-vc-steps.abf", "2017-11-16 14:05:01.627000")]
        [TestCase("17n16016-ic-ramp.abf", "2017-11-16 14:07:11.016000")]
        [TestCase("17n16016-ic-steps.abf", "2017-11-16 14:08:10.748000")]
        [TestCase("18808025-memtest.abf", "2018-08-08 13:49:04.826000")]
        public void Test_Creation_DateTime(string filename, string expectedDateTimeString)
        {
            var abf = new AbfSharp.ABF(SampleData.GetAbfPath(filename));
            Console.WriteLine($"ABF created: {abf.Header.StartDateTime}");

            DateTime expectedDateTime = DateTime.Parse(expectedDateTimeString);
            Assert.AreEqual(expectedDateTime, abf.Header.StartDateTime);
        }

        [TestCase("16921011-vc-memtest-tags.abf")]
        [TestCase("17n16012-vc-steps.abf")]
        [TestCase("17n16016-ic-ramp.abf")]
        [TestCase("17n16016-ic-steps.abf")]
        [TestCase("18808025-memtest.abf")]
        public void Test_AbfFileHeader_Markdown(string abfFilename)
        {
            string abfFilePath = SampleData.GetAbfPath(abfFilename);
            var abf = new AbfSharp.ABF(abfFilePath);
            string md = abf.Header.GetHeaderMarkdown();
            Console.WriteLine(md);

            string abfFolder = System.IO.Path.GetDirectoryName(abfFilePath);
            string abfID = System.IO.Path.GetFileNameWithoutExtension(abfFilePath);
            string mdPath = System.IO.Path.Combine(abfFolder, $"{abfID}.md");
            System.IO.File.WriteAllText(mdPath, md);
            Console.WriteLine("Wrote: {mdPath}");
        }
    }
}
