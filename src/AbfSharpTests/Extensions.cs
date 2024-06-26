using System.Diagnostics;
using System.Reflection;

namespace AbfSharpTests;

internal static class Extensions
{
    public static void SaveTestImage(this ScottPlot.Plot plot, int width = 600, int height = 400, string suffix = "")
    {
        StackTrace stackTrace = new();
        StackFrame frame = stackTrace.GetFrame(1) ?? throw new InvalidOperationException("unknown caller");
        MethodBase method = frame.GetMethod() ?? throw new InvalidDataException("unknown method");
        string callingMethod = method.Name;

        string saveFolder = Path.Combine(TestContext.CurrentContext.TestDirectory, "test_figures");
        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);
        string filename = $"{callingMethod}_{suffix}.png";
        string filePath = Path.Combine(saveFolder, filename);
        plot.SavePng(filePath, width, height);
        Console.WriteLine(filePath);
    }
}
