using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AbfSharp
{
    public class ABF
    {
        /// <summary>
        /// Additional information about this ABF
        /// </summary>
        public readonly AbfHeader Header;

        /// <summary>
        /// Sweep data managed in memory
        /// </summary>
        private readonly AbfData Data;

        /// <summary>
        /// Number of sweeps in the ABF.
        /// Gap-free files are considered to have one long sweep.
        /// </summary>
        public int SweepCount => Header.sweepCount;

        /// <summary>
        /// Number of channels in the ABF.
        /// </summary>
        public int ChannelCount => Header.channelCount;

        /// <summary>
        /// Full path to this ABF file.
        /// </summary>
        public readonly string Path;

        /// <summary>
        /// The original filename without the .abf extension.
        /// This identifier is useful for plots titles and base names of analysis files.
        /// </summary>
        public string AbfID => System.IO.Path.GetFileNameWithoutExtension(Path);

        /// <summary>
        /// The epoch table stores information about the DAC (command).
        /// </summary>
        public readonly EpochTable EpochTable;

        /// <summary>
        /// The ABF class reads and ABF file and exposes its data values and header information.
        /// </summary>
        /// <param name="filePath">path to the ABF file</param>
        /// <param name="preload">which sweeps to load upon instantiation</param>
        public ABF(string filePath, Preload preload = Preload.AllSweeps)
        {
            Path = filePath;

            using ABFFIO.AbfInterface abffio = new(filePath);
            Header = new AbfHeader(filePath, abffio);
            EpochTable = new EpochTable(abffio.header);
            Data = new AbfData(Header.sweepCount, Header.channelCount, Header.sweepLengthPoints);

            if (preload == Preload.FirstSweep)
                LoadSweep(abffio, 0, 0);
            else if (preload == Preload.AllSweeps)
                LoadAllSweeps(abffio);
        }

        public override string ToString() => $"ABF {Header.fileName} ({Header.sweepCount} sweeps)";

        private void LoadSweep(ABFFIO.AbfInterface abffio, int sweepIndex, int channelIndex)
        {
            abffio.ReadChannel(sweepIndex + 1, channelIndex);
            var values = new double[abffio.buffer.Length];
            Array.Copy(abffio.buffer, 0, values, 0, abffio.buffer.Length);
            Data.SetValues(sweepIndex, channelIndex, values);
        }

        private void LoadAllSweeps(ABFFIO.AbfInterface abffio)
        {
            for (int sweepIndex = 0; sweepIndex < Header.sweepCount; sweepIndex++)
                for (int channelNumber = 0; channelNumber < Header.channelCount; channelNumber++)
                    LoadSweep(abffio, sweepIndex, channelNumber);
        }

        /// <summary>
        /// Return a single sweep
        /// </summary>
        /// <param name="sweepIndex">sweep (starting at 0)</param>
        /// <param name="channelIndex">channel (starting at 0)</param>
        /// <returns>Sweep class (with ADC data in Values)</returns>
        public Sweep GetSweep(int sweepIndex, int channelIndex = 0)
        {
            if (sweepIndex < 0)
                sweepIndex = Header.sweepCount + sweepIndex;

            if (Data.HasValues(sweepIndex, channelIndex) == false)
                using (var abffio = new ABFFIO.AbfInterface(Header.filePath))
                    LoadSweep(abffio, sweepIndex, channelIndex);

            double[] values = Data.GetValues(sweepIndex, channelIndex);

            // TODO: support variable length sweeps https://swharden.com/pyabf/abf2-file-format/#synch-array
            double sweepLengthSeconds = Header.sampleRate * Header.sweepCount;
            return new Sweep(values, Header.sampleRate, sweepIndex * sweepLengthSeconds);
        }

        /// <summary>
        /// Return a single large sweep containing all data from the ABF
        /// </summary>
        /// <param name="channelIndex">channel (starting at 0)</param>
        /// <returns>Sweep class (with ADC data in Values)</returns>
        public Sweep GetFullRecording(int channelIndex = 0)
        {
            for (int sweepIndex = 0; sweepIndex < Header.sweepCount; sweepIndex++)
                if (Data.HasValues(sweepIndex, channelIndex) == false)
                    using (var abffio = new ABFFIO.AbfInterface(Header.filePath))
                        LoadSweep(abffio, sweepIndex, channelIndex);

            return new Sweep(Data.GetAllValues(channelIndex), Header.sampleRate, 0);
        }
    }
}
