using System;
using System.Collections.Generic;
using System.Text;

namespace AbfSharp
{
    public class AbfVersion
    {
        public int major;
        public int minor;
        public int bugfix;

        public AbfVersion(int major, int minor, int bugfix)
        {
            this.major = major;
            this.minor = minor;
            this.bugfix = bugfix;
        }

        public override string ToString()
        {
            return $"{major}.{minor}.{bugfix}";
        }
    }
}
