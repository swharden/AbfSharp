using System;
using System.Diagnostics;
using System.Linq;

namespace ABFsharp.Analyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Command(args);

            string testArgLine = "analyze.exe " +
                "../../../../../dev/abfs/17n16012.abf " +
                "trace -stacked -yOffset 100 -baselineSec 0 1";
            Command(testArgLine.Split(" "));
        }

        static void Help()
        {
            Console.WriteLine("");
            Console.WriteLine("ABF Analyzer Arguments:");
            Console.WriteLine(" help text not yet written");
            Console.WriteLine("");
        }

        static void Command(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine($"ERROR: at least 3 arguments are required.");
                Help();
                return;
            }

            string abfFilePath = System.IO.Path.GetFullPath(args[1]);
            if (!System.IO.File.Exists(abfFilePath))
            {
                Console.WriteLine($"ERROR: file does not exist:\n{abfFilePath}");
                return;
            }

            ABF abf = new ABF(abfFilePath);

            string command = args[2].Trim();
            if (command == "trace")
            {
                // TODO: create Figure API and tests before resuming here
            }
            else
            {
                Console.WriteLine($"ERROR: '{command}' is not a valid analysis");
                return;
            }
        }
    }
}
