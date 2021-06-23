using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AbfSharp
{
    public class HeaderAbf1 : Header
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
            ReadGroup6(reader);
            ReadGroup7(reader);
            ReadGroup9(reader);
            ReadGroup10(reader);
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
            int lLegacyClockChange = reader.ReadInt32();
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

        public void ReadGroup6(BinaryReader reader)
        {
            reader.BaseStream.Seek(260, SeekOrigin.Begin);
            nExperimentType = reader.ReadInt16();

            reader.BaseStream.Seek(280, SeekOrigin.Begin);
            nManualInfoStrategy = reader.ReadInt16();
            fCellID1 = reader.ReadSingle();
            fCellID2 = reader.ReadSingle();
            fCellID3 = reader.ReadSingle();

            reader.BaseStream.Seek(294, SeekOrigin.Begin);
            sCreatorInfo = new string(reader.ReadChars(15)).Replace("\0", "").Trim();

            reader.BaseStream.Seek(4512, SeekOrigin.Begin);
            nTelegraphEnable = ReadArrayInt16(reader, 16);
            nTelegraphInstrument = ReadArrayInt16(reader, 16);
            fTelegraphAdditGain = ReadArraySingle(reader, 16);
            fTelegraphFilter = ReadArraySingle(reader, 16);
            fTelegraphMembraneCap = ReadArraySingle(reader, 16);
            nTelegraphMode = ReadArrayInt16(reader, 16);
            nTelegraphDACScaleFactorEnable = ReadArrayInt16(reader, 16);

            // If telegraph info contains garbage data, zero-out everything.
            // There may be a better way to determine if this section exists or not.
            if (nTelegraphEnable.Min() < 0 || nTelegraphEnable.Max() > 16)
            {
                nTelegraphEnable = new Int16[16];
                nTelegraphInstrument = new Int16[16];
                fTelegraphAdditGain = new Single[16];
                fTelegraphFilter = new Single[16];
                fTelegraphMembraneCap = new Single[16];
                nTelegraphMode = new Int16[16];
                nTelegraphDACScaleFactorEnable = new Int16[8];
            }

            reader.BaseStream.Seek(4832, SeekOrigin.Begin);
            nAutoAnalyseEnable = reader.ReadInt16();
            string sAutoAnalysisMacroName = ReadString(reader, 64);
            sProtocolPath = ReadString(reader, 256);
            sFileComment = ReadString(reader, 128);
            FileGUID = MakeGuid(reader.ReadBytes(16));

            reader.BaseStream.Seek(5298, SeekOrigin.Begin);
            fInstrumentHoldingLevel = ReadArraySingle(reader, 16);
            ulFileCRC = reader.ReadUInt32();

            reader.BaseStream.Seek(5318, SeekOrigin.Begin);
            sModifierInfo = new string(reader.ReadChars(16).Where(x => x >= 'A' && x < 'z').ToArray()).Trim();
        }

        public void ReadGroup7(BinaryReader reader)
        {
            reader.BaseStream.Seek(378, SeekOrigin.Begin);
            nADCPtoLChannelMap = ReadArrayInt16(reader, 16);
            nADCSamplingSeq = ReadArrayInt16(reader, 16);
            sADCChannelName = ReadArrayStrings(reader, 16, 10);
            sADCUnits = ReadArrayStrings(reader, 16, 8);
            fADCProgrammableGain = ReadArraySingle(reader, 16);
            fADCDisplayAmplification = ReadArraySingle(reader, 16);
            fADCDisplayOffset = ReadArraySingle(reader, 16);
            fInstrumentScaleFactor = ReadArraySingle(reader, 16);
            fInstrumentOffset = ReadArraySingle(reader, 16);
            fSignalGain = ReadArraySingle(reader, 16);
            fSignalOffset = ReadArraySingle(reader, 16);
            fSignalLowpassFilter = ReadArraySingle(reader, 16);
            fSignalHighpassFilter = ReadArraySingle(reader, 16);
            sDACChannelName = ReadArrayStrings(reader, 4, 10);
            sDACChannelUnits = ReadArrayStrings(reader, 4, 8);
            fDACScaleFactor = ReadArraySingle(reader, 4);
            fDACHoldingLevel = ReadArraySingle(reader, 4);
            nSignalType = reader.ReadInt16();

            // ABF1 doesn't support the hum filter so it's always zero
            bHumFilterEnable = new byte[16];

            // extended group 7
            reader.BaseStream.Seek(2074, SeekOrigin.Begin);
            fDACCalibrationFactor = ReadArraySingle(reader, 4);
            fDACCalibrationOffset = ReadArraySingle(reader, 4);

            if (fFileVersionNumber < 1.6)
            {
                for (int i = 0; i < 4; i++)
                {
                    fDACCalibrationFactor[i] = 1;
                    fDACCalibrationOffset[i] = 1;
                }
            }

            // ABF1 doesn't support these
            nLowpassFilterType = new byte[16];
            nHighpassFilterType = new byte[16];
        }

        public void ReadGroup9(BinaryReader reader)
        {
            reader.BaseStream.Seek(1436, SeekOrigin.Begin);
            nDigitalEnable = reader.ReadInt16();

            reader.BaseStream.Seek(1440, SeekOrigin.Begin);
            nActiveDACChannel = reader.ReadInt16();

            reader.BaseStream.Seek(1584, SeekOrigin.Begin);
            nDigitalHolding = reader.ReadInt16();
            nDigitalInterEpisode = reader.ReadInt16();
            nDigitalValue = ReadArrayInt16(reader, 16);

            reader.BaseStream.Seek(2296, SeekOrigin.Begin);
            nWaveformEnable = ReadArrayInt16(reader, 2);
            nWaveformSource = ReadArrayInt16(reader, 2);
            nInterEpisodeLevel = ReadArrayInt16(reader, 2);
            nEpochType = ReadArrayInt16(reader, 20);
            fEpochInitLevel = ReadArraySingle(reader, 20);
            fEpochLevelInc = ReadArraySingle(reader, 20);
            lEpochInitDuration = ReadArrayInt32(reader, 20);
            lEpochDurationInc = ReadArrayInt32(reader, 20);
            nDigitalTrainValue = ReadArrayInt16(reader, 10);
            bEpochCompression = new byte[50];
            nDigitalTrainActiveLogic = reader.ReadInt16();
        }

        public void ReadGroup10(BinaryReader reader)
        {
            reader.BaseStream.Seek(2708, SeekOrigin.Begin);
            fDACFileScale = ReadArraySingle(reader, 2);
            fDACFileOffset = ReadArraySingle(reader, 2);
            lDACFileEpisodeNum = ReadArrayInt32(reader, 2);
            nDACFileADCNum = ReadArrayInt16(reader, 2);
            sDACFilePath = Encoding.ASCII.GetString(reader.ReadBytes(412).Where(x => x != 0).ToArray());
        }
    }
}
