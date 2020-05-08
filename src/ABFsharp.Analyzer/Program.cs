using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ABFsharp.Analyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            string testFolder = @"D:\Data\abfs-2019\project2\abfs";
            foreach (var abfFilePath in Directory.GetFiles(testFolder, "*.abf"))
            {
                var abf = new ABF(abfFilePath, ABF.Preload.HeaderOnly);


                if (abf.protocol == "0201 memtest")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{abf.abfID} {abf.protocol}");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    AnalyzeMemtest(abf);
                    //continue;
                    break;
                }

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"{abf.abfID} {abf.protocol} (unknown analysis)");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        public static void AnalyzeMemtest(ABF abf)
        {
            
        }
    }
}
