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

}
