using System;
using System.Collections.Generic;
using System.Text;

namespace AbfSharp
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
}
