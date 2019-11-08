using System;
using System.Collections.Generic;
using System.Text;

namespace ABFsharp
{
    public class Tag
    {
        public double timeSec;
        int sweep;
        public string comment;
        public int type;

        public Tag(double timeSec, int sweep, string comment, int type)
        {
            this.timeSec = timeSec;
            this.sweep = sweep;
            this.comment = comment;
            this.type = type;
        }

        public override string ToString()
        {
            return $"Tag type {type} at {timeSec} sec (sweep {sweep}): \"{comment}\"";
        }
    }
}
