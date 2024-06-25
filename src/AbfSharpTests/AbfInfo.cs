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
}
