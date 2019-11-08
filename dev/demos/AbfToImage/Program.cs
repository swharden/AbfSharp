﻿using System;
using System.Drawing;

namespace AbfToImage
{
    class Program
    {
        static void Main(string[] args)
        {
            string pathAbfFolder = System.IO.Path.GetFullPath("../../../../../abfs");
            string abfFilePath = System.IO.Path.Combine(pathAbfFolder, "17n16018.abf");
            if (!System.IO.File.Exists(abfFilePath))
                throw new Exception("ABF file does not exist");

            var abf = new ABFsharp.ABF(abfFilePath);
            var plt = new ScottPlot.Plot(800, 400);

            for (int i = 0; i < abf.info.sweepCount; i += 4)
            {
                abf.SetSweep(i);
                plt.PlotSignal(abf.sweep.valuesCopy, abf.info.sampleRate, label: $"sweep {i + 1}");
            }

            plt.Title(abf.info.fileName);
            plt.YLabel("Membrane Potential (mV)");
            plt.XLabel("Sweep Time (seconds)");
            plt.Legend();
            plt.AxisAuto(0);
            plt.SaveFig("sweep.png");
        }
    }
}