using System;
using System.Collections.Generic;
using System.Text;

namespace ABFsharp.Tests
{
    public class DemoAbfPaths
    {
        public const string abfFolderPath = "../../../../../dev/abfs/";

        public string MemtestWithTags = System.IO.Path.GetFullPath(System.IO.Path.Combine(abfFolderPath, "vc_drug_memtest.abf"));
        public string VoltageClampIV = System.IO.Path.GetFullPath(System.IO.Path.Combine(abfFolderPath, "17n16012.abf"));
        public string CurrentClampRamp = System.IO.Path.GetFullPath(System.IO.Path.Combine(abfFolderPath, "17n16016.abf"));
        public string CurrentClampSteps = System.IO.Path.GetFullPath(System.IO.Path.Combine(abfFolderPath, "17n16018.abf"));
        public string Memtest = System.IO.Path.GetFullPath(System.IO.Path.Combine(abfFolderPath, "18808025.abf"));
    }
}
