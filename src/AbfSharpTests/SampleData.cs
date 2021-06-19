using NUnit.Framework;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AbfSharpTests
{
    public static class SampleData
    {
        public static readonly string DATA_FOLDER = Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../../../dev/abfs/");
        public static readonly string GRAPHICS_FOLDER = Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../../../dev/graphics/");

        public static string[] GetAllAbfPaths() => Directory.GetFiles(DATA_FOLDER, "*.abf");

        public static string GetAbfPath(string filename)
        {
            string fullpath = Path.Combine(DATA_FOLDER, filename);
            fullpath = Path.GetFullPath(fullpath);
            if (!File.Exists(fullpath))
                throw new ArgumentException($"file not found: {fullpath}");
            return fullpath;
        }

        [Test]
        public static void Test_SampleABFs_AreUnique()
        {
            var md5 = System.Security.Cryptography.MD5.Create();

            Dictionary<string, string> filenamesByHash = new();

            foreach (string abfFilePath in GetAllAbfPaths())
            {
                byte[] bytes = System.IO.File.ReadAllBytes(abfFilePath);
                string hash = string.Join("", md5.ComputeHash(bytes).Select(x => x.ToString("x2")).ToArray());
                if (filenamesByHash.ContainsKey(hash))
                    throw new ArgumentException($"Duplicate ABFs: {Path.GetFileName(abfFilePath)} {Path.GetFileName(filenamesByHash[hash])}");
                else
                    filenamesByHash[hash] = abfFilePath;
            }
        }
    };
}