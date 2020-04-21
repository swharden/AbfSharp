using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace ABFsharp.Tests
{
    public class Plot
    {
        DemoAbfPaths sampleABFs = new DemoAbfPaths();

        private (ABF abf, ScottPlot.Plot plt) GetAbfAndPlot(string abfFilePath)
        {
            var abf = new ABF(abfFilePath);
            var plt = new ScottPlot.Plot(600, 400);
            return (abf, plt);
        }

        private void SaveFig(ScottPlot.Plot plt, string title = null,
            string xLabel = "Sweep Time (milliseconds)", string yLabel = "Clamp Current (pA)",
            bool tightX = true)
        {
            StackTrace stackTrace = new StackTrace();
            string callingMethodName = stackTrace.GetFrame(1).GetMethod().Name;
            title = (title is null) ? callingMethodName.Replace("_", " ") : title;

            if (tightX)
                plt.AxisAuto(0);

            plt.Title(title.Trim());
            plt.YLabel(yLabel);
            plt.XLabel(xLabel);
            plt.Legend(location: ScottPlot.legendLocation.upperRight);

            string saveFilePath = System.IO.Path.GetFullPath($"{callingMethodName}.png");
            plt.SaveFig(saveFilePath);
            Console.WriteLine($"Saved: {saveFilePath}");
        }

        [Test]
        public void Test_Plot_FirstSweep()
        {
            (ABF abf, ScottPlot.Plot plt) = GetAbfAndPlot(sampleABFs.Memtest);

            var trace = abf.GetSweep(0);
            plt.PlotSignal(trace.values, trace.sampleRate / 1000);

            SaveFig(plt, "First Sweep");
        }

        [Test]
        public void Test_Plot_SecondSweep()
        {
            (ABF abf, ScottPlot.Plot plt) = GetAbfAndPlot(sampleABFs.Memtest);

            var trace = abf.GetSweep(1);
            plt.PlotSignal(trace.values, trace.sampleRate / 1000);

            SaveFig(plt, "Second Sweep");
        }

        [Test]
        public void Test_Plot_LastSweep()
        {
            (ABF abf, ScottPlot.Plot plt) = GetAbfAndPlot(sampleABFs.Memtest);

            var trace = abf.GetSweep(-1);
            plt.PlotSignal(trace.values, trace.sampleRate / 1000);

            SaveFig(plt, "Last Sweep");
        }

        [Test]
        public void Test_Plot_FullRecording()
        {
            (ABF abf, ScottPlot.Plot plt) = GetAbfAndPlot(sampleABFs.CurrentClampRamp);

            var trace = abf.GetFullRecording();
            plt.PlotSignal(trace.values, trace.sampleRate);

            SaveFig(plt, "Full Recording", xLabel: "Experiment Time (seconds)", yLabel: "Membrane Potential (mV)");
        }

        [Test]
        public void Test_Plot_Overlay()
        {
            (ABF abf, ScottPlot.Plot plt) = GetAbfAndPlot(sampleABFs.CurrentClampSteps);

            for (int i = 0; i < abf.sweepCount; i += 5)
            {
                var trace = abf.GetSweep(i);
                plt.PlotSignal(trace.values, trace.sampleRate,
                    label: $"Sweep {i + 1}", lineWidth: 2);
            }

            SaveFig(plt, "Overlayed Sweeps", xLabel: "Sweep Time (seconds)", yLabel: "Membrane Potential (mV)");
        }

        [Test]
        public void Test_Plot_Stacked()
        {
            (ABF abf, ScottPlot.Plot plt) = GetAbfAndPlot(sampleABFs.CurrentClampSteps);

            for (int i = 5; i < abf.sweepCount; i++)
            {
                var trace = abf.GetSweep(i);
                plt.PlotSignal(trace.values, trace.sampleRate,
                    color: Color.Blue, yOffset: i * 120);
            }

            plt.Grid(false);
            plt.Ticks(displayTicksY: false);
            SaveFig(plt, "Stacked Sweeps", xLabel: "Sweep Time (seconds)", yLabel: null);
        }

        Color Mix(Color colorA, Color colorB, double fracA)
        {
            byte r = (byte)((colorA.R * (1 - fracA)) + colorB.R * fracA);
            byte g = (byte)((colorA.G * (1 - fracA)) + colorB.G * fracA);
            byte b = (byte)((colorA.B * (1 - fracA)) + colorB.B * fracA);
            return Color.FromArgb(r, g, b);
        }

        Color Mix(string hexA, string hexB, double fracA)
        {
            Color colorA = ColorTranslator.FromHtml(hexA);
            Color colorB = ColorTranslator.FromHtml(hexB);
            return Mix(colorA, colorB, fracA);
        }

        [Test]
        public void Test_Plot_3D()
        {
            (ABF abf, ScottPlot.Plot plt) = GetAbfAndPlot(sampleABFs.CurrentClampSteps);

            for (int i = abf.sweepCount - 1; i >= 0; i--)
            {
                double frac = (double)i / (abf.sweepCount - 1);
                Color color = Mix("#8181f2", "#a2f0ca", frac);
                var trace = abf.GetSweep(i);
                plt.PlotSignal(
                    ys: trace.values,
                    sampleRate: trace.sampleRate,
                    color: color,
                    yOffset: i * 15,
                    xOffset: i * .025, 
                    maxRenderIndex: 20000, lineWidth: 2);
            }

            // hack-in an L-shaped scalebar until ScottPlot supports it natively
            plt.PlotLine(1.15, -50, 1.25, -50, lineWidth: 2, color: Color.Black);
            plt.PlotLine(1.25, -50, 1.25, -10, lineWidth: 2, color: Color.Black);
            plt.PlotText("100 ms", 1.2, -50, color: Color.Black, fontSize: 16,
                alignment: ScottPlot.TextAlignment.upperCenter);
            plt.PlotText("40 mV", 1.25, -58, color: Color.Black, fontSize: 16, rotation: -90);

            plt.Grid(false);
            plt.Ticks(false, false);
            plt.Frame(false);
            plt.Resize(800, 600);
            SaveFig(plt, " ", null, null);
        }
    }
}
