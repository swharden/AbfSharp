using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfSharpTests
{
    class Plot
    {
        private static void SaveFig(ScottPlot.Plot plt, string filename)
        {
            string fullPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(SampleData.GRAPHICS_FOLDER, filename));
            plt.SaveFig(fullPath);
            Console.WriteLine(fullPath);
        }

        [Test]
        public void Test_Quickstart_Values()
        {
            string abfFilePath = SampleData.GetAbfPath("17n16016-ic-steps.abf");
            var abf = new AbfSharp.ABF(abfFilePath);
            double[] values = abf.GetSweep(0).Values;
            for (int i = 0; i < 10; i++)
                Console.Write($"{values[i]:F3}, ");

            // -62.469, -62.317, -62.439, -62.439, -62.317, -62.469, -62.347, -62.256, -62.469, -62.317,
        }

        [Test]
        public void Test_Quickstart_Plot()
        {
            string abfFilePath = SampleData.GetAbfPath("17n16016-ic-steps.abf");
            var abf = new AbfSharp.ABF(abfFilePath);

            var plot = new ScottPlot.Plot(600, 300);
            foreach (int sweepIndex in new int[] { 0, 4, 8, 12 })
            {
                var sweep = abf.GetSweep(sweepIndex);
                double[] times = sweep.GetSweepTimes();
                double[] voltages = sweep.Values;
                plot.AddScatterLines(times, voltages, label: $"sweep {sweepIndex + 1}");
            }

            plot.AxisAuto(horizontalMargin: 0);
            plot.XLabel("Time (seconds)");
            plot.YLabel("Potential (mV)");
            plot.Legend(true, ScottPlot.Alignment.UpperRight);

            SaveFig(plot, "quickstart-plot.png");
        }
    }
}
