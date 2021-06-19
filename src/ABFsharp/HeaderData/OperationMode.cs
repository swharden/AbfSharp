using System;
using System.Collections.Generic;
using System.Text;

namespace AbfSharp.HeaderData
{
    public enum OperationMode
    {
        /// <summary>
        /// Variable-length sweeps
        /// </summary>
        EventDriven = 1,

        /// <summary>
        /// Loss free (Same as Event-driven, fixed length)
        /// </summary>
        Oscilloscope = 2,

        GapFree = 3,

        OscilloscopeHighSpeed = 4,

        /// <summary>
        /// Clampex only
        /// </summary>
        EpisodicStimulation = 5,
    }
}
