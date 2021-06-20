using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AbfSharp.ABFFIO
{
    /// <summary>
    /// ABFFIO.DLL wrapper for .NET
    /// </summary>
    public class AbfInterface : IDisposable
    {
        private UInt32 sweepPointCount;
        private readonly UInt32 sweepCount;
        private readonly string path;
        private readonly bool debug;

        public readonly float[] buffer;

        public Structs.ABFFileHeader header;

        [DllImport("ABFFIO.dll", CharSet = CharSet.Ansi)]
        private static extern bool ABF_IsABFFile(String szFileName, ref Int32 pnDataFormat, ref Int32 pnError);

        [DllImport("ABFFIO.dll", CharSet = CharSet.Ansi)]
        private static extern bool ABF_ReadOpen(String szFileName, ref Int32 phFile, UInt32 fFlags, ref Structs.ABFFileHeader pFH, ref UInt32 puMaxSamples, ref UInt32 pdwMaxEpi, ref Int32 pnError);

        public AbfInterface(string abfFilePath, bool hideDebugMessages = true)
        {
            if (!System.IO.File.Exists(abfFilePath))
                throw new ArgumentException($"file does not exist: {abfFilePath}");

            path = System.IO.Path.GetFullPath(abfFilePath);
            debug = !hideDebugMessages;

            // ensure this file is a valid ABF
            Int32 dataFormat = 0;
            Int32 errorCode = 0;
            ABF_IsABFFile(abfFilePath, ref dataFormat, ref errorCode);
            if (errorCode != 0)
                throw new ArgumentException($"ABFFIO says not an ABF file: {abfFilePath}");

            // open the file and read its header
            if (hideDebugMessages) Debug.WriteLine($"OPENING: {abfFilePath}");
            Int32 fileHandle = 0;
            uint loadFlags = 0;
            ABF_ReadOpen(abfFilePath, ref fileHandle, loadFlags, ref header, ref sweepPointCount, ref sweepCount, ref errorCode);
            AbfError.AssertSuccess(errorCode);

            // create the sweep buffer in memory
            buffer = new float[sweepPointCount];
        }

        [DllImport("ABFFIO.dll", CharSet = CharSet.Ansi)]
        private static extern bool ABF_Close(Int32 nFile, ref Int32 pnError);

        public void Dispose()
        {
            Int32 fileHandle = 0;
            Int32 errorCode = 0;
            ABF_Close(fileHandle, ref errorCode);
            AbfError.AssertSuccess(errorCode);
            if (debug) Debug.WriteLine($"{System.IO.Path.GetFileName(path)} closed");
        }

        [DllImport("ABFFIO.dll", CharSet = CharSet.Ansi)]
        private static extern bool ABF_ReadTags(Int32 nFile, ref Structs.ABFFileHeader pFH, UInt32 dwFirstTag, ref Structs.ABFTag pTagArray, UInt32 uNumTags, ref Int32 pnError);

        public Structs.ABFTag[] ReadTags()
        {
            Int32 fileHandle = 0;
            Int32 errorCode = 0;
            Structs.ABFTag[] abfTags = new Structs.ABFTag[(UInt32)header.lNumTagEntries];
            for (uint i = 0; i < abfTags.Length; i++)
            {
                ABF_ReadTags(fileHandle, ref header, i, ref abfTags[i], 1, ref errorCode);
                AbfError.AssertSuccess(errorCode);
            }
            return abfTags;
        }

        [DllImport("ABFFIO.dll", CharSet = CharSet.Ansi)]
        private static extern bool ABF_ReadChannel(Int32 nFile, ref Structs.ABFFileHeader pFH, Int32 nChannel, Int32 dwEpisode, ref float pfBuffer, ref UInt32 puNumSamples, ref Int32 pnError);

        public void ReadChannel(int sweepNumber, int channelNumber)
        {
            if (debug) Debug.WriteLine($"{System.IO.Path.GetFileName(path)} reading Ch{channelNumber} Sw{sweepNumber}");
            Int32 errorCode = 0;
            Int32 fileHandle = 0;
            int physicalChannel = header.nADCSamplingSeq[channelNumber];
            ABF_ReadChannel(fileHandle, ref header, physicalChannel, sweepNumber, ref buffer[0], ref sweepPointCount, ref errorCode);
            AbfError.AssertSuccess(errorCode);
        }

        [DllImport("ABFFIO.dll", CharSet = CharSet.Ansi)]
        private static extern int ABFH_GetEpochDuration(ref Structs.ABFFileHeader pFH, Int32 nChannel, Int32 dwEpisode, Int32 nEpoch);
        public int GetEpochDuration(int channelNumber, int sweepNumber, int epochNumber)
        {
            return ABFH_GetEpochDuration(ref header, channelNumber, sweepNumber, epochNumber);
        }

        [DllImport("ABFFIO.dll", CharSet = CharSet.Ansi)]
        private static extern float ABFH_GetEpochLevel(ref Structs.ABFFileHeader pFH, Int32 nChannel, Int32 dwEpisode, Int32 nEpoch);
        public float GetEpochLevel(int channelNumber, int sweepNumber, int epochNumber)
        {
            return ABFH_GetEpochLevel(ref header, channelNumber, sweepNumber, epochNumber);
        }

        // Return the bounds of a given epoch in a given episode. 
        // Values returned are ZERO relative (not relative to start of sweep)
        [DllImport("ABFFIO.dll", CharSet = CharSet.Ansi)]
        private static extern bool ABFH_GetEpochLimits(ref Structs.ABFFileHeader pFH,
            Int32 nADCChannel, Int32 uDACChannel, Int32 dwEpisode, Int32 nEpoch,
            ref UInt32 puEpochStart, ref UInt32 puEpochEnd, ref Int32 pnError);
        public (bool valid, int start, int end) GetEpochLimits(int channelNumber, int sweepNumber, int epochNumber)
        {
            UInt32 puEpochStart = 0;
            UInt32 puEpochEnd = 0;
            Int32 pnError = 0;
            bool valid = ABFH_GetEpochLimits(ref header,
                channelNumber, channelNumber, sweepNumber, epochNumber,
                ref puEpochStart, ref puEpochEnd, ref pnError);

            return (valid, (int)puEpochStart, (int)puEpochEnd);
        }

        // Get the duration of the first/last holding period.
        [Obsolete("read this using the epoch module")]
        public int GetHoldingLength()
        {
            int nSweepLength = header.lNumSamplesPerEpisode;
            int nNumChannels = header.nADCNumChannels;

            int nHoldingCount = nSweepLength / Structs.ABFH_HOLDINGFRACTION;
            nHoldingCount -= nHoldingCount % nNumChannels;
            if (nHoldingCount < nNumChannels)
                nHoldingCount = nNumChannels;
            return nHoldingCount;
        }
    }
}
