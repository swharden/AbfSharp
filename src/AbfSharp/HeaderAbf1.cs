using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AbfSharp
{
    public class HeaderAbf1 : HeaderBase
    {
        public HeaderAbf1(BinaryReader reader, string filePath)
        {
            Path = System.IO.Path.GetFullPath(filePath);
            Read(reader);
        }

        public HeaderAbf1(string filePath)
        {
            Path = System.IO.Path.GetFullPath(filePath);
            using Stream fs = File.Open(filePath, FileMode.Open);
            using BinaryReader reader = new(fs);
            Read(reader);
        }

        private void Read(BinaryReader reader)
        {
            ReadGroup1(reader);
            ReadGroup2(reader);
            ReadGroup3(reader);
            ReadGroup5(reader);
        }

        private void ReadGroup1(BinaryReader reader)
        {
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            SignatureBytes = reader.ReadBytes(4);
            fFileVersionNumber = reader.ReadSingle();
            nOperationMode = reader.ReadInt16();
            lActualAcqLength = reader.ReadInt32();
            nNumPointsIgnored = reader.ReadInt16();
            lActualEpisodes = reader.ReadInt32();

            // fix Y2K bug
            int datecode = reader.ReadInt32();
            int day = datecode % 100;
            datecode /= 100;
            int month = datecode % 100;
            datecode /= 100;
            int year = datecode;
            if (year < 80)
                year += 2000;
            if (year < 100)
                year += 1900;
            uFileStartDate = (uint)(year) * 10000 + (uint)month * 100 + (uint)day;

            uFileStartTimeMS = (uint)reader.ReadInt32() * 1000;
            lStopwatchTime = reader.ReadInt32();

            // actually the header version number is just the file version number
            fHeaderVersionNumber = reader.ReadSingle();
            fHeaderVersionNumber = fFileVersionNumber;

            nFileType = reader.ReadInt16();

            // Add nFileStartMillisecs from group 6
            reader.BaseStream.Seek(366, SeekOrigin.Begin);
            uFileStartTimeMS += (uint)reader.ReadInt16();
        }

        private void ReadGroup2(BinaryReader reader)
        {
            reader.BaseStream.Seek(40, SeekOrigin.Begin);
            lDataSectionPtr = reader.ReadInt32();
            lTagSectionPtr = reader.ReadInt32();
            lNumTagEntries = reader.ReadInt32();
            lScopeConfigPtr = reader.ReadInt32();
            lNumScopes = reader.ReadInt32();
            lNumDeltas = reader.ReadInt32();
            lVoiceTagPtr = reader.ReadInt32();
            lVoiceTagEntries = reader.ReadInt32();

            reader.BaseStream.Seek(92, SeekOrigin.Begin);
            lSynchArrayPtr = reader.ReadInt32();
            lSynchArraySize = reader.ReadInt32();
            nDataFormat = reader.ReadInt16();
            nSimultaneousScan = reader.ReadInt16();
            lStatisticsConfigPtr = reader.ReadInt32();
            lAnnotationSectionPtr = reader.ReadInt32();

            /*
            reader.BaseStream.Seek(2048, SeekOrigin.Begin);

            lDACFilePtr = new int[8];
            for (int i = 0; i < 2; i++)
                lDACFilePtr[i] = reader.ReadInt32();

            lDACFileNumEpisodes = new int[8];
            for (int i = 0; i < 2; i++)
                lDACFileNumEpisodes[i] = reader.ReadInt32();
            */
        }

        private void ReadGroup3(BinaryReader reader)
        {
            reader.BaseStream.Seek(120, SeekOrigin.Begin);
            nADCNumChannels = reader.ReadInt16();
            float fADCSampleInterval = reader.ReadSingle();
            fADCSequenceInterval = fADCSampleInterval * nADCNumChannels;
            float fADCSecondSampleInterval = reader.ReadSingle();
            fSynchTimeUnit = reader.ReadSingle();
            fSecondsPerRun = reader.ReadSingle();
            lNumSamplesPerEpisode = reader.ReadInt32();
            lPreTriggerSamples = reader.ReadInt32();
            lEpisodesPerRun = reader.ReadInt32();
            lRunsPerTrial = reader.ReadInt32();
            lNumberOfTrials = reader.ReadInt32();
            nAveragingMode = reader.ReadInt16();
            nUndoRunCount = reader.ReadInt16();
            nFirstEpisodeInRun = reader.ReadInt16();
            fTriggerThreshold = reader.ReadSingle();
            nTriggerSource = reader.ReadInt16();
            nTriggerAction = reader.ReadInt16();
            nTriggerPolarity = reader.ReadInt16();
            fScopeOutputInterval = reader.ReadSingle();
            fEpisodeStartToStart = reader.ReadSingle();
            fRunStartToStart = reader.ReadSingle();
            fTrialStartToStart = reader.ReadSingle();
            lAverageCount = reader.ReadInt32();
            lLegacyClockChange = reader.ReadInt32();
            nAutoTriggerStrategy = reader.ReadInt16();
        }

        public void ReadGroup5(BinaryReader reader)
        {
            reader.BaseStream.Seek(244, SeekOrigin.Begin);
            fADCRange = reader.ReadSingle();
            fDACRange = reader.ReadSingle();
            lADCResolution = reader.ReadInt32();
            lDACResolution = reader.ReadInt32();
        }
    }
}
