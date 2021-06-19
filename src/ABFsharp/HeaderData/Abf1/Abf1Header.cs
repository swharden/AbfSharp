using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.HeaderData.Abf1
{
    public class Abf1Header
    {
        const int ABF_ADCCOUNT = 16;

        // Group 1
        public readonly int lFileSignature;
        public readonly float fFileVersionNumber;
        public readonly int nOperationMode;
        public readonly int lActualAcqLength;
        public readonly int nNumPointsIgnored;
        public readonly int lActualEpisodes;
        public readonly int lFileStartDate;
        public readonly int lFileStartTime;
        public readonly int lStopwatchTime;
        public readonly float fHeaderVersionNumber;
        public readonly int nFileType;
        public readonly int nMSBinFormat;

        // Group 2
        public readonly Int32 lDataSectionPtr;
        public readonly Int32 lTagSectionPtr;
        public readonly Int32 lNumTagEntries;
        public readonly Int32 lSynchArrayPtr;
        public readonly Int32 lSynchArraySize;
        public readonly Int16 nDataFormat;

        // Group 3
        public readonly Int16 nADCNumChannels;
        public readonly Single fADCSampleInterval;
        public readonly Single fSynchTimeUnit;
        public readonly Int32 lNumSamplesPerEpisode;
        public readonly Int32 lPreTriggerSamples;
        public readonly Int32 lEpisodesPerRun;

        // Group 5
        public readonly Single fADCRange;
        public readonly Single fDACRange;
        public readonly Int32 lADCResolution;
        public readonly Int32 lDACResolution;

        // Group 6 - Environmental Information (118 bytes)
        public readonly Int16 nExperimentType;
        public readonly string sCreatorInfo;
        public readonly string sFileCommentOld;
        public readonly Int16 nFileStartMillisecs;

        // Group 6 Extended - Environmental Information  (898 bytes)
        /*
        public readonly Int16[] nTelegraphEnable;
        public readonly Int16[] nTelegraphInstrument;
        public readonly Single[] fTelegraphAdditGain;
        public readonly Single[] fTelegraphFilter;
        public readonly Single[] fTelegraphMembraneCap;
        public readonly Int16[] nTelegraphMode;
        public readonly Int16[] nTelegraphDACScaleFactorEnable;
        */

        // Group 7
        public readonly Int16[] nADCPtoLChannelMap = new Int16[16];
        public readonly Int16[] nADCSamplingSeq = new Int16[16];
        public readonly string[] sADCChannelName = new string[16]; // 10 chars
        public readonly string[] sADCUnits = new string[16]; // 8 chars
        public readonly float[] fADCProgrammableGain = new float[16];
        public readonly float[] fInstrumentScaleFactor = new float[16];
        public readonly float[] fInstrumentOffset = new float[16];
        public readonly float[] fSignalGain = new float[16];
        public readonly float[] fSignalOffset = new float[16];
        public readonly string[] sDACChannelName = new string[4]; // 10 chars
        public readonly string[] sDACChannelUnit = new string[4]; // 8 chars

        // Group 18
        public readonly byte[] uFileGUID;

        public Abf1Header(BinaryReader reader)
        {
            // https://docs.python.org/3/library/struct.html#format-characters
            // https://github.com/swharden/pyABF/blob/master/src/pyabf/abfHeader.py
            // "h" = 2-byte int
            // "H" = 2-byte uint
            // "i" = 4-byte int
            // "I" = 4-byte uint
            // "l" = 4-byte int
            // "L" = 4-byte uint
            // "f" = 4-byte float

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            lFileSignature = reader.ReadInt32();
            fFileVersionNumber = reader.ReadSingle();
            nOperationMode = reader.ReadInt16();
            lActualAcqLength = reader.ReadInt16();
            nNumPointsIgnored = reader.ReadInt16();
            lActualEpisodes = reader.ReadInt32();
            lFileStartDate = reader.ReadInt32();
            lFileStartTime = reader.ReadInt32();
            lStopwatchTime = reader.ReadInt16();
            fHeaderVersionNumber = reader.ReadSingle();
            nFileType = reader.ReadInt16();
            nMSBinFormat = reader.ReadInt16();

            // GROUP 2 - File Structure (78 bytes)
            reader.BaseStream.Seek(40, SeekOrigin.Begin);
            lDataSectionPtr = reader.ReadInt32();
            lTagSectionPtr = reader.ReadInt32();
            lNumTagEntries = reader.ReadInt32();

            reader.BaseStream.Seek(92, SeekOrigin.Begin);
            lSynchArrayPtr = reader.ReadInt32();
            lSynchArraySize = reader.ReadInt32();
            nDataFormat = reader.ReadInt16();

            // GROUP 3 - Trial hierarchy information (82 bytes)
            reader.BaseStream.Seek(120, SeekOrigin.Begin);
            nADCNumChannels = reader.ReadInt16();
            fADCSampleInterval = reader.ReadSingle();

            reader.BaseStream.Seek(130, SeekOrigin.Begin);
            fSynchTimeUnit = reader.ReadSingle();

            reader.BaseStream.Seek(138, SeekOrigin.Begin);
            lNumSamplesPerEpisode = reader.ReadInt32();
            lPreTriggerSamples = reader.ReadInt32();
            lEpisodesPerRun = reader.ReadInt32();

            // GROUP 4 - Display Parameters (44 bytes)

            // GROUP 5 - Hardware information (16 bytes)
            reader.BaseStream.Seek(244, SeekOrigin.Begin);
            fADCRange = reader.ReadSingle();
            fDACRange = reader.ReadSingle();
            lADCResolution = reader.ReadInt32();
            lDACResolution = reader.ReadInt32();

            // GROUP 6 - Environmental Information (118 bytes)
            reader.BaseStream.Seek(294, SeekOrigin.Begin);
            sCreatorInfo = new string(reader.ReadChars(15)).Replace("\0", "").Trim();
            reader.ReadChars(1);
            sFileCommentOld = new string(reader.ReadChars(56)).Trim();
            nFileStartMillisecs = reader.ReadInt16();

            // GROUP 7 - Multi-channel information (1044 bytes)

            reader.BaseStream.Seek(378, SeekOrigin.Begin);

            for (int i = 0; i < 16; i++)
                nADCPtoLChannelMap[i] = reader.ReadInt16();

            for (int i = 0; i < 16; i++)
                nADCSamplingSeq[i] = reader.ReadInt16();

            for (int i = 0; i < 16; i++)
                sADCChannelName[i] = string.Join("", reader.ReadChars(10));

            for (int i = 0; i < 16; i++)
                sADCUnits[i] = string.Join("", reader.ReadChars(8));

            for (int i = 0; i < 16; i++)
                fADCProgrammableGain[i] = reader.ReadSingle();

            // missing

            reader.BaseStream.Seek(922, SeekOrigin.Begin);

            for (int i = 0; i < 16; i++)
                fInstrumentScaleFactor[i] = reader.ReadSingle();

            for (int i = 0; i < 16; i++)
                fInstrumentOffset[i] = reader.ReadSingle();

            for (int i = 0; i < 16; i++)
                fSignalGain[i] = reader.ReadSingle();

            for (int i = 0; i < 16; i++)
                fSignalOffset[i] = reader.ReadSingle();

            for (int i = 0; i < 4; i++)
                sDACChannelName[i] = string.Join("", reader.ReadChars(10));

            for (int i = 0; i < 4; i++)
                sDACChannelUnit[i] = string.Join("", reader.ReadChars(8));

            // GROUP 18 - Application version data (16 bytes)
            reader.BaseStream.Seek(5282, SeekOrigin.Begin);
            uFileGUID = reader.ReadBytes(16);
        }

        private static Int16[] ReadArrayInt16(BinaryReader reader, int size)
        {
            Int16[] arr = new Int16[size];
            for (int i = 0; i < size; i++)
                arr[i] = reader.ReadInt16();
            return arr;
        }

        private static Single[] ReadArraySingle(BinaryReader reader, int size)
        {
            Single[] arr = new Single[size];
            for (int i = 0; i < size; i++)
                arr[i] = reader.ReadSingle();
            return arr;
        }
    }
}
