# ABFsharp
[![](https://img.shields.io/azure-devops/build/swharden/swharden/5?label=Build&logo=azure%20pipelines)](https://dev.azure.com/swharden/swharden/_build/latest?definitionId=5&branchName=master)

ABFsharp is a .NET library that makes it easy to read electrophysiology data from Axon Binary Format (ABF) files.

> **⚠️ WARNING:** This project is early in development and not ready for use in production environments. 

ABFsharp wraps ABFFIO.DLL (the official C library provided to interface ABF files) and handles the low-level calls so you can focus on writing ABF analysis software using expressive code and modern language paradigms. 

ABFsharp targets .NET Standard so it can be used in .NET Framework and .NET Core applications.

## Platform Requirements

ABFsharp wraps ABFFIO.DLL which is a closed source, Windows-only, 32-bit x86 DLL. Therefore, ABFsharp can only be used on Windows and cannot be used in 64-bit applications. One day I may create a 64-bit wrapper, but this is not yet available.

Developers interested in learning more about reading data from ABF files directly in 64-bit and cross-platform environments may find useful resources in the [pyABF](https://github.com/swharden/pyABF) project.

## Quickstart

![](dev/quickstart.png)

```cs
var abf = new ABFsharp.ABF("17n16018.abf");
var plt = new ScottPlot.Plot(800, 400);

for (int i = 0; i < abf.sweepCount; i += 4)
{
    var sweep = abf.GetSweep(i);
    plt.PlotSignal(sweep.values, sweep.sampleRate, label: $"sweep {i + 1}");
}

plt.Title(abf.fileName);
plt.YLabel("Membrane Potential (mV)");
plt.XLabel("Sweep Time (seconds)");
plt.Legend();
plt.AxisAuto(0);
plt.SaveFig("quickstart.png");
```
