using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ABFsharp.Analyzer.Analyses
{
    public class Analysis
    {
        private const string defaultOutputFolderName = "_autoanalysis";

        readonly string OutputFolder;

        public Analysis(string abfFilePath, string outputFolder)
        {
            abfFilePath = Path.GetFullPath(abfFilePath);
            if (!File.Exists(abfFilePath))
                throw new ArgumentException($"abf file does not exist: {abfFilePath}");

            outputFolder = (outputFolder is null) ?
                Path.Combine(Path.GetDirectoryName(abfFilePath), defaultOutputFolderName) :
                Path.GetFullPath(outputFolder);
            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);
            OutputFolder = outputFolder;
        }
    }
}
