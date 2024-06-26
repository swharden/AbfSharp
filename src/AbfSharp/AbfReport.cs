using System.Diagnostics;

namespace AbfSharp;

public class AbfReport(ABF abf)
{
    private readonly ABF ABF = abf;

    public string GetReportHtml()
    {
        Reports.AbfHeaderReport report = new(ABF.FilePath);
        return report.GetHtml();
    }

    public string GetReportMarkdown()
    {
        Reports.AbfHeaderReport report = new(ABF.FilePath);
        return report.GetMarkdown();
    }

    public void LaunchReportInBrowser()
    {
        string htmlFilePath = Path.GetTempFileName() + ".html";
        File.WriteAllText(htmlFilePath, GetReportHtml());

        ProcessStartInfo psi = new(htmlFilePath) { UseShellExecute = true };
        Process p = new() { StartInfo = psi };
        p.Start();
    }
}
