using ScottPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Linq;

namespace ABFsharp.Analyzer
{
    public static class PlotData
    {
        public static void Continuous(ABF abf, ScottPlot.Plot plt)
        {
            var trace = abf.GetFullRecording();
            plt.PlotSignal(trace.values, trace.sampleRate);
            plt.AxisAuto(0);
        }

        public static void Overlay(ABF abf, ScottPlot.Plot plt, byte alpha = 255, bool allSameColor = true)
        {
            var colors = new ScottPlot.Config.Colors();

            for (int i = 0; i < abf.sweepCount; i++)
            {
                int colorIndex = (allSameColor) ? 0 : i;
                var color = Color.FromArgb(alpha, colors.GetColor(colorIndex));
                var trace = abf.GetSweep(i);
                plt.PlotSignal(trace.values, trace.sampleRate, color: color);
            }
            plt.AxisAuto(0);
        }

        public static void Stack(ABF abf, ScottPlot.Plot plt, double? yOffset = null)
        {
            var colors = new ScottPlot.Config.Colors();
            var C0 = colors.GetColor(0);

            for (int i = 0; i < abf.sweepCount; i++)
            {
                var trace = abf.GetSweep(i);
                if (yOffset is null)
                    yOffset = trace.values.Max() - trace.values.Min();
                plt.PlotSignal(trace.values, trace.sampleRate, color: C0, yOffset: i * yOffset.Value);
            }
            plt.AxisAuto(0);
        }

    }
}
