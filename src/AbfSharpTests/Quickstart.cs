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
        public void Test_Quickstart_Official()
        {
            string abfPath = SampleData.GetAbfPath("File_axon_5.abf");
            var abf = new AbfSharp.ABFFIO.ABF(abfPath);
            float[] sweep = abf.GetSweep(0);
            for (int i = 0; i < 5; i++)
                Console.Write($"{sweep[i]:0.000}, ");
        }
        [Test]
        public void Test_Quickstart_Native()
        {
            string abfPath = SampleData.GetAbfPath("File_axon_5.abf");
            var abf = new AbfSharp.ABF(abfPath);
            float[] sweep = abf.GetSweep(0);
            for (int i = 0; i < 5; i++)
                Console.Write($"{sweep[i]:0.000}, ");
        }
    }
}
