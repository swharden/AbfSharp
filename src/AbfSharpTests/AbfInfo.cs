using NUnit.Framework;
using System;

namespace AbfSharpTests;

public class AbfInfo
{
    [Test]
    public void Test_AbfFileHeader_Markdown()
    {
        foreach (string abfFilePath in SampleData.GetAllAbfPaths())
        {
            var abf = new AbfSharp.ABFFIO.ABF(abfFilePath);
            string md = AbfSharp.Reports.AbfHeaderReport.GetMarkdown(abf.Header);

            string abfFolder = System.IO.Path.GetDirectoryName(abfFilePath);
            string abfID = System.IO.Path.GetFileNameWithoutExtension(abfFilePath);
            string mdPath = System.IO.Path.Combine(abfFolder, $"{abfID}.md");
            System.IO.File.WriteAllText(mdPath, md);

            Console.WriteLine(mdPath);
        }
    }
}
