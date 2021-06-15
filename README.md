# AbfSharp
[![](https://img.shields.io/azure-devops/build/swharden/swharden/5?label=Build&logo=azure%20pipelines)](https://dev.azure.com/swharden/swharden/_build/latest?definitionId=5&branchName=master)

**AbfSharp is a .NET Standard library for reading electrophysiology data from Axon Binary Format (ABF) files.** AbfSharp wraps ABFFIO.DLL (the official closed-source library) and provides a simple .NET interface, handling low-level calls and memory management so you can focus on writing ABF analysis code.

<div align="center">

![](dev/graphics/Test_Plot_3D.png)

</div>

### Project Status

**⚠️ AbfSharp is pre-release** (version < 1.0.0) meaning it is functional, but its API is not guaranteed to be stable across future versions.

### Supported Platforms

* AbfSharp targets .NET Standard 2.0 so it can be used in .NET Framework and .NET Core applications

* ABFFIO.DLL is only available as a 32-bit binary for Windows. Since AbfSharp just wraps this DLL, applications that use AbfSharp must also run Windows and build targeting the `x86` (32-bit) platform. To work with ABF files on other operating systems visit the [pyABF project](https://swharden.com/pyabf).

## Quickstart

Load data from an ABF file:

```cs
var abf = AbfSharp.ABF("demo.abf");
double[] voltage = abf.GetSweep(0).Values;
```

Plot sweep data using [ScottPlot](https://swharden.com/scottplot):

```cs
var plt = new ScottPlot.Plot(600, 400);
plt.PlotSignal(sweep.Values, sweep.SampleRate);
plt.Title("Full Recording");
plt.XLabel("Experiment Time (seconds)");
plt.YLabel("Membrane Porential (mV)");
plt.SaveFig("demo.png");
```

![](dev/graphics/Test_Plot_FullRecording.png)

## Resources
* [pyABF](https://swharden.com/pyabf) - a pure-Python interface for ABF files
* [phpABF](https://github.com/swharden/phpABF) - a pure-PHP interface for reading header data from ABF files
* [The ABF File Format](https://swharden.com/pyabf/abf2-file-format/) - extensive documentation about the format of data in binary ABF files
* [Axon Binary File Format](http://mdc.custhelp.com/euf/assets/software/FSP_ABFHelp_2.03.pdf) - official user guide
* [Axon Multiclamp SDK](http://mdc.custhelp.com/euf/assets/images/Multiclamp_SDK.zip) - now hosted by [Molecular Devices](https://support.moleculardevices.com/s/article/Axon-Conventional-Electrophysiology-Suite-of-Products-Knowledge-Base)
* ABFFIO.DLL can be independently obtained from the latest [pCLAMP](https://support.moleculardevices.com/s/article/Axon-pCLAMP-11-Electrophysiology-Data-Acquisition-Analysis-Software-Download-Page) distribution