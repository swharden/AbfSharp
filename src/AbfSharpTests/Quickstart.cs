using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbfSharpTests
{
    class Quickstart
    {
        [Test]
        public void Test_Quickstart_ShowValues()
        {
            string abfPath = SampleData.GetAbfPath("File_axon_5.abf");
            var abf = new AbfSharp.ABFFIO.ABF(abfPath);
            float[] sweep = abf.GetSweep(0);
            for (int i = 0; i < 5; i++)
                Console.Write($"{sweep[i]}, ");
        }
    }
}
