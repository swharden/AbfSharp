using System;
using System.Collections.Generic;
using System.Text;

namespace AbfSharp.AbfReader
{
    public struct Section
    {
        public readonly long FirstByte;
        public readonly long Size;
        public readonly long Count;
        public Section(long fb, long sz, long ct) => (FirstByte, Size, Count) = (fb, sz, ct);
    }
}
