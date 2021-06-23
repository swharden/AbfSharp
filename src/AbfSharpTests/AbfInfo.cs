using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AbfSharpTests
{
    /// <summary>
    /// These tests generate Markdown documentation of ABF header values
    /// </summary>
    class AbfInfo
    {
        [Ignore("FIX THIS SOON AND MAKE IT CONSISTENT WITH PYABF")]
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

        [Test]
        public void Test_AbfFileHeader_Markdown()
        {
            foreach (string abfFilePath in SampleData.GetAllAbfPaths())
            {
                var abf = new AbfSharp.ABFFIO.ABF(abfFilePath);
                string md = GetHeaderMarkdown(abf.Header);
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
