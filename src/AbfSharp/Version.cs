using System.Diagnostics;
using System.Reflection;

namespace AbfSharp;

public static class Version
{
    /// <summary>
    /// Version formatted like "1.2.3-beta"
    /// </summary>
    public static string VersionString { get; private set; } = GetVersionString();

    private static string GetVersionString()
    {
        string v = Assembly.GetAssembly(typeof(ABF))!
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
            .InformationalVersion;

        return v.Contains('+') ? v.Split('+')[0] : v;
    }

    public static string DllVersion = GetDllVersion();

    private static string GetDllVersion()
    {
        if (IntPtr.Size != 4)
            throw new InvalidOperationException("AbfSharp can only be used in 32-bit (x86) projects");

        string assemblyFolder = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!);
        string[] expectedRelativePaths =
        {
            Path.Combine(assemblyFolder, "ABFFIO.DLL"),
            Path.Combine(assemblyFolder, "runtimes/win-x86/native/ABFFIO.DLL"),
        };

        foreach (string relativePath in expectedRelativePaths)
        {
            string dllPath = Path.Combine(assemblyFolder, relativePath);
            if (File.Exists(dllPath))
                return FileVersionInfo.GetVersionInfo(dllPath).FileVersion ?? string.Empty;
        }

        throw new FileNotFoundException("ABFFIO.DLL not found");
    }
}
