using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ABFsharp
{
    public class ABF
    {
        public enum Preload { AllSweeps, FirstSweep, HeaderOnly }

        private AbfHeader header;
        private AbfData abfData;

        public int sweepsInMemory { get { return abfData.sweepsInMemory; } }
        public readonly double loadTimeMilliseconds;

        // bring in a few useful header values
        public readonly int sweepCount;
        public readonly int channelCount;

        public readonly string abfID;
        public readonly string protocol;
        public readonly string protocolPath;

        public ABF(string filePath, Preload preload = Preload.AllSweeps)
        {
            using (var abffio = new ABFFIO.AbfInterface(filePath))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                header = new AbfHeader(filePath, abffio);
                sweepCount = header.sweepCount;
                channelCount = header.channelCount;

                abfID = System.IO.Path.GetFileNameWithoutExtension(filePath);
                protocol = header.protocol;
                protocolPath = header.protocolFilePath;

                abfData = new AbfData(header.sweepCount, header.channelCount, header.sweepLengthPoints);

                if (preload == Preload.FirstSweep)
                    LoadSweep(abffio, 0, 0);
                else if (preload == Preload.AllSweeps)
                    LoadAllSweeps(abffio);

                stopwatch.Stop();
                loadTimeMilliseconds = (double)stopwatch.ElapsedTicks / Stopwatch.Frequency * 1000;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"ABF [{header.fileName}] with {header.sweepCount} sweeps");

            int pointsInMemory = sweepsInMemory * header.sweepLengthPoints;
            double memoryUsedMB = pointsInMemory * 8 / 1e6f;

            if (sweepsInMemory == 0)
                sb.Append(" (none in memory)");
            else if (sweepsInMemory == header.sweepCount)
                sb.Append($" (all in memory, ~{memoryUsedMB:N} MB)");
            else
                sb.Append($" ({sweepsInMemory} in memory, ~{memoryUsedMB:N0} MB)");

            sb.Append($" loaded in {loadTimeMilliseconds:N} ms");
            return sb.ToString();
        }

        private void LoadSweep(ABFFIO.AbfInterface abffio, int sweepIndex, int channelIndex)
        {
            abffio.ReadChannel(sweepIndex + 1, channelIndex);
            var values = new double[abffio.buffer.Length];
            Array.Copy(abffio.buffer, 0, values, 0, abffio.buffer.Length);
            abfData.SetValues(sweepIndex, channelIndex, values);
        }

        private void LoadAllSweeps(ABFFIO.AbfInterface abffio)
        {
            for (int sweepIndex = 0; sweepIndex < header.sweepCount; sweepIndex++)
                for (int channelNumber = 0; channelNumber < header.channelCount; channelNumber++)
                    LoadSweep(abffio, sweepIndex, channelNumber);
        }

        public Trace GetSweep(int sweepIndex, int channelIndex = 0)
        {
            if (sweepIndex < 0)
                sweepIndex = header.sweepCount + sweepIndex;

            if (abfData.HasValues(sweepIndex, channelIndex) == false)
                using (var abffio = new ABFFIO.AbfInterface(header.filePath))
                    LoadSweep(abffio, sweepIndex, channelIndex);

            return new Trace()
            {
                values = abfData.GetValues(sweepIndex, channelIndex),
                sampleRate = header.sampleRate
            };
        }

        public Trace GetFullRecording(int channelIndex = 0)
        {
            for (int sweepIndex = 0; sweepIndex < header.sweepCount; sweepIndex++)
                if (abfData.HasValues(sweepIndex, channelIndex) == false)
                    using (var abffio = new ABFFIO.AbfInterface(header.filePath))
                        LoadSweep(abffio, sweepIndex, channelIndex);

            return new Trace()
            {
                values = abfData.GetAllValues(channelIndex),
                sampleRate = header.sampleRate
            };
        }

        public string GetHeaderDescription()
        {
            return header.GetDescription();
        }
    }
}
