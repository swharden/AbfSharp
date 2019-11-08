using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ABFsharpTests
{
    [TestClass]
    public class UnitTest1
    {
        private string folderAbfs;
        private string folderHere;
        private string abfFilePath;

        [TestInitialize]
        public void TestInitialize()
        {
            folderHere = System.IO.Path.GetFullPath(".");
            folderAbfs = System.IO.Path.GetFullPath(folderHere + "/../../../../dev/abfs");
            abfFilePath = System.IO.Path.Combine(folderAbfs, "17n16012.abf");
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public void Test_DLL_Exists()
        {
            if (!System.IO.File.Exists("ABFFIO.DLL"))
                throw new Exception("ABFFIO.DLL not found");
        }

        [TestMethod]
        public void Test_DemoFiles_MustExist()
        {
            Console.WriteLine($"folderHere: {folderHere}");
            Console.WriteLine($"folderAbfs: {folderAbfs}");
            Console.WriteLine($"abfFilePath: {abfFilePath}");

            if (!System.IO.File.Exists(abfFilePath))
                throw new Exception("ABF file not found");
        }

        [TestMethod]
        public void Test_ABF_Loads()
        {
            var abf = new ABFsharp.ABF(abfFilePath);
            Console.WriteLine(abf.info);
        }
    }
}
