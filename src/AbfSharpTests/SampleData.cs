namespace AbfSharpTests;

public static class SampleData
{
    public static readonly string REPO_ROOT = GetRepoRoot();

    private static string GetRepoRoot()
    {
        string folder = Path.GetFullPath("./");

        for (int i = 0; i < 100; i++)
        {
            string parentFolder = Path.GetDirectoryName(folder);
            string licenseFile = Path.Combine(parentFolder, "LICENSE");
            if (File.Exists(licenseFile))
                return parentFolder;
            folder = parentFolder;
        }

        throw new InvalidOperationException("license file never found");
    }

    public static readonly string DATA_FOLDER = Path.Combine(REPO_ROOT, "dev/abfs/");
    public static readonly string GRAPHICS_FOLDER = Path.Combine(REPO_ROOT, "dev/graphics/");

    [Test]
    public static void Test_Path_ABFFIO_DLL_IsInWorkingDirectory()
    {
        string wd = Path.GetFullPath("./");
        Console.WriteLine($"Working directory: {wd}");

        string dllPath = Path.Combine(wd, "ABFFIO.dll");
        File.Exists(dllPath).Should().BeTrue();
    }

    [Test]
    public static void Test_Environment_Is32Bit()
    {
        Environment.Is64BitProcess.Should().BeFalse();
    }

    [Test]
    public static void Test_Path_RepoRoot() => Directory.Exists(REPO_ROOT).Should().BeTrue();

    [Test]
    public static void Test_Path_DataFolder() => Directory.Exists(DATA_FOLDER).Should().BeTrue();

    [Test]
    public static void Test_Path_GraphicsFolder() => Directory.Exists(GRAPHICS_FOLDER).Should().BeTrue();

    public static string[] GetAllAbfPaths() => Directory.GetFiles(DATA_FOLDER, "*.abf");

    public static string GetAbfPath(string filename)
    {
        string fullpath = Path.Combine(DATA_FOLDER, filename);
        fullpath = Path.GetFullPath(fullpath);
        if (!File.Exists(fullpath))
            throw new ArgumentException($"file not found: {fullpath}");
        return fullpath;
    }
};