using System;

namespace AbfSharp.ABFFIO;

[Obsolete("use AbfSharp.ABF", true)]
public class ABF : AbfSharp.ABF
{
    public ABF(string filePath, bool preloadSweepData = true) : base(filePath, preloadSweepData)
    {

    }
}
