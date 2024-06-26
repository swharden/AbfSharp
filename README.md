# <img src="dev/icon/icon.ico" height="24" width="24"> AbfSharp
[![CI](https://github.com/swharden/AbfSharp/actions/workflows/ci.yaml/badge.svg)](https://github.com/swharden/AbfSharp/actions/workflows/ci.yaml)
[![NuGet](https://img.shields.io/nuget/vpre/abfsharp?color=%23004880&label=NuGet&logo=nuget)](https://www.nuget.org/packages/AbfSharp/)
[![GitHub](https://img.shields.io/github/license/swharden/abfsharp?color=%231281c0)](LICENSE)

**AbfSharp is a .NET library for reading electrophysiology data from Axon Binary Format (ABF) files.** ABFSharp wraps ABFFIO.DLL (the official ABF file reading library) and exposes its core functionality with an idiomatic .NET interface.

<div align="center">
  <img src="dev/graphics/Test_Plot_3D.png" width="70%" />
</div>

## Platform Requirements

Because AbfSharp calls a 32-bit Windows DLL, your application must be built against the same target. Modify your csproj file to include:

```xml
<TargetFramework>net8.0-windows</TargetFramework>
<PlatformTarget>x86</PlatformTarget>
```

## Quickstart

```cs
// Read the first sweep of an ABF file
AbfSharp.ABF abf = new("File_axon_5.abf");
float[] sweep = abf.GetSweep(0);

// Show the first 5 values of the sweep
for (int i = 0; i < 5; i++)
    Console.Write(sweep[i]);
```