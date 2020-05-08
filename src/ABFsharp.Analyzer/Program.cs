using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ABFsharp.Analyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            string testFolder = @"D:\Data\abfs-2019\project2\abfs";
            foreach (var abfFilePath in Directory.GetFiles(testFolder, "*.abf"))
            {
                var abf = new ABF(abfFilePath, ABF.Preload.HeaderOnly);


                if (abf.protocol == "0201 memtest")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{abf.abfID} {abf.protocol}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    AnalyzeMemtest(abf);
                    //continue;
                    break;
                }

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"{abf.abfID} {abf.protocol} (unknown analysis)");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        public static void AnalyzeMemtest(ABF abf)
        {
            var sweep = abf.GetSweep(0);
            var plt = new ScottPlot.Plot();
            plt.Grid(color: ColorTranslator.FromHtml("#efefef")); // TODO: fix this somehow

            plt.PlotSignal(sweep.values);

            foreach (var epoch in abf.epochTable.Epochs)
                if (epoch.isValid)
                    plt.PlotHSpan(epoch.indexFirst, epoch.indexLast, label: epoch.Name);
            plt.Legend();

            plt.YLabel("Clamp Current (pA)");
            plt.XLabel("Array Index");
            plt.AxisAuto(0);

            new ScottPlot.FormsPlotViewer(plt).ShowDialog();
        }
    }
}
