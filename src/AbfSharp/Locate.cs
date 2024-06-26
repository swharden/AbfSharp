namespace AbfSharp;

public static class Locate
{
    public static string[] Abfs(string folder, bool recursive = true)
    {
        return Directory.GetFiles(folder, "*.abf", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
    }

    public static IEnumerable<string> AbfsWithProtocol(string folder, string protocol, bool recursive = true)
    {
        foreach (string abf in Abfs(folder, recursive))
        {
            AbfFileInfo info = new(abf);
            string abfProtocol = Path.GetFileNameWithoutExtension(info.Protocol);
            if (abfProtocol.Contains(protocol))
                yield return abf;
        }
    }
}
