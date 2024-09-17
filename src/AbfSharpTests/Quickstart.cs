namespace AbfSharpTests;

public class Quickstart
{
    [Test]
    public void Test_Quickstart()
    {
        string abfPath = SampleData.GetAbfPath("File_axon_5.abf");
        ABF abf = new(abfPath);
        Sweep sweep = abf.GetSweep(0);

        for (int i = 0; i < 5; i++)
            Console.Write($"{sweep.Values[i]:0.000}, ");
    }
}
