using System;
using System.Collections.Generic;
using System.Text;

namespace AbfSharp
{
    public struct Epoch
    {
        public string Name;
        public EpochType Type;
        public double Level;
        public double LevelDelta;
        public int Duration;
        public int DurationDelta;
        public byte Digital;
        public int TrainRateHz;
        public int PulseWidthMsec;
        public int IndexFirst;

        public override string ToString()
        {
            return $"Epoch '{Name}' ({Type}) {Duration} points at {Level}";
        }
    }
}
