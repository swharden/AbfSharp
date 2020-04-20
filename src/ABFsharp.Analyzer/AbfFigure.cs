using System;
using System.Collections.Generic;
using System.Text;

namespace ABFsharp.Analyzer
{
    public class AbfFigure
    {
        private readonly ABF abf;
        public readonly string outputFolder;
        private readonly ScottPlot.Plot plt = new ScottPlot.Plot();

        public AbfFigure(ABF abf, string outputFolder = "./")
        {
            if (abf is null)
                throw new ArgumentException("input ABF cannot be null");
            if (!System.IO.Directory.Exists(outputFolder))
                throw new ArgumentException("output folder doesn't exist");

            this.abf = abf;
            this.outputFolder = System.IO.Path.GetFullPath(outputFolder);
        }

        public void Overlay()
        {
            for (int i = 0; i < abf.info.sweepCount; i++)
            {
                var sweep = abf.GetSweep(i);
                plt.PlotSignal(sweep.values, abf.info.sampleRate);
            }
            plt.Title($"{abf.info.id} Overlay ({abf.info.sweepCount} sweeps)");
            plt.AxisAuto(0);

            Save(plt, "overlay");
        }

        private void Save(ScottPlot.Plot plt, string suffix, int width = 600, int height = 400)
        {
            string fileName = $"{abf.info.id}_{suffix}.png";
            string filePath = System.IO.Path.Combine(outputFolder, fileName);

            plt.Resize(width, height);
            plt.SaveFig(filePath);
            Console.WriteLine($"Saved: {filePath}");
        }
    }
}
