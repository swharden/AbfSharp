using AbfSharp.HeaderData;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbfSharp
{
    public class HeaderBase
    {
        /* Fields were brought in from the ABFFIO header struct.
         * Uncomment them one by one only as they are implemented and tested.
         */

        #region constants
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_ADCCOUNT = 16; // number of ADC channels supported.
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_DACCOUNT = 8; // number of DAC channels supported.
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_EPOCHCOUNT = 50; // number of waveform epochs supported.
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_ADCUNITLEN = 8; // length of ADC units strings
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_ADCNAMELEN = 10; // length of actual ADC channel name strings
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_DACUNITLEN = 8; // length of DAC units strings
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_DACNAMELEN = 10; // length of DAC channel name strings
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_USERLISTLEN = 256; // length of the user list (V1.6)
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_USERLISTCOUNT = ABF_DACCOUNT; // number of independent user lists (V1.6)       
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_FILECOMMENTLEN = 128; // length of file comment string (V1.6)
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_PATHLEN = 256; // length of full path, used for DACFile and Protocol name.
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_CREATORINFOLEN = 16; // length of file creator info string
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_ARITHMETICOPLEN = 2; // length of the Arithmetic operator field
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_ARITHMETICUNITSLEN = 8; // length of arithmetic units string
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_STATS_REGIONS = 24; // The number of independent statistics regions. // ST-91
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_TAGCOMMENTLEN = 56; // length of tag comment string
        [Obsolete("this field has not yet been implemented/tested")] public const int ABFH_HOLDINGFRACTION = 64; // helps calculate the pre-epoch duration
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_ADCNAMELEN_USER = 8; // length of user-entered ADC channel name strings
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_OLDFILECOMMENTLEN = 56; // length of file comment string (pre V1.6)
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_BLOCKSIZE = 512; // Size of block alignment in ABF files.
        [Obsolete("this field has not yet been implemented/tested")] public const int PCLAMP6_MAXSWEEPLENGTH = 16384; // Maximum multiplexed sweep length supported by pCLAMP6 apps.
        [Obsolete("this field has not yet been implemented/tested")] public const int PCLAMP7_MAXSWEEPLEN_PERCHAN = 1032258; // Maximum per channel sweep length supported by pCLAMP7 apps.
        [Obsolete("this field has not yet been implemented/tested")] public const int PCLAMP11_MAXSWEEPLEN_PERCHAN = 5161290; // Maximum per channel sweep length supported by pCLAMP11 apps. //ST-1
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_MAX_SWEEPS_PER_AVERAGE = 65500; // The maximum number of sweeps that can be combined into a cumulative average 
        [Obsolete("this field has not yet been implemented/tested")] public const int ABF_MAX_TRIAL_SAMPLES = 0x7FFFFFFF; // Maximum length of acquisition supported (samples)
        #endregion

        #region GROUP 0 - Metadata

        /// <summary>
        /// First 4 characters of the file (typically "ABF " or "ABF2").
        /// </summary>
        public string Signature => Encoding.ASCII.GetString(SignatureBytes);

        /// <summary>
        /// First 4 bytes of the file
        /// </summary>
        public byte[] SignatureBytes { get; protected set; }

        /// <summary>
        /// Full path to this ABF file
        /// </summary>
        public string Path { get; protected set; }

        /// <summary>
        /// Filename of this ABF file
        /// </summary>
        public string Filename => System.IO.Path.GetFileName(Path);

        /// <summary>
        /// Filename without the .abf extension (useful for chart titles, logging, etc.)
        /// </summary>
        public string AbfID => System.IO.Path.GetFileNameWithoutExtension(Path);

        #endregion

        #region GROUP 1 - File ID and size information

        /// <summary>
        /// File format version stored in the data file during acquisition.
        /// </summary>
        public float fFileVersionNumber { get; protected set; }

        /// <summary>
        /// Operation mode:
        /// 1 = Event-driven, variable length
        /// 2 = Oscilloscope, loss free (Same as Event-driven, fixed length)
        /// 3 = Gap-free
        /// 4 = Oscilloscope, high-speed
        /// 5 = episodic stimulation (Clampex only)
        /// </summary>
        public short nOperationMode { get; protected set; }

        public OperationMode OperationMode => (OperationMode)nOperationMode;

        /// <summary>
        /// Actual number of ADC samples (aggregate) in data file. 
        /// Averaged sweeps are included.
        /// </summary>

        public int lActualAcqLength { get; protected set; }

        /// <summary>
        /// Number of points ignored at data start. 
        /// Normally zero, but non-zero for gap-free acquisition using AXOLAB configurations with one or more ADS boards.
        /// I think this is always zero in ABF2 files.
        /// </summary>
        public short nNumPointsIgnored { get; protected set; }

        /// <summary>
        /// Actual number of sweeps in the file including averaged sweeps. 
        /// If nOperationMode = 3 (gap-free) the value of this parameter is 1.
        /// </summary>
        public int lActualEpisodes { get; protected set; }

        /// <summary>
        /// Numerical representation of the day the ABF recording was made (YYYYMMDD)
        /// </summary>
        public uint uFileStartDate;

        /// <summary>
        /// Time the ABF recording was started (milliseconds since midnight)
        /// </summary>
        public uint uFileStartTimeMS;

        /// <summary>
        /// Date and time the ABF recording was started.
        /// </summary>
        public DateTime FileStart => AbfDateTime(uFileStartDate, uFileStartTimeMS);

        /// <summary>
        /// Convert a MMMMDDYY date and time of day (milliseconds past midnight) to a .NET DateTime
        /// </summary>
        private static DateTime AbfDateTime(uint dateCode, uint dayTimeMS)
        {
            uint day = dateCode % 100;
            dateCode /= 100;
            uint month = dateCode % 100;
            dateCode /= 100;
            uint year = dateCode;
            try
            {
                return new DateTime((int)year, (int)month, (int)day) + TimeSpan.FromMilliseconds(dayTimeMS);
            }
            catch (ArgumentOutOfRangeException)
            {
                // happens when synthesized ABFs (de novo) have zeros for month and day
                return new DateTime();
            }
        }

        /// <summary>
        /// Time since the stopwatch was zeroed that the data portion of this file was first written to. 
        /// Not supported by all programs. 
        /// Default = 0.
        /// </summary>
        public int lStopwatchTime;

        /// <summary>
        /// This is set by the ABF file READER, not stored in the ABF file itself.
        /// When old ABF files are read by a modern reader, this number gets higher.
        /// When using ABFFIO.DLL, this may be a creative way to get the version.
        /// </summary>
        public float fHeaderVersionNumber;

        /// <summary>
        /// Numeric equivalent of file type. I expect this to always be 1 for ABF files, or 0 for ABFs created de novo.
        /// 1 = ABF file
        /// 2 = Old FETCHEX file (FTCX)
        /// 3 = Old Clampex file (CLPX)
        /// </summary>
        public short nFileType;

        #endregion

        #region GROUP 2 - File Structure
        [Obsolete("this field has not yet been implemented/tested")] public int lDataSectionPtr;
        [Obsolete("this field has not yet been implemented/tested")] public int lTagSectionPtr;
        [Obsolete("this field has not yet been implemented/tested")] public int lNumTagEntries;
        [Obsolete("this field has not yet been implemented/tested")] public int lScopeConfigPtr;
        [Obsolete("this field has not yet been implemented/tested")] public int lNumScopes;
        [Obsolete("this field has not yet been implemented/tested")] public int lDeltaArrayPtr;
        [Obsolete("this field has not yet been implemented/tested")] public int lNumDeltas;
        [Obsolete("this field has not yet been implemented/tested")] public int lVoiceTagPtr;
        [Obsolete("this field has not yet been implemented/tested")] public int lVoiceTagEntries;
        [Obsolete("this field has not yet been implemented/tested")] public int lSynchArrayPtr;
        [Obsolete("this field has not yet been implemented/tested")] public int lSynchArraySize;
        [Obsolete("this field has not yet been implemented/tested")] public short nDataFormat;
        [Obsolete("this field has not yet been implemented/tested")] public short nSimultaneousScan;
        [Obsolete("this field has not yet been implemented/tested")] public int lStatisticsConfigPtr;
        [Obsolete("this field has not yet been implemented/tested")] public int lAnnotationSectionPtr;
        [Obsolete("this field has not yet been implemented/tested")] public int lNumAnnotations;
        [Obsolete("this field has not yet been implemented/tested")] public int[] lDACFilePtr = new int[ABF_DACCOUNT];
        [Obsolete("this field has not yet been implemented/tested")] public int[] lDACFileNumEpisodes = new int[ABF_DACCOUNT];
        #endregion

        #region GROUP 3 - Trial hierarchy information
        [Obsolete("this field has not yet been implemented/tested")] public short nADCNumChannels;
        [Obsolete("this field has not yet been implemented/tested")] public float fADCSequenceInterval;
        [Obsolete("this field has not yet been implemented/tested")] public uint uFileCompressionRatio;
        [Obsolete("this field has not yet been implemented/tested")] public byte bEnableFileCompression;
        [Obsolete("this field has not yet been implemented/tested")] public float fSynchTimeUnit;
        [Obsolete("this field has not yet been implemented/tested")] public float fSecondsPerRun;
        [Obsolete("this field has not yet been implemented/tested")] public int lNumSamplesPerEpisode;
        [Obsolete("this field has not yet been implemented/tested")] public int lPreTriggerSamples;
        [Obsolete("this field has not yet been implemented/tested")] public int lEpisodesPerRun;
        [Obsolete("this field has not yet been implemented/tested")] public int lRunsPerTrial;
        [Obsolete("this field has not yet been implemented/tested")] public int lNumberOfTrials;
        [Obsolete("this field has not yet been implemented/tested")] public short nAveragingMode;
        [Obsolete("this field has not yet been implemented/tested")] public short nUndoRunCount;
        [Obsolete("this field has not yet been implemented/tested")] public short nFirstEpisodeInRun;
        [Obsolete("this field has not yet been implemented/tested")] public float fTriggerThreshold;
        [Obsolete("this field has not yet been implemented/tested")] public short nTriggerSource;
        [Obsolete("this field has not yet been implemented/tested")] public short nTriggerAction;
        [Obsolete("this field has not yet been implemented/tested")] public short nTriggerPolarity;
        [Obsolete("this field has not yet been implemented/tested")] public float fScopeOutputInterval;
        [Obsolete("this field has not yet been implemented/tested")] public float fEpisodeStartToStart;
        [Obsolete("this field has not yet been implemented/tested")] public float fRunStartToStart;
        [Obsolete("this field has not yet been implemented/tested")] public float fTrialStartToStart;
        [Obsolete("this field has not yet been implemented/tested")] public int lAverageCount;
        [Obsolete("this field has not yet been implemented/tested")] public short nAutoTriggerStrategy;
        [Obsolete("this field has not yet been implemented/tested")] public float fFirstRunDelayS;
        [Obsolete("this field has not yet been implemented/tested")] public uint nTriggerTimeout;
        #endregion

        #region GROUP 4 - Display Parameters
        [Obsolete("this field has not yet been implemented/tested")] public short nDataDisplayMode;
        [Obsolete("this field has not yet been implemented/tested")] public short nChannelStatsStrategy;
        [Obsolete("this field has not yet been implemented/tested")] public int lSamplesPerTrace;
        [Obsolete("this field has not yet been implemented/tested")] public int lStartDisplayNum;
        [Obsolete("this field has not yet been implemented/tested")] public int lFinishDisplayNum;
        [Obsolete("this field has not yet been implemented/tested")] public short nShowPNRawData;
        [Obsolete("this field has not yet been implemented/tested")] public float fStatisticsPeriod;
        [Obsolete("this field has not yet been implemented/tested")] public int lStatisticsMeasurements;
        [Obsolete("this field has not yet been implemented/tested")] public short nStatisticsSaveStrategy;
        #endregion

        #region GROUP 5 - Hardware information
        [Obsolete("this field has not yet been implemented/tested")] public float fADCRange;
        [Obsolete("this field has not yet been implemented/tested")] public float fDACRange;
        [Obsolete("this field has not yet been implemented/tested")] public int lADCResolution;
        [Obsolete("this field has not yet been implemented/tested")] public int lDACResolution;
        [Obsolete("this field has not yet been implemented/tested")] public short nDigitizerADCs;
        [Obsolete("this field has not yet been implemented/tested")] public short nDigitizerDACs;
        [Obsolete("this field has not yet been implemented/tested")] public short nDigitizerTotalDigitalOuts;
        [Obsolete("this field has not yet been implemented/tested")] public short nDigitizerSynchDigitalOuts;
        [Obsolete("this field has not yet been implemented/tested")] public short nDigitizerType;
        #endregion

        #region GROUP 6 Environmental Information
        [Obsolete("this field has not yet been implemented/tested")] public short nExperimentType;
        [Obsolete("this field has not yet been implemented/tested")] public short nManualInfoStrategy;
        [Obsolete("this field has not yet been implemented/tested")] public float fCellID1;
        [Obsolete("this field has not yet been implemented/tested")] public float fCellID2;
        [Obsolete("this field has not yet been implemented/tested")] public float fCellID3;
        [Obsolete("this field has not yet been implemented/tested")] public string sProtocolPath;
        [Obsolete("this field has not yet been implemented/tested")] public string sCreatorInfo;
        [Obsolete("this field has not yet been implemented/tested")] public string sModifierInfo;
        [Obsolete("this field has not yet been implemented/tested")] public short nCommentsEnable;
        [Obsolete("this field has not yet been implemented/tested")] public string sFileComment;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nTelegraphEnable;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nTelegraphInstrument;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fTelegraphAdditGain;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fTelegraphFilter;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fTelegraphMembraneCap;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fTelegraphAccessResistance;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nTelegraphMode;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nTelegraphDACScaleFactorEnable;
        [Obsolete("this field has not yet been implemented/tested")] public short nAutoAnalyseEnable;
        [Obsolete("this field has not yet been implemented/tested")] public Guid FileGUID;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fInstrumentHoldingLevel;
        [Obsolete("this field has not yet been implemented/tested")] public uint ulFileCRC;
        [Obsolete("this field has not yet been implemented/tested")] public short nCRCEnable;
        #endregion

        #region GROUP 7 - Multi-channel information
        [Obsolete("this field has not yet been implemented/tested")] public short nSignalType;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nADCPtoLChannelMap;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nADCSamplingSeq;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fADCProgrammableGain;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fADCDisplayAmplification;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fADCDisplayOffset;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fInstrumentScaleFactor;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fInstrumentOffset;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fSignalGain;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fSignalOffset;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fSignalLowpassFilter;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fSignalHighpassFilter;
        [Obsolete("this field has not yet been implemented/tested")] public string nLowpassFilterType;
        [Obsolete("this field has not yet been implemented/tested")] public string nHighpassFilterType;
        [Obsolete("this field has not yet been implemented/tested")] public byte[] bHumFilterEnable;
        [Obsolete("this field has not yet been implemented/tested")] public string sADCChannelName;
        [Obsolete("this field has not yet been implemented/tested")] public string sADCUnits;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fDACScaleFactor;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fDACHoldingLevel;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fDACCalibrationFactor;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fDACCalibrationOffset;
        [Obsolete("this field has not yet been implemented/tested")] public string sDACChannelName;
        [Obsolete("this field has not yet been implemented/tested")] public string sDACChannelUnits;
        #endregion

        #region GROUP 9 - Epoch Waveform and Pulses
        [Obsolete("this field has not yet been implemented/tested")] public short nDigitalEnable;
        [Obsolete("this field has not yet been implemented/tested")] public short nActiveDACChannel;
        [Obsolete("this field has not yet been implemented/tested")] public short nDigitalDACChannel;
        [Obsolete("this field has not yet been implemented/tested")] public short nDigitalHolding;
        [Obsolete("this field has not yet been implemented/tested")] public short nDigitalInterEpisode;
        [Obsolete("this field has not yet been implemented/tested")] public short nDigitalTrainActiveLogic;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nDigitalValue;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nDigitalTrainValue;
        [Obsolete("this field has not yet been implemented/tested")] public byte[] bEpochCompression;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nWaveformEnable;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nWaveformSource;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nInterEpisodeLevel;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nEpochType;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fEpochInitLevel;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fEpochFinalLevel;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fEpochLevelInc;
        [Obsolete("this field has not yet been implemented/tested")] public int[] lEpochInitDuration;
        [Obsolete("this field has not yet been implemented/tested")] public int[] lEpochDurationInc;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nEpochTableRepetitions;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fEpochTableStartToStartInterval;
        #endregion

        #region GROUP 10 - DAC Output File
        [Obsolete("this field has not yet been implemented/tested")] public float[] fDACFileScale;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fDACFileOffset;
        [Obsolete("this field has not yet been implemented/tested")] public int[] lDACFileEpisodeNum;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nDACFileADCNum;
        [Obsolete("this field has not yet been implemented/tested")] public string sDACFilePath;
        #endregion

        #region GROUP 11a - Presweep (conditioning) pulse train
        [Obsolete("this field has not yet been implemented/tested")] public short[] nConditEnable;
        [Obsolete("this field has not yet been implemented/tested")] public int[] lConditNumPulses;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fBaselineDuration;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fBaselineLevel;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fStepDuration;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fStepLevel;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fPostTrainPeriod;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fPostTrainLevel;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fCTStartLevel;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fCTEndLevel;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fCTIntervalDuration;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fCTStartToStartInterval;
        #endregion

        #region GROUP 11b - Membrane Test Between Sweeps
        [Obsolete("this field has not yet been implemented/tested")] public short[] nMembTestEnable;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fMembTestPreSettlingTimeMS;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fMembTestPostSettlingTimeMS;
        #endregion

        #region GROUP 11c - PreSignal test pulse
        [Obsolete("this field has not yet been implemented/tested")] public short[] nPreSignalEnable;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fPreSignalPreStepDuration;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fPreSignalPreStepLevel;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fPreSignalStepDuration;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fPreSignalStepLevel;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fPreSignalPostStepDuration;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fPreSignalPostStepLevel;
        #endregion

        #region GROUP 11d - Hum Silncer Adapt between sweeps
        [Obsolete("this field has not yet been implemented/tested")] public short nAdaptEnable;
        [Obsolete("this field has not yet been implemented/tested")] public float fInterSweepAdaptTimeS;
        #endregion

        #region GROUP 12 - Variable parameter user list
        [Obsolete("this field has not yet been implemented/tested")] public short[] nULEnable;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nULParamToVary;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nULRepeat;
        [Obsolete("this field has not yet been implemented/tested")] public string sULParamValueList;
        #endregion

        #region GROUP 13 - Statistics measurements
        [Obsolete("this field has not yet been implemented/tested")] public short nStatsEnable;
        [Obsolete("this field has not yet been implemented/tested")] public ushort nStatsActiveChannels;
        [Obsolete("this field has not yet been implemented/tested")] public ushort nStatsSearchRegionFlags;
        [Obsolete("this field has not yet been implemented/tested")] public short nStatsSmoothing;
        [Obsolete("this field has not yet been implemented/tested")] public short nStatsSmoothingEnable;
        [Obsolete("this field has not yet been implemented/tested")] public short nStatsBaseline;
        [Obsolete("this field has not yet been implemented/tested")] public short nStatsBaselineDAC;
        [Obsolete("this field has not yet been implemented/tested")] public int lStatsBaselineStart;
        [Obsolete("this field has not yet been implemented/tested")] public int lStatsBaselineEnd;
        [Obsolete("this field has not yet been implemented/tested")] public int[] lStatsMeasurements;
        [Obsolete("this field has not yet been implemented/tested")] public int[] lStatsStart;
        [Obsolete("this field has not yet been implemented/tested")] public int[] lStatsEnd;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nRiseBottomPercentile;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nRiseTopPercentile;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nDecayBottomPercentile;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nDecayTopPercentile;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nStatsChannelPolarity;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nStatsSearchMode;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nStatsSearchDAC;
        #endregion

        #region GROUP 14 - Channel Arithmetic
        [Obsolete("this field has not yet been implemented/tested")] public short nArithmeticEnable;
        [Obsolete("this field has not yet been implemented/tested")] public short nArithmeticExpression;
        [Obsolete("this field has not yet been implemented/tested")] public float fArithmeticUpperLimit;
        [Obsolete("this field has not yet been implemented/tested")] public float fArithmeticLowerLimit;
        [Obsolete("this field has not yet been implemented/tested")] public short nArithmeticADCNumA;
        [Obsolete("this field has not yet been implemented/tested")] public short nArithmeticADCNumB;
        [Obsolete("this field has not yet been implemented/tested")] public float fArithmeticK1;
        [Obsolete("this field has not yet been implemented/tested")] public float fArithmeticK2;
        [Obsolete("this field has not yet been implemented/tested")] public float fArithmeticK3;
        [Obsolete("this field has not yet been implemented/tested")] public float fArithmeticK4;
        [Obsolete("this field has not yet been implemented/tested")] public float fArithmeticK5;
        [Obsolete("this field has not yet been implemented/tested")] public float fArithmeticK6;
        [Obsolete("this field has not yet been implemented/tested")] public string sArithmeticOperator;
        [Obsolete("this field has not yet been implemented/tested")] public string sArithmeticUnits;
        #endregion

        #region GROUP 15 - Leak subtraction
        [Obsolete("this field has not yet been implemented/tested")] public short nPNPosition;
        [Obsolete("this field has not yet been implemented/tested")] public short nPNNumPulses;
        [Obsolete("this field has not yet been implemented/tested")] public short nPNPolarity;
        [Obsolete("this field has not yet been implemented/tested")] public float fPNSettlingTime;
        [Obsolete("this field has not yet been implemented/tested")] public float fPNInterpulse;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nLeakSubtractType;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fPNHoldingLevel;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nLeakSubtractADCIndex;
        #endregion

        #region GROUP 16 - Miscellaneous variables
        [Obsolete("this field has not yet been implemented/tested")] public short nLevelHysteresis;
        [Obsolete("this field has not yet been implemented/tested")] public int lTimeHysteresis;
        [Obsolete("this field has not yet been implemented/tested")] public short nAllowExternalTags;
        [Obsolete("this field has not yet been implemented/tested")] public short nAverageAlgorithm;
        [Obsolete("this field has not yet been implemented/tested")] public float fAverageWeighting;
        [Obsolete("this field has not yet been implemented/tested")] public short nUndoPromptStrategy;
        [Obsolete("this field has not yet been implemented/tested")] public short nTrialTriggerSource;
        [Obsolete("this field has not yet been implemented/tested")] public short nStatisticsDisplayStrategy;
        [Obsolete("this field has not yet been implemented/tested")] public short nExternalTagType;
        [Obsolete("this field has not yet been implemented/tested")] public int lHeaderSize;
        [Obsolete("this field has not yet been implemented/tested")] public short nStatisticsClearStrategy;
        [Obsolete("this field has not yet been implemented/tested")] public short nEnableFirstLastHolding;

        #endregion

        #region GROUP 17 - Trains parameters
        [Obsolete("this field has not yet been implemented/tested")] public int[] lEpochPulsePeriod;
        [Obsolete("this field has not yet been implemented/tested")] public int[] lEpochPulseWidth;
        #endregion

        #region GROUP 18 - Application version data
        [Obsolete("this field has not yet been implemented/tested")] public short nCreatorMajorVersion;
        [Obsolete("this field has not yet been implemented/tested")] public short nCreatorMinorVersion;
        [Obsolete("this field has not yet been implemented/tested")] public short nCreatorBugfixVersion;
        [Obsolete("this field has not yet been implemented/tested")] public short nCreatorBuildVersion;
        [Obsolete("this field has not yet been implemented/tested")] public short nModifierMajorVersion;
        [Obsolete("this field has not yet been implemented/tested")] public short nModifierMinorVersion;
        [Obsolete("this field has not yet been implemented/tested")] public short nModifierBugfixVersion;
        [Obsolete("this field has not yet been implemented/tested")] public short nModifierBuildVersion;
        #endregion

        #region GROUP 19 - LTP protocol
        [Obsolete("this field has not yet been implemented/tested")] public short nLTPType;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nLTPUsageOfDAC;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nLTPPresynapticPulses;
        #endregion

        #region GROUP 20 - Digidata 132x Trigger out flag
        [Obsolete("this field has not yet been implemented/tested")] public short nScopeTriggerOut;
        #endregion

        #region GROUP 21 - Epoch resistance
        [Obsolete("this field has not yet been implemented/tested")] public string sEpochResistanceSignalName;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nEpochResistanceState;
        #endregion

        #region GROUP 22 - Alternating episodic mode
        [Obsolete("this field has not yet been implemented/tested")] public short nAlternateDACOutputState;
        [Obsolete("this field has not yet been implemented/tested")] public short nAlternateDigitalOutputState;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nAlternateDigitalValue;
        [Obsolete("this field has not yet been implemented/tested")] public short[] nAlternateDigitalTrainValue;
        #endregion

        #region GROUP 23 - Post-processing actions
        [Obsolete("this field has not yet been implemented/tested")] public float[] fPostProcessLowpassFilter;
        [Obsolete("this field has not yet been implemented/tested")] public string nPostProcessLowpassFilterType;
        #endregion

        #region GROUP 24 - Legacy gear shift info
        [Obsolete("this field has not yet been implemented/tested")] public float fLegacyADCSequenceInterval;
        [Obsolete("this field has not yet been implemented/tested")] public float fLegacyADCSecondSequenceInterval;
        [Obsolete("this field has not yet been implemented/tested")] public int lLegacyClockChange;
        [Obsolete("this field has not yet been implemented/tested")] public int lLegacyNumSamplesPerEpisode;
        #endregion

        #region GROUP 25 - Gap-Free Config
        [Obsolete("this field has not yet been implemented/tested")] public short[] nGapFreeEpochType;
        [Obsolete("this field has not yet been implemented/tested")] public float[] fGapFreeEpochLevel;
        [Obsolete("this field has not yet been implemented/tested")] public int[] lGapFreeEpochDuration;
        [Obsolete("this field has not yet been implemented/tested")] public byte[] nGapFreeDigitalValue;
        [Obsolete("this field has not yet been implemented/tested")] public short nGapFreeEpochStart;
        #endregion
    }
}
