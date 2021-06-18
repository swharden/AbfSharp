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
        /// <summary>
        /// Markdown-formatted table of all header struct values
        /// </summary>
        /// <returns></returns>
        private static string GetHeaderMarkdown(AbfSharp.ABFFIO.Structs.ABFFileHeader HeaderStruct)
        {
            StringBuilder sb = new();
            sb.AppendLine("Name | Type | Value");
            sb.AppendLine("---|---|---");

            FieldInfo[] fields = HeaderStruct.GetType().GetFields();
            foreach (FieldInfo fi in fields)
            {
                object structElementValue = HeaderStruct.GetType().GetField(fi.Name).GetValue(HeaderStruct);
                if (structElementValue.GetType().IsArray)
                {
                    List<string> vals = new();
                    int length = ((Array)structElementValue).Length;
                    foreach (object arrayValue in (Array)structElementValue)
                        vals.Add(arrayValue.ToString());
                    if (vals.Count > 20)
                    {
                        vals = vals.Take(20).ToList();
                        vals.Add("...");
                    }
                    string typeName = structElementValue.GetType().ToString().Replace("[]", $"[{length}]").Replace("System.", "");
                    sb.AppendLine($"{fi.Name} | {typeName} | {string.Join(", ", vals)}");
                }
                else if (structElementValue.GetType() == typeof(string))
                {
                    string s = (string)structElementValue;
                    sb.AppendLine($"{fi.Name} | string ({s.Length}) | \"{s}\"");
                }
                else
                {
                    string typeName = structElementValue.GetType().ToString().Replace("System.", "");
                    sb.AppendLine($"{fi.Name} | {typeName} | {structElementValue}");
                }
            }

            return sb.ToString();
        }

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

        [Test]
        public void Test_AbfFileHeader_Markdown()
        {
            foreach (string abfFilePath in SampleData.GetAllAbfPaths())
            {
                var abf = new AbfSharp.ABF(abfFilePath);
                string md = GetHeaderMarkdown(abf.Header.HeaderStruct);
                Console.WriteLine(md);

                string abfFolder = System.IO.Path.GetDirectoryName(abfFilePath);
                string abfID = System.IO.Path.GetFileNameWithoutExtension(abfFilePath);
                string mdPath = System.IO.Path.Combine(abfFolder, $"{abfID}.md");
                System.IO.File.WriteAllText(mdPath, md);
                Console.WriteLine("Wrote: {mdPath}");
            }
        }
    }
}
