using System;
using System.Diagnostics;

namespace ABFsharp
{
    public class ABF
    {
        public AbfInfo info;
        public Sweep sweep; // TODO: needs more things

        private readonly Sweep[,] sweeps;

        public int sweepsInMemory { get; private set; }

        public enum Preload { AllSweeps, FirstSweep, HeaderOnly }

        public ABF(string filePath, Preload preload = Preload.AllSweeps)
        {
            using (var abffio = new ABFFIO.AbfInterface(filePath))
            {
                info = new AbfInfo(filePath, abffio.header);
                sweeps = new Sweep[info.sweepCount, info.channelCount];

                LoadTags(abffio);

                if (preload == Preload.AllSweeps)
                    LoadAllSweeps(abffio);
                else if (preload == Preload.FirstSweep)
                    LoadSweep(abffio, 0, 0);
            }
        }

        public override string ToString()
        {
            if (sweep is null)
                return $"ABF [{info.fileName}] with {info.sweepCount} sweeps";
            else
                return $"ABF [{info.fileName}] set to sweep {sweep.number} of {info.sweepCount}";
        }

        #region loading data from ABF using DLL

        private void LoadSweep(ABFFIO.AbfInterface abffio, int sweepIndex = 0, int channelIndex = 0)
        {
            abffio.ReadChannel(sweepIndex + 1, channelIndex);
            var thisSweep = new Sweep(info, sweepIndex, channelIndex);
            thisSweep.values = new double[abffio.buffer.Length];
            Array.Copy(abffio.buffer, 0, thisSweep.values, 0, abffio.buffer.Length);
            sweeps[sweepIndex, channelIndex] = thisSweep;
            sweepsInMemory += 1;
        }

        private void LoadAllSweeps(ABFFIO.AbfInterface abffio)
        {
            for (int sweepIndex = 0; sweepIndex < info.sweepCount; sweepIndex++)
                for (int channelNumber = 0; channelNumber < info.channelCount; channelNumber++)
                    LoadSweep(abffio, sweepIndex, channelNumber);
        }

        private void LoadTags(ABFFIO.AbfInterface abffio)
        {
            ABFFIO.Structs.ABFTag[] abfTags = abffio.ReadTags();
            for (int i = 0; i < abfTags.Length; i++)
            {
                ABFFIO.Structs.ABFTag abfTag = abfTags[i];
                double timeSec = abfTag.lTagTime * abffio.header.fSynchTimeUnit / 1e6;
                string comment = new string(abfTag.sComment).Trim();
                int timeSweep = (int)(timeSec / info.sweepIntervalSec);
                Tag tag = new Tag(timeSec, timeSweep, comment, abfTag.nTagType);
                info.tags[i] = tag;
            }
        }

        #endregion

        public Sweep GetSweep(int index = 0, int channel = 0)
        {
            if (sweeps[index, channel] is null)
                using (var abffio = new ABFFIO.AbfInterface(info.filePath))
                    LoadSweep(abffio, index, channel);

            return sweeps[index, channel];
        }

        public double[] GetFullRecording(int channel = 0)
        {
            double[] data = new double[info.sweepLengthPoints * info.sweepCount];
            for (int sweepIndex = 0; sweepIndex < info.sweepCount; sweepIndex++)
            {
                var sweep = GetSweep(sweepIndex, channel);
                Array.Copy(sweep.values, 0, data, sweepIndex * info.sweepLengthPoints, info.sweepLengthPoints);
            }
            return data;
        }
    }
}
