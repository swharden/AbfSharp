using System;

namespace ABFsharp
{
    public class ABF : IDisposable
    {

        private ABFFIO.Interface abffio;
        public AbfInfo info;
        public Sweep sweep;
        public double[,,] data; // sweep, position, channel
        private bool[,] dataLoaded; // sweep, channel

        public ABF(string filePath, bool preLoadSweepData = true)
        {
            abffio = new ABFFIO.Interface(filePath);
            info = new AbfInfo(filePath, abffio.header);
            sweep = new Sweep(info);
            ReadTags();

            if (preLoadSweepData)
            {
                LoadAllSweeps();
                SetSweep(0, 0);
            }

        }

        public void Dispose()
        {
            Close();
        }

        public override string ToString()
        {
            return $"ABF ({info.fileName}) set to sweep X of {info.sweepCount}";
        }

        public void Close()
        {
            abffio.Close();
        }

        private void ReadTags()
        {
            // populates info.tags from data retrieved from abffio module
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

        private void EnsureDataArrayExists()
        {
            if (data == null)
            {
                data = new double[info.sweepCount, info.sweepLengthPoints, info.channelCount];
                dataLoaded = new bool[info.sweepCount, info.channelCount];
            }
        }

        private void LoadSweep(int sweep, int channel = 0)
        {
            EnsureDataArrayExists();
            if (!dataLoaded[sweep, channel])
            {
                abffio.ReadChannel(sweep + 1, channel);
                for (int i = 0; i < info.sweepLengthPoints; i++)
                    data[sweep, i, channel] = abffio.sweepBuffer[i];
                dataLoaded[sweep, channel] = true;
            }
        }

        public void LoadAllSweeps()
        {
            EnsureDataArrayExists();
            for (int channel = 0; channel < info.channelCount; channel++)
                for (int sweep = 0; sweep < info.sweepCount; sweep++)
                    LoadSweep(sweep, channel);
        }

        public void SetSweep(int sweepNumber = 0, int channelNumber = 0)
        {
            EnsureDataArrayExists();
            if (!dataLoaded[sweepNumber, channelNumber])
                LoadSweep(sweepNumber, channelNumber);

            for (int i = 0; i < info.sweepLengthPoints; i++)
                sweep.values[i] = data[sweepNumber, i, channelNumber];
        }
    }
}
