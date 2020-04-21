using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ABFsharp.Tests.Analyzer
{
    class Figures
    {
        DemoAbfPaths sampleABFs = new DemoAbfPaths();

        [Test]
        public void Test_Plot_Continuous()
        {
            ABF abf = new ABF(sampleABFs.CurrentClampRamp);
            var plt = new ScottPlot.Plot(600, 400);

            ABFsharp.Analyzer.PlotData.Continuous(abf, plt);

            ABFsharp.Analyzer.ModifyPlot.Style(plt);
            ABFsharp.Analyzer.ModifyPlot.YLabelIC(plt);
            ABFsharp.Analyzer.ModifyPlot.XLabelSec(plt);
            ABFsharp.Analyzer.ModifyPlot.TitleCallingMethod(plt);

            TestTools.SaveFig(plt);
        }

        [Test]
        public void Test_Plot_Overlay()
        {
            ABF abf = new ABF(sampleABFs.CurrentClampSteps);
            var plt = new ScottPlot.Plot(600, 400);

            ABFsharp.Analyzer.PlotData.Overlay(abf, plt, 50);

            ABFsharp.Analyzer.ModifyPlot.Style(plt);
            ABFsharp.Analyzer.ModifyPlot.YLabelIC(plt);
            ABFsharp.Analyzer.ModifyPlot.XLabelSec(plt);
            ABFsharp.Analyzer.ModifyPlot.TitleCallingMethod(plt);

            TestTools.SaveFig(plt);
        }

        [Test]
        public void Test_Plot_OverlayMemtest()
        {
            ABF abf = new ABF(sampleABFs.Memtest);
            var plt = new ScottPlot.Plot(600, 400);

            ABFsharp.Analyzer.PlotData.Overlay(abf, plt, 50, allSameColor: false);

            ABFsharp.Analyzer.ModifyPlot.Style(plt);
            ABFsharp.Analyzer.ModifyPlot.YLabelVC(plt);
            ABFsharp.Analyzer.ModifyPlot.XLabelSec(plt);
            ABFsharp.Analyzer.ModifyPlot.TitleCallingMethod(plt);
            ABFsharp.Analyzer.ModifyPlot.ZomToSD(abf, plt, 5);

            TestTools.SaveFig(plt);
        }

        [Test]
        public void Test_Plot_Stack()
        {
            ABF abf = new ABF(sampleABFs.CurrentClampSteps);
            var plt = new ScottPlot.Plot(600, 400);

            ABFsharp.Analyzer.PlotData.Stack(abf, plt, 150);

            ABFsharp.Analyzer.ModifyPlot.Style(plt);
            ABFsharp.Analyzer.ModifyPlot.XLabelSec(plt);
            ABFsharp.Analyzer.ModifyPlot.TitleCallingMethod(plt);
            ABFsharp.Analyzer.ModifyPlot.XAxisOnly(plt);

            TestTools.SaveFig(plt);
        }
    }
}
