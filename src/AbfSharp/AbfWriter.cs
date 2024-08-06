using System.Text;

namespace AbfSharp;

public class AbfWriter(double sampleRate = 20000, string units = "pA")
{
    List<double[]> SweepValues { get; } = [];
    double SampleRate { get; } = sampleRate;
    double UsPerSample => 1e6 / SampleRate;
    string Units { get; } = units;

    public void AddSweep(double[] sweep)
    {
        if ((SweepValues.Count > 0) && (SweepValues.First().Length != sweep.Length))
        {
            throw new InvalidOperationException("all sweeps must have equal length");
        }

        SweepValues.Add(sweep);
    }

    public void Save(string filePath)
    {
        if (SweepValues.Count == 0)
        {
            throw new InvalidOperationException($"Call ${nameof(AddSweep)}() to add data before saving");
        }

        const int BLOCKSIZE = 512;
        const int HEADER_BLOCKS = 4;

        int sweepCount = SweepValues.Count;
        int sweepPointCount = SweepValues.First().Length;

        int dataPointCount = sweepPointCount * sweepCount;

        int bytesPerPoint = 2;
        int dataBlocks = dataPointCount * bytesPerPoint / BLOCKSIZE + 1;
        int fileBlockCount = HEADER_BLOCKS + dataBlocks;
        int fileSize = fileBlockCount * BLOCKSIZE;
        byte[] bytes = new byte[fileSize];

        Array.Copy(Encoding.ASCII.GetBytes("ABF "), 0, bytes, 0, 4); // fFileSignature
        Array.Copy(BitConverter.GetBytes((float)1.3), 0, bytes, 4, sizeof(float)); // fFileVersionNumber
        Array.Copy(BitConverter.GetBytes((UInt16)5), 0, bytes, 8, sizeof(UInt16)); // nOperationMode (5 is episodic)
        Array.Copy(BitConverter.GetBytes((UInt32)dataPointCount), 0, bytes, 10, sizeof(UInt32)); // lActualAcqLength
        Array.Copy(BitConverter.GetBytes((UInt32)sweepCount), 0, bytes, 16, sizeof(UInt32)); // lActualEpisodes
        Array.Copy(BitConverter.GetBytes((UInt32)HEADER_BLOCKS), 0, bytes, 40, sizeof(UInt32)); // lDataSectionPtr
        Array.Copy(BitConverter.GetBytes((UInt16)0), 0, bytes, 100, sizeof(UInt16)); // nDataFormat (0 for Int16)
        Array.Copy(BitConverter.GetBytes((UInt16)1), 0, bytes, 120, sizeof(UInt16)); // nADCNumChannels
        Array.Copy(BitConverter.GetBytes((float)UsPerSample), 0, bytes, 122, sizeof(float)); // fADCSampleInterval
        Array.Copy(BitConverter.GetBytes((UInt32)sweepPointCount), 0, bytes, 138, sizeof(UInt32)); // lNumSamplesPerEpisode

        // set the scaling factor to be the biggest allowable to accommodate the data
        double maxValue = SweepValues.Select(x => x.Max()).Max();
        double minValue = SweepValues.Select(x => x.Min()).Min();
        double maxAbsValue = Math.Max(Math.Abs(maxValue), Math.Abs(minValue));
        double fInstrumentScaleFactor = 100;
        double fADCRange = 10;
        double lADCResolution = 1 << 15;
        double valueScale = double.NaN;
        for (int i = 0; i < 10; i++)
        {
            fInstrumentScaleFactor /= 10;
            valueScale = lADCResolution / fADCRange * fInstrumentScaleFactor;
            double maxDeviationFromZero = 32767 / valueScale;
            if (maxDeviationFromZero >= maxAbsValue)
                break;
        }

        Array.Copy(BitConverter.GetBytes((UInt32)lADCResolution), 0, bytes, 252, sizeof(UInt32)); // lADCResolution
        Array.Copy(BitConverter.GetBytes((float)fADCRange), 0, bytes, 244, sizeof(float)); // fADCRange
        for (int i = 0; i < 16; i++)
        {
            Array.Copy(BitConverter.GetBytes((float)fInstrumentScaleFactor), 0, bytes, 922 + i * 4, sizeof(float));
            Array.Copy(BitConverter.GetBytes((float)1), 0, bytes, 1050 + i * 4, sizeof(float)); // fSignalGain
            Array.Copy(BitConverter.GetBytes((float)1), 0, bytes, 730 + i * 4, sizeof(float)); // fADCProgrammableGain
            Array.Copy(Encoding.ASCII.GetBytes(Units.PadRight(8)), 0, bytes, 602 + i * 8, 8);
        }

        int dataByteOffset = BLOCKSIZE * HEADER_BLOCKS;
        for (int i = 0; i < SweepValues.Count; i++)
        {
            double[] sweep = SweepValues[i];
            int sweepByteOffset = i * sweepPointCount * bytesPerPoint;
            for (int j = 0; j < sweep.Length; j++)
            {
                int valueByteOffset = j * bytesPerPoint;
                int bytePosition = dataByteOffset + sweepByteOffset + valueByteOffset;
                Array.Copy(BitConverter.GetBytes((UInt16)(sweep[j] * valueScale)), 0, bytes, bytePosition, sizeof(UInt16));
            };
        }

        File.WriteAllBytes(filePath, bytes);
    }
}
