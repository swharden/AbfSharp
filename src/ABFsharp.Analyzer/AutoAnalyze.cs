using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ABFsharp.Analyzer
{
    public static class AutoAnalyze
    {
        const string defaultOutputFolderName = "_autoanalysis";

        public static void AbfFile(string abfFilePath, string outputFolder)
        {
            //var a = new Analyses.Analysis(abfFilePath, outputFolder);

            // validate ABF file
            if (!System.IO.File.Exists(abfFilePath))
                throw new ArgumentException($"abf file does not exist: {abfFilePath}");
            abfFilePath = System.IO.Path.GetFullPath(abfFilePath);
            string abfFileName = System.IO.Path.GetFileName(abfFilePath);
            string abfID = System.IO.Path.GetFileNameWithoutExtension(abfFilePath);

            // validate output folder
            string abfFolder = System.IO.Path.GetDirectoryName(abfFilePath);
            if (outputFolder is null)
                outputFolder = System.IO.Path.Combine(abfFolder, defaultOutputFolderName);
            else
                outputFolder = System.IO.Path.GetFullPath(outputFolder);
            if (!System.IO.Directory.Exists(outputFolder))
                System.IO.Directory.CreateDirectory(outputFolder);

            // analyze
            ABF abf = new ABF(abfFilePath);
            var plt = new ScottPlot.Plot();
            PlotData.Continuous(abf, plt);

            // save
            string outputFilePath = System.IO.Path.Combine(outputFolder, $"{abfID}_continuous.png");
            plt.SaveFig(outputFilePath);
            Debug.WriteLine($"Saved: {outputFilePath}");
        }
    }
}
