using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace ABFsharp.Analyzer
{
    public static class ModifyPlot
    {
        public static void Style(ScottPlot.Plot plt)
        {
            plt.Grid(color: ColorTranslator.FromHtml("#efefef"));
        }

        public static void YLabelIC(ScottPlot.Plot plt)
        {
            plt.YLabel("Membrane Potential (mV)");
        }

        public static void YLabelVC(ScottPlot.Plot plt)
        {
            plt.YLabel("Clamp Current (pA)");
        }

        public static void XLabelMillisec(ScottPlot.Plot plt)
        {
            plt.XLabel("Time (milliseconds)");
        }

        public static void XLabelSec(ScottPlot.Plot plt)
        {
            plt.XLabel("Time (seconds)");
        }

        public static void XLabelMin(ScottPlot.Plot plt)
        {
            plt.XLabel("Time (Minutes)");
        }

        public static void TitleCallingMethod(ScottPlot.Plot plt)
        {
            StackTrace stackTrace = new StackTrace();
            string callingMethodName = stackTrace.GetFrame(1).GetMethod().Name;
            plt.Title(callingMethodName.Replace("_", " "));
        }

        public static void XAxisOnly(ScottPlot.Plot plt)
        {
            plt.Frame(left: false, top: false, right: false, bottom: true);
            plt.Grid(false);
            plt.Ticks(displayTicksY: false);
        }

        public static void ZomToSD(ABFsharp.ABF abf, ScottPlot.Plot plt, double multiplier = 1)
        {
            Trace trace = abf.GetSweep(0);
            var stdev = StDev(trace.values);
            var mean = Mean(trace.values);
            plt.Axis(null, null, mean - stdev * multiplier, mean + stdev * multiplier);
        }

        // TODO: move all these to ScottPlot's statistics module
        private static double Sum(double[] values)
        {
            double sum = 0;
            for (int i = 0; i < values.Length; i++)
                sum += values[i];
            return sum;
        }

        private static double Mean(double[] values)
        {
            return Sum(values) / values.Length;
        }

        private static double StDev(double[] values)
        {
            double sumVariancesSquared = 0;
            double mean = Mean(values);
            for (int i = 0; i < values.Length; i++)
            {
                double pointVariance = Math.Abs(mean - values[i]);
                double pointVarianceSquared = Math.Pow(pointVariance, 2);
                sumVariancesSquared += pointVarianceSquared;
            }
            double meanVarianceSquared = sumVariancesSquared / values.Length;
            double stDev = Math.Sqrt(meanVarianceSquared);
            return stDev;
        }

        private static double StdErr(double[] values)
        {
            return StDev(values) / Math.Sqrt(values.Length);
        }
    }
}
