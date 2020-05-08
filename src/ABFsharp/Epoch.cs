using ABFsharp.ABFFIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ABFsharp
{
    public enum EpochType
    {
        Off = 0,
        Step = 1,
        Ramp = 2,
        Pulse = 3,
        Triangle = 4,
        Cosine = 5,
        //Unknown = 6,
        Biphasic = 7,
    }

    public struct Epoch
    {
        public string Name;
        public EpochType Type;
        public double level;
        public double levelDelta;
        public int duration;
        public int durationDelta;
        public byte digital;
        public int trainRateHz;
        public int pulseWidthMsec;
        public int indexFirst;
        public int indexLast { get { return indexFirst + duration; } }
        public bool isValid { get { return Type != EpochType.Off && duration > 0; } }

        public override string ToString()
        {
            return $"Epoch '{Name}' ({Type}) {duration} points at {level}";
        }
    }

    public class EpochTable
    {
        public readonly List<Epoch> Epochs = new List<Epoch>();

        public EpochTable(Structs.ABFFileHeader header, int channel = 0)
        {
            // add the pre-epoch period as an epoch
            int preEpochPointCount = header.lNumSamplesPerEpisode / Structs.ABFH_HOLDINGFRACTION;
            preEpochPointCount -= preEpochPointCount % header.nADCNumChannels;
            if (preEpochPointCount < header.nADCNumChannels)
                preEpochPointCount = header.nADCNumChannels;
            Epochs.Add(new Epoch()
            {
                Name = "PRE",
                Type = EpochType.Step,
                level = header.fDACHoldingLevel[channel],
                duration = preEpochPointCount,
                indexFirst = 0,
            });


            // add each epoch in the table
            for (int i = 0; i < Structs.ABF_EPOCHCOUNT; i++)
            {
                int offset = Structs.ABF_EPOCHCOUNT * channel;
                Epochs.Add(new Epoch()
                {
                    Name = ((char)(i + 'A')).ToString(),
                    Type = (EpochType)header.nEpochType[i + offset],
                    level = header.fEpochInitLevel[i + offset],
                    duration = header.lEpochInitDuration[i + offset],
                    indexFirst = Epochs.Last().indexFirst + Epochs.Last().duration
                });
            }

            // add the post-epoch period as an epoch
            int sweepPointCount = header.lNumSamplesPerEpisode / header.nADCNumChannels;
            Epochs.Add(new Epoch()
            {
                Name = "POST",
                Type = EpochType.Step,
                level = header.fDACHoldingLevel[channel],
                duration = sweepPointCount - Epochs.Sum(x => x.duration),
                indexFirst = Epochs.Last().indexFirst + Epochs.Last().duration
            });
        }

        public void Display()
        {
            foreach (var epoch in Epochs)
                if (epoch.Type != EpochType.Off && epoch.duration > 0)
                    Debug.WriteLine(epoch);
        }
    }
}
