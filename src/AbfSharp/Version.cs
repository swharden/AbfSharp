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

        string thisFolder = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!);
        string dllFilePath = Path.Combine(thisFolder, "ABFFIO.DLL");
        Console.WriteLine(dllFilePath);

        if (!File.Exists(dllFilePath))
            throw new FileNotFoundException("ABFFIO.DLL should be in the folder next to the executable");

        return FileVersionInfo.GetVersionInfo(dllFilePath).FileVersion ?? string.Empty;
    }
}
