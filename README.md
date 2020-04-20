# ABFsharp
[![](https://img.shields.io/azure-devops/build/swharden/swharden/5?label=Build&logo=azure%20pipelines)](https://dev.azure.com/swharden/swharden/_build/latest?definitionId=5&branchName=master)

ABFsharp is a .NET library that makes it easy to read electrophysiology data from Axon Binary Format (ABF) files.

> **⚠️ WARNING:** This project is early in development and not ready for use in production environments. 

![](dev/quickstart.png)

ABFsharp wraps ABFFIO.DLL (the official C library provided to interface ABF files) and handles the low-level calls so you can focus on writing ABF analysis software using expressive code and modern language paradigms.

## Supported Platforms

ABFsharp targets .NET Standard so it can be used in .NET Framework and .NET Core applications.

ABFsharp wraps ABFFIO.DLL which is a closed source, Windows-only, 32-bit x86 DLL. Therefore, ABFsharp can only be used on Windows and cannot be used in 64-bit applications. One day I may create a 64-bit wrapper, but this is not yet available.

Developers interested in learning more about reading data from ABF files directly in 64-bit and cross-platform environments may find useful resources in the [pyABF](https://github.com/swharden/pyABF) project.

## ABFsharp API

This section summarizes how ABFsharp reads and exposes ABF data, which is surprisingly different from the underlying ABFFIO library it wraps. Unlike ABFFIO's calling routines which rely on internal state and special sequences of internal method calls, the ABFsharp library favors immutability and statelessness.

* The entire ABF file is read into memory when the `ABF` object is instantiated. This avoids the file-locking behavior typically associated with applications that use ABFFIO (like ClampFit).

* Header values are stored in the `ABF` and are immutable.

* Methods are used to request data, and all data is returned as new `Trace` objects which contain duplicated mutable data.

* `Trace` objects have have `values`, a `sampleRate`, `commandValues`, `units`, `commandUnits`, etc. All values of `Trace` objects are mutable, but mutating their data does not affect data in the `ABF` that created them.

The ABFFIO library can only load one sweep into memory at a time. In contrast, ABFsharp loads all sweeps into memory at once. Data is cloned and loaded into `Trace` objects as it is requested. This means ABFsharp uses more memory than ABFFIO, but also that ABFsharp is extremely fast in comparison because it doesn't have to read data from the hard disk every time data for a sweep is requested. It also means ABFsharp is safer to use than ABFFIO because you cannot unintentionally mutate the source data. Finally, this model lends itself to multi-threaded analysis routines.

How much memory does ABFsharp require? A 1-hour 20kHz recording has 72 million points. Storing these as double (64-bit) values, that's 576 MB. Even if the memory requirement is doubled due to an architecture that favors immutability, any modern computer will handle this without concern.

## Quickstart Examples

These examples demonstrate how to use ABFsharp in combination with [ScottPlot](http://swharden.com/scottplot/) to perform common ABF plotting tasks. 

All examples assume the following:

```cs
var abf = new ABFsharp.ABF("file.abf");
var plt = new ScottPlot.Plot();
```

#### Plot Sweep

```cs
var trace = abf.GetSweep(0);
plt.PlotSignal(trace.values, trace.sampleRate);
```

_Gap-free recordings are treated as episodic recordings with a single sweep_

#### Plot Full Recording

```cs
var trace = abf.GetAllSweeps();
plt.PlotSignal(trace.values, trace.sampleRate);
```

#### Overlay Sweeps
```cs
for (int i=0; i<abf.sweepCount; i++) {
    var trace = abf.GetSweep(i);
    plt.PlotSignal(trace.values, trace.sampleRate);
}
```

#### Stacked Sweeps with Baseline Subtraction
```cs
for (int i=0; i<abf.sweepCount; i++) {
    var trace = abf.GetSweep(i);
    trace.Baseline(1.0, 1.5);
    plt.PlotSignal(trace.values, trace.sampleRate, yOffset: 100 * i);
}
```

#### Area of an Evoked Current by Sweep
```cs
double[] areasBySweep = new double[abf.sweepCount];
for (int i=0; i<abf.sweepCount; i++) {
    var trace = abf.GetSweep(i);
    trace.Baseline(1.0, 1.5);
    var stats = trace.Measure(2.0, 2.5);
    areasBySweep[i] = stats.area;
}
plt.PlotScatter(abf.startTimesMin, areasBySweep);
```