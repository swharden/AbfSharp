using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp.HeaderData.Abf1
{
    public class Abf1Header
    {
        // TODO: verify types against official header types
        public int lFileSignature { get; private set; }
        public float fFileVersionNumber { get; private set; }
        public int nOperationMode { get; private set; }

        public Abf1Header(BinaryReader reader)
        {
            // https://docs.python.org/3/library/struct.html#format-characters
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

            /*
            from pyABF:

            # GROUP 1 - File ID and size information. (40 bytes)
            self.lFileSignature = readStruct(fb, "i", 0)
            self.fFileVersionNumber = readStruct(fb, "f", 4)
            self.nOperationMode = readStruct(fb, "h", 8)
            self.lActualAcqLength = readStruct(fb, "i", 10)
            self.nNumPointsIgnored = readStruct(fb, "h", 14)
            self.lActualEpisodes = readStruct(fb, "i", 16)
            self.lFileStartDate = readStruct(fb, "i", 20)
            self.lFileStartTime = readStruct(fb, "i", 24)
            self.lStopwatchTime = readStruct(fb, "i", 28)
            self.fHeaderVersionNumber = readStruct(fb, "f", 32)
            self.nFileType = readStruct(fb, "h", 36)
            self.nMSBinFormat = readStruct(fb, "h", 38)

            # GROUP 2 - File Structure (78 bytes)
            self.lDataSectionPtr = readStruct(fb, "i", 40)
            self.lTagSectionPtr = readStruct(fb, "i", 44)
            self.lNumTagEntries = readStruct(fb, "i", 48)

            # missing entries

            self.lSynchArrayPtr = readStruct(fb, "i", 92)
            self.lSynchArraySize = readStruct(fb, "i", 96)
            self.nDataFormat = readStruct(fb, "h", 100)

            # missing entries

            # GROUP 3 - Trial hierarchy information (82 bytes)
            self.nADCNumChannels = readStruct(fb, "h", 120)
            self.fADCSampleInterval = readStruct(fb, "f", 122)
            # missing entries
            self.fSynchTimeUnit = readStruct(fb, "f", 130)
            # missing entries
            self.lNumSamplesPerEpisode = readStruct(fb, "i", 138)
            self.lPreTriggerSamples = readStruct(fb, "i", 142)
            self.lEpisodesPerRun = readStruct(fb, "i", 146)
            # missing entries

            # GROUP 4 - Display Parameters (44 bytes)
            # missing entries

            # GROUP 5 - Hardware information (16 bytes)
            self.fADCRange = readStruct(fb, "f", 244)
            self.fDACRange = readStruct(fb, "f", 248)
            self.lADCResolution = readStruct(fb, "i", 252)
            self.lDACResolution = readStruct(fb, "i", 256)

            # GROUP 6 - Environmental Information (118 bytes)
            self.nExperimentType = readStruct(fb, "h", 260)
            # missing entries
            self.sCreatorInfo = readStruct(fb, "16s", 294)
            self.sFileCommentOld = readStruct(fb, "56s", 310)
            self.nFileStartMillisecs = readStruct(fb, "h", 366)
            # missing entries

            # GROUP 7 - Multi-channel information (1044 bytes)
            self.nADCPtoLChannelMap = readStruct(fb, "16h", 378)
            self.nADCSamplingSeq = readStruct(fb, "16h", 410)
            self.sADCChannelName = readStruct(fb, "10s"*16, 442)
            self.sADCUnits = readStruct(fb, "8s"*16, 602)
            self.fADCProgrammableGain = readStruct(fb, "16f", 730)
            # missing entries
            self.fInstrumentScaleFactor = readStruct(fb, "16f", 922)
            self.fInstrumentOffset = readStruct(fb, "16f", 986)
            self.fSignalGain = readStruct(fb, "16f", 1050)
            self.fSignalOffset = readStruct(fb, "16f", 1114)
            self.sDACChannelName = readStruct(fb, "10s"*4, 1306)
            self.sDACChannelUnit = readStruct(fb, "8s"*4, 1346)
            # missing entries

            # GROUP 8 - Synchronous timer outputs (14 bytes)
            # missing entries
            # GROUP 9 - Epoch Waveform and Pulses (184 bytes)
            self.nDigitalEnable = readStruct(fb, "h", 1436)
            # missing entries
            self.nActiveDACChannel = readStruct(fb, "h", 1440)
            # missing entries
            self.nDigitalHolding = readStruct(fb, "h", 1584)
            self.nDigitalInterEpisode = readStruct(fb, "h", 1586)
            # missing entries
            self.nDigitalValue = readStruct(fb, "10h", 1588)

            # GROUP 10 - DAC Output File (98 bytes)
            # missing entries
            # GROUP 11 - Presweep (conditioning) pulse train (44 bytes)
            # missing entries
            # GROUP 13 - Autopeak measurement (36 bytes)
            # missing entries
            # GROUP 14 - Channel Arithmetic (52 bytes)
            # missing entries
            # GROUP 15 - On-line subtraction (34 bytes)
            # missing entries
            # GROUP 16 - Miscellaneous variables (82 bytes)
            # missing entries
            # EXTENDED GROUP 2 - File Structure (16 bytes)
            self.lDACFilePtr = readStruct(fb, "2i", 2048)
            self.lDACFileNumEpisodes = readStruct(fb, "2i", 2056)
            # EXTENDED GROUP 3 - Trial Hierarchy
            # missing entries
            # EXTENDED GROUP 7 - Multi-channel information (62 bytes)
            self.fDACCalibrationFactor = readStruct(fb, "4f", 2074)
            self.fDACCalibrationOffset = readStruct(fb, "4f", 2090)

            # GROUP 17 - Trains parameters (160 bytes)
            # missing entries
            # EXTENDED GROUP 9 - Epoch Waveform and Pulses (412 bytes)
            self.nWaveformEnable = readStruct(fb, "2h", 2296)
            self.nWaveformSource = readStruct(fb, "2h", 2300)
            self.nInterEpisodeLevel = readStruct(fb, "2h", 2304)
            self.nEpochType = readStruct(fb, "20h", 2308)
            self.fEpochInitLevel = readStruct(fb, "20f", 2348)
            self.fEpochLevelInc = readStruct(fb, "20f", 2428)
            self.lEpochInitDuration = readStruct(fb, "20i", 2508)
            self.lEpochDurationInc = readStruct(fb, "20i", 2588)
            # missing entries

            # EXTENDED GROUP 10 - DAC Output File (552 bytes)
            self.fDACFileScale = readStruct(fb, "2f", 2708)
            self.fDACFileOffset = readStruct(fb, "2f", 2716)
            self.lDACFileEpisodeNum = readStruct(fb, "2i", 2724)
            self.nDACFileADCNum = readStruct(fb, "2h", 2732)
            self.sDACFilePath = readStruct(fb, "256s"*2, 2736)
            # EXTENDED GROUP 11 - Presweep (conditioning) pulse train (100 bytes)
            # missing entries
            # EXTENDED GROUP 12 - Variable parameter user list (1096 bytes)
            # missing entries
            # EXTENDED GROUP 15 - On-line subtraction (56 bytes)
            # missing entries
            # EXTENDED GROUP 6 Environmental Information  (898 bytes)
            self.nTelegraphEnable = readStruct(fb, "16h", 4512)
            self.nTelegraphInstrument = readStruct(fb, "16h", 4544)
            self.fTelegraphAdditGain = readStruct(fb, "16f", 4576)
            self.fTelegraphFilter = readStruct(fb, "16f", 4640)
            self.fTelegraphMembraneCap = readStruct(fb, "16f", 4704)
            self.nTelegraphMode = readStruct(fb, "16h", 4768)
            self.nTelegraphDACScaleFactorEnable = readStruct(fb, "4h", 4800)
            # missing entries
            self.sProtocolPath = readStruct(fb, "256s", 4898)
            self.sFileCommentNew = readStruct(fb, "128s", 5154)
            self.fInstrumentHoldingLevel = readStruct(fb, "4f", 5298)
            self.ulFileCRC = readStruct(fb, "I", 5314)
            # missing entries
            self.nCreatorMajorVersion = readStruct(fb, "h", 5798)
            self.nCreatorMinorVersion = readStruct(fb, "h", 5800)
            self.nCreatorBugfixVersion = readStruct(fb, "h", 5802)
            self.nCreatorBuildVersion = readStruct(fb, "h", 5804)

            # EXTENDED GROUP 13 - Statistics measurements (388 bytes)
            # missing entries
            # GROUP 18 - Application version data (16 bytes)
            self.uFileGUID = readStruct(fb, "16B", 5282)
            # missing entries
            # GROUP 19 - LTP protocol (14 bytes)
            # missing entries
            # GROUP 20 - Digidata 132x Trigger out flag. (8 bytes)
            # missing entries
            # GROUP 21 - Epoch resistance (56 bytes) // TODO old value of 40 correct??
            # missing entries
            # GROUP 22 - Alternating episodic mode (58 bytes)
            # missing entries
            # GROUP 23 - Post-processing actions (210 bytes)
            # missing entries

            */
        }
    }
}
