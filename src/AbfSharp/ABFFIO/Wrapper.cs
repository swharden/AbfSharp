using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AbfSharp.ABFFIO
{
    /// <summary>
    /// This class provides .NET methods which call ABFFIO.DLL functions under the hood.
    /// </summary>
    public class Wrapper : IDisposable
    {
        private UInt32 SweepPointCount;
        private readonly UInt32 SweepCount;
        private readonly string FilePath;

        public readonly float[] buffer;

        public AbfFileHeader header;

        [DllImport("ABFFIO.dll", CharSet = CharSet.Ansi)]
        private static extern bool ABF_IsABFFile(String szFileName, ref Int32 pnDataFormat, ref Int32 pnError);

        [DllImport("ABFFIO.dll", CharSet = CharSet.Ansi)]
        private static extern bool ABF_ReadOpen(String szFileName, ref Int32 phFile, UInt32 fFlags, ref AbfFileHeader pFH, ref UInt32 puMaxSamples, ref UInt32 pdwMaxEpi, ref Int32 pnError);

        public Wrapper(string abfFilePath, bool hideDebugMessages = true)
        {
            if (!System.IO.File.Exists(abfFilePath))
                throw new ArgumentException($"file does not exist: {abfFilePath}");

            FilePath = System.IO.Path.GetFullPath(abfFilePath);

            // ensure this file is a valid ABF
            Int32 dataFormat = 0;
            Int32 errorCode = 0;
            ABF_IsABFFile(abfFilePath, ref dataFormat, ref errorCode);
            if (errorCode != 0)
                throw new InvalidOperationException($"ABF_IsABFFile() returned error {errorCode} ({(Error)errorCode})");

            // open the file and read its header
            if (hideDebugMessages) Debug.WriteLine($"OPENING: {abfFilePath}");
            Int32 fileHandle = 0;
            uint loadFlags = 0;
            ABF_ReadOpen(abfFilePath, ref fileHandle, loadFlags, ref header, ref SweepPointCount, ref SweepCount, ref errorCode);
            if (errorCode != 0)
                throw new InvalidOperationException($"ABF_ReadOpen() returned error {errorCode} ({(Error)errorCode})");

            // create the sweep buffer in memory
            buffer = new float[SweepPointCount];
        }

        [DllImport("ABFFIO.dll", CharSet = CharSet.Ansi)]
        private static extern bool ABF_Close(Int32 nFile, ref Int32 pnError);

        public void Dispose()
        {
            Int32 fileHandle = 0;
            Int32 errorCode = 0;
            ABF_Close(fileHandle, ref errorCode);
            if (errorCode != 0)
                throw new InvalidOperationException($"ABF_Close() returned error {errorCode} ({(Error)errorCode})");
            Debug.WriteLine($"{System.IO.Path.GetFileName(FilePath)} closed");
        }

        [DllImport("ABFFIO.dll", CharSet = CharSet.Ansi)]
        private static extern bool ABF_ReadTags(Int32 nFile, ref AbfFileHeader pFH, UInt32 dwFirstTag, ref TagStruct pTagArray, UInt32 uNumTags, ref Int32 pnError);

        public TagStruct[] ReadTags()
        {
            Int32 fileHandle = 0;
            Int32 errorCode = 0;
            TagStruct[] abfTags = new TagStruct[(UInt32)header.lNumTagEntries];
            for (uint i = 0; i < abfTags.Length; i++)
            {
                ABF_ReadTags(fileHandle, ref header, i, ref abfTags[i], 1, ref errorCode);
                if (errorCode != 0)
                    throw new InvalidOperationException($"ABF_ReadTags() returned error {errorCode} ({(Error)errorCode})");
            }
            return abfTags;
        }

        [DllImport("ABFFIO.dll", CharSet = CharSet.Ansi)]
        private static extern bool ABF_ReadChannel(Int32 nFile, ref AbfFileHeader pFH, Int32 nChannel, Int32 dwEpisode, ref float pfBuffer, ref UInt32 puNumSamples, ref Int32 pnError);

        public void ReadChannel(int sweepNumber, int channelNumber)
        {
            Debug.WriteLine($"{System.IO.Path.GetFileName(FilePath)} reading Ch{channelNumber} Sw{sweepNumber}");
            Int32 errorCode = 0;
            Int32 fileHandle = 0;
            int physicalChannel = header.nADCSamplingSeq[channelNumber];
            ABF_ReadChannel(fileHandle, ref header, physicalChannel, sweepNumber, ref buffer[0], ref SweepPointCount, ref errorCode);
            if (errorCode != 0)
                throw new InvalidOperationException($"ABF_ReadChannel() returned error {errorCode} ({(Error)errorCode})");
        }

        [DllImport("ABFFIO.dll", CharSet = CharSet.Ansi)]
        private static extern int ABFH_GetEpochDuration(ref AbfFileHeader pFH, Int32 nChannel, Int32 dwEpisode, Int32 nEpoch);
        public int GetEpochDuration(int channelNumber, int sweepNumber, int epochNumber)
        {
            return ABFH_GetEpochDuration(ref header, channelNumber, sweepNumber, epochNumber);
        }

        [DllImport("ABFFIO.dll", CharSet = CharSet.Ansi)]
        private static extern float ABFH_GetEpochLevel(ref AbfFileHeader pFH, Int32 nChannel, Int32 dwEpisode, Int32 nEpoch);
        public float GetEpochLevel(int channelNumber, int sweepNumber, int epochNumber)
        {
            return ABFH_GetEpochLevel(ref header, channelNumber, sweepNumber, epochNumber);
        }

        /// <summary>
        /// Return the bounds of a given epoch in a given episode. 
        /// Values returned are ZERO relative (not relative to start of sweep)
        /// </summary>
        [DllImport("ABFFIO.dll", CharSet = CharSet.Ansi)]
        private static extern bool ABFH_GetEpochLimits(ref AbfFileHeader pFH,
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
    }
}
