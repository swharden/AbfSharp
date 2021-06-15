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
        /// Array of ABF tags (e.g., comment tags).
        /// Empty if this ABF has no tags.
        /// </summary>
        public Tag[] Tags => Header.tags;

        /// <summary>
        /// True if the ABF has at least 1 tag
        /// </summary>
        public bool HasTags => Tags.Length > 0;

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
        /// The ABF class reads and ABF file and exposes its data values and header information.
        /// </summary>
        /// <param name="filePath">path to the ABF file</param>
        /// <param name="preload">which sweeps to load upon instantiation</param>
        public ABF(string filePath, Preload preload = Preload.AllSweeps)
        {
            Path = filePath;

            using ABFFIO.AbfInterface abffio = new(filePath);
            Header = new AbfHeader(filePath, abffio);
            //EpochTable = new Epoch.EpochTable(abffio.header);
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

        /// <summary>
        /// Return an array of epochs details for the given channel.
        /// Core epochs (A, B, C, etc.) are flanked by 2 extra epochs which describe the sweep outside the epoch range.
        /// </summary>
        /// <param name="channel">channel index (starts at 0)</param>
        public Epoch[] GetEpochs(int channel = 0)
        {
            List<Epoch> Epochs = new();

            var header = Header.HeaderStruct;

            // add the pre-epoch period as an epoch
            int preEpochPointCount = header.lNumSamplesPerEpisode / ABFFIO.Structs.ABFH_HOLDINGFRACTION;
            preEpochPointCount -= preEpochPointCount % header.nADCNumChannels;
            if (preEpochPointCount < header.nADCNumChannels)
                preEpochPointCount = header.nADCNumChannels;
            Epochs.Add(new Epoch()
            {
                Name = "PRE",
                Type = EpochType.Step,
                Level = header.fDACHoldingLevel[channel],
                Duration = preEpochPointCount,
                IndexFirst = 0,
            });

            // add each epoch in the table
            for (int i = 0; i < ABFFIO.Structs.ABF_EPOCHCOUNT; i++)
            {
                int offset = ABFFIO.Structs.ABF_EPOCHCOUNT * channel;
                Epochs.Add(new Epoch()
                {
                    Name = ((char)(i + 'A')).ToString(),
                    Type = (EpochType)header.nEpochType[i + offset],
                    Level = header.fEpochInitLevel[i + offset],
                    Duration = header.lEpochInitDuration[i + offset],
                    IndexFirst = Epochs.Last().IndexFirst + Epochs.Last().Duration
                });
            }

            // add the post-epoch period as an epoch
            int sweepPointCount = header.lNumSamplesPerEpisode / header.nADCNumChannels;
            Epochs.Add(new Epoch()
            {
                Name = "POST",
                Type = EpochType.Step,
                Level = header.fDACHoldingLevel[channel],
                Duration = sweepPointCount - Epochs.Sum(x => x.Duration),
                IndexFirst = Epochs.Last().IndexFirst + Epochs.Last().Duration
            });

            return Epochs.ToArray();
        }
    }
}
