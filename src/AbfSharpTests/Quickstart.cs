using NUnit.Framework;
using System;

namespace AbfSharpTests;

public class Quickstart
{
    [Test]
    public void Test_Quickstart()
    {
        string abfPath = SampleData.GetAbfPath("File_axon_5.abf");
        AbfSharp.ABFFIO.ABF abf = new(abfPath);
        float[] sweep = abf.GetSweep(0);
        for (int i = 0; i < 5; i++)
            Console.Write($"{sweep[i]:0.000}, ");
    }
}
