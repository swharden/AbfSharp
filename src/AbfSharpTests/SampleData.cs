using NUnit.Framework;
using System.IO;
using System;

namespace AbfSharpTests
{
    public static class SampleData
    {
        public static readonly string DATA_FOLDER = Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../../../dev/abfs/");
        public static readonly string GRAPHICS_FOLDER = Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../../../dev/graphics/");

        public static string GetAbfPath(string filename)
        {
            string fullpath = Path.Combine(DATA_FOLDER, filename);
            fullpath = Path.GetFullPath(fullpath);
            if (!File.Exists(fullpath))
                throw new ArgumentException($"file not found: {fullpath}");
            return fullpath;
        }

        public static string[] GetAllAbfPaths() => Directory.GetFiles(DATA_FOLDER, "*.abf");
    }
}