using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ABFsharp.Tests
{
    public static class TestTools
    {
        public static void SaveFig(ScottPlot.Plot plt, int? resizeX = null, int? resizeY = null)
        {
            plt.Resize(resizeX, resizeY);

            StackTrace stackTrace = new StackTrace();
            string callingMethodName = stackTrace.GetFrame(1).GetMethod().Name;
            string saveFileName = $"{callingMethodName}.png";
            string saveFolder = System.IO.Path.GetFullPath("./output");
            if (!System.IO.Directory.Exists(saveFolder))
                System.IO.Directory.CreateDirectory(saveFolder);
            string saveFilePath = System.IO.Path.Combine(saveFolder, saveFileName);
            
            plt.SaveFig(saveFilePath);
            Console.WriteLine($"Saved: {saveFilePath}");
        }
    }
}
