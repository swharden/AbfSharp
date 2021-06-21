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
        public const int ABF_ADCCOUNT = 16; // number of ADC channels supported.
        public const int ABF_DACCOUNT = 8; // number of DAC channels supported.
        public const int ABF_EPOCHCOUNT = 50; // number of waveform epochs supported.
        public const int ABF_ADCUNITLEN = 8; // length of ADC units strings
        public const int ABF_ADCNAMELEN = 10; // length of actual ADC channel name strings
        public const int ABF_DACUNITLEN = 8; // length of DAC units strings
        public const int ABF_DACNAMELEN = 10; // length of DAC channel name strings
        public const int ABF_USERLISTLEN = 256; // length of the user list (V1.6)
        public const int ABF_USERLISTCOUNT = ABF_DACCOUNT; // number of independent user lists (V1.6)       
        public const int ABF_FILECOMMENTLEN = 128; // length of file comment string (V1.6)
        public const int ABF_PATHLEN = 256; // length of full path, used for DACFile and Protocol name.
        public const int ABF_CREATORINFOLEN = 16; // length of file creator info string
        public const int ABF_ARITHMETICOPLEN = 2; // length of the Arithmetic operator field
        public const int ABF_ARITHMETICUNITSLEN = 8; // length of arithmetic units string
        public const int ABF_STATS_REGIONS = 24; // The number of independent statistics regions. // ST-91
        public const int ABF_TAGCOMMENTLEN = 56; // length of tag comment string
        public const int ABFH_HOLDINGFRACTION = 64; // helps calculate the pre-epoch duration
        public const int ABF_ADCNAMELEN_USER = 8; // length of user-entered ADC channel name strings
        public const int ABF_OLDFILECOMMENTLEN = 56; // length of file comment string (pre V1.6)
        public const int ABF_BLOCKSIZE = 512; // Size of block alignment in ABF files.
        public const int PCLAMP6_MAXSWEEPLENGTH = 16384; // Maximum multiplexed sweep length supported by pCLAMP6 apps.
        public const int PCLAMP7_MAXSWEEPLEN_PERCHAN = 1032258; // Maximum per channel sweep length supported by pCLAMP7 apps.
        public const int PCLAMP11_MAXSWEEPLEN_PERCHAN = 5161290; // Maximum per channel sweep length supported by pCLAMP11 apps. //ST-1
        public const int ABF_MAX_SWEEPS_PER_AVERAGE = 65500; // The maximum number of sweeps that can be combined into a cumulative average 
        public const int ABF_MAX_TRIAL_SAMPLES = 0x7FFFFFFF; // Maximum length of acquisition supported (samples)
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

        /// <summary>
        /// Block number of start of Data section
        /// </summary>
        public int lDataSectionPtr;

        /// <summary>
        /// Block number of start of Tags section
        /// </summary>
        public int lTagSectionPtr;

        /// <summary>
        /// Number of tags in this ABF
        /// </summary>
        public int lNumTagEntries;

        /// <summary>
        /// Block number of start of the Scope Configuration Section
        /// </summary>
        public int lScopeConfigPtr;

        /// <summary>
        /// Number of items in the scope configuration section
        /// </summary>
        public int lNumScopes;

        /// <summary>
        /// Block number of the start of the delta section
        /// </summary>
        public int lDeltaArrayPtr;

        /// <summary>
        /// Number of items in the delta section
        /// </summary>
        public int lNumDeltas;

        /// <summary>
        /// Block number of the start of the voice tag section
        /// </summary>
        public int lVoiceTagPtr;

        /// <summary>
        /// Number of voice tags
        /// </summary>
        public int lVoiceTagEntries;

        /// <summary>
        /// Block number of the start of the Synch Array section
        /// </summary>
        public int lSynchArrayPtr;

        /// <summary>
        /// Number of items in the sync array section
        /// </summary>
        public int lSynchArraySize;

        /// <summary>
        /// Format of data points in memory:
        /// 0 = 2-byte integer
        /// 1 = IEEE 4 byte float
        /// </summary>
        public short nDataFormat;

        /// <summary>
        /// Number of bytes for each data point.
        /// Each sample is this times the number of channels.
        /// </summary>
        public int BytesPerValue =>
            nDataFormat switch
            {
                0 => 2,
                1 => 4,
                _ => throw new NotImplementedException($"unsupported nDataFormat: {nDataFormat}"),
            };

        /// <summary>
        /// ADC Channel Scanning Mode: 
        /// 0=Multiplexed
        /// 1=Simultaneous Scanning (unimplemented?)
        /// </summary>
        public short nSimultaneousScan;

        /// <summary>
        /// Block position of the start of the statistics section
        /// </summary>
        public int lStatisticsConfigPtr;

        /// <summary>
        /// Number of items in the annotations section
        /// </summary>
        public int lAnnotationSectionPtr;

        /// <summary>
        /// Number of annotations
        /// </summary>
        public int lNumAnnotations;

        /// <summary>
        /// Block number of start of DAC file section
        /// </summary>
        public int[] lDACFilePtr = new int[ABF_DACCOUNT];

        /// <summary>
        /// Number of sweeps in the DAC file section. Sweeps are not multiplexed.
        /// </summary>
        public int[] lDACFileNumEpisodes = new int[ABF_DACCOUNT];

        #endregion

        #region GROUP 3 - Trial hierarchy information

        /// <summary>
        /// Number of ADC input channels recorded.
        /// </summary>
        public short nADCNumChannels;

        /// <summary>
        /// Number of ADC input channels recorded.
        /// </summary>
        public int ChannelCount => nADCNumChannels;

        /// <summary>
        /// Number of microseconds between samples (divided by number of channels).
        /// For a single-channel 20 kHz recording this will be 50.
        /// </summary>
        public float fADCSequenceInterval;

        /// <summary>
        /// I'm not sure exactly what this does...
        /// </summary>
        public uint uFileCompressionRatio;

        /// <summary>
        /// I'm not sure exactly what this does...
        /// </summary>
        public byte bEnableFileCompression;

        /// <summary>
        /// This sync time unit relates sample rate with actual clock time.
        /// Sweeps (synch array) and tags are logged in synch time units.
        /// TODO: how to convert to/from a time period?
        /// </summary>
        public float fSynchTimeUnit;

        /// <summary>
        /// TODO: what exactly is this?
        /// </summary>
        public float fSecondsPerRun;

        /// <summary>
        /// The total number of samples per episode, for the recorded channels only.
        /// This does not include channels which are acquired but not recorded.
        /// This is the number of samples per episode per channel, times the number of recorded channels.
        /// If you want the samples per episode for one channel, you must divide this by get_channel_count_recorded().
        /// </summary>
        public int lNumSamplesPerEpisode;

        public int lPreTriggerSamples;
        public int lEpisodesPerRun;
        public int lRunsPerTrial;
        public int lNumberOfTrials;
        public short nAveragingMode;
        public short nUndoRunCount;
        public short nFirstEpisodeInRun;
        public float fTriggerThreshold;

        /// <summary>
        /// I'm not sure exactly what this is.
        /// ABFFIO produces a different number for some ABF1 files.
        /// </summary>
        public short nTriggerSource;
        public short nTriggerAction;
        public short nTriggerPolarity;
        public float fScopeOutputInterval;
        public float fEpisodeStartToStart;
        public float fRunStartToStart;
        public float fTrialStartToStart;
        public int lAverageCount;
        public short nAutoTriggerStrategy;
        public float fFirstRunDelayS;

        [Obsolete("not implemented")]
        public uint nTriggerTimeout;
        #endregion

        #region GROUP 4 - Display Parameters
        // I think these control the view on the screen when you open an ABF
        [Obsolete("not implemented")] public short nDataDisplayMode;
        [Obsolete("not implemented")] public short nChannelStatsStrategy;
        [Obsolete("not implemented")] public int lSamplesPerTrace;
        [Obsolete("not implemented")] public int lStartDisplayNum;
        [Obsolete("not implemented")] public int lFinishDisplayNum;
        [Obsolete("not implemented")] public short nShowPNRawData;
        [Obsolete("not implemented")] public float fStatisticsPeriod;
        [Obsolete("not implemented")] public int lStatisticsMeasurements;
        [Obsolete("not implemented")] public short nStatisticsSaveStrategy;
        #endregion

        #region GROUP 5 - Hardware information

        /// <summary>
        /// ADC positive full-scale input in volts (e.g. 10.00V)
        /// </summary>
        public float fADCRange;

        /// <summary>
        /// DAC positive full-scale range in volts
        /// </summary>
        public float fDACRange;

        /// <summary>
        /// Number of ADC counts corresponding to the positive full-scale voltage in ADCRange (e.g. 2000, 2048, 32000 or 32768)
        /// </summary>
        public int lADCResolution;

        /// <summary>
        /// Number of DAC counts corresponding to the positive full-scale voltage in DACRange
        /// </summary>
        public int lDACResolution;

        // I donp't think the rest of these aren't supported in ABF1 files
        [Obsolete("not implemented")] public short nDigitizerADCs;
        [Obsolete("not implemented")] public short nDigitizerDACs;
        [Obsolete("not implemented")] public short nDigitizerTotalDigitalOuts;
        [Obsolete("not implemented")] public short nDigitizerSynchDigitalOuts;
        [Obsolete("not implemented")] public short nDigitizerType;

        #endregion

        #region GROUP 6 Environmental Information
        public short nExperimentType;
        public short nManualInfoStrategy;
        public float fCellID1;
        public float fCellID2;
        public float fCellID3;
        public string sProtocolPath;
        public string sCreatorInfo;
        public string sModifierInfo;
        public short nCommentsEnable;
        public string sFileComment;
        public short[] nTelegraphEnable;
        public short[] nTelegraphInstrument;
        public float[] fTelegraphAdditGain;
        public float[] fTelegraphFilter;
        public float[] fTelegraphMembraneCap;
        public float[] fTelegraphAccessResistance;
        public short[] nTelegraphMode;
        public short[] nTelegraphDACScaleFactorEnable;
        public short nAutoAnalyseEnable;
        public Guid FileGUID;
        public float[] fInstrumentHoldingLevel;
        public uint ulFileCRC;
        public short nCRCEnable;
        #endregion

        #region GROUP 7 - Multi-channel information
        public short nSignalType;
        public short[] nADCPtoLChannelMap;
        public short[] nADCSamplingSeq;
        public float[] fADCProgrammableGain;
        public float[] fADCDisplayAmplification;
        public float[] fADCDisplayOffset;
        public float[] fInstrumentScaleFactor;
        public float[] fInstrumentOffset;
        public float[] fSignalGain;
        public float[] fSignalOffset;
        public float[] fSignalLowpassFilter;
        public float[] fSignalHighpassFilter;
        public string nLowpassFilterType;
        public string nHighpassFilterType;
        public byte[] bHumFilterEnable;
        public string sADCChannelName;
        public string sADCUnits;
        public float[] fDACScaleFactor;
        public float[] fDACHoldingLevel;
        public float[] fDACCalibrationFactor;
        public float[] fDACCalibrationOffset;
        public string sDACChannelName;
        public string sDACChannelUnits;
        #endregion

        #region GROUP 9 - Epoch Waveform and Pulses
        public short nDigitalEnable;
        public short nActiveDACChannel;
        public short nDigitalDACChannel;
        public short nDigitalHolding;
        public short nDigitalInterEpisode;
        public short nDigitalTrainActiveLogic;
        public short[] nDigitalValue;
        public short[] nDigitalTrainValue;
        public byte[] bEpochCompression;
        public short[] nWaveformEnable;
        public short[] nWaveformSource;
        public short[] nInterEpisodeLevel;
        public short[] nEpochType;
        public float[] fEpochInitLevel;
        public float[] fEpochFinalLevel;
        public float[] fEpochLevelInc;
        public int[] lEpochInitDuration;
        public int[] lEpochDurationInc;
        public short[] nEpochTableRepetitions;
        public float[] fEpochTableStartToStartInterval;
        #endregion

        #region GROUP 10 - DAC Output File
        public float[] fDACFileScale;
        public float[] fDACFileOffset;
        public int[] lDACFileEpisodeNum;
        public short[] nDACFileADCNum;
        public string sDACFilePath;
        #endregion

        #region GROUP 11a - Presweep (conditioning) pulse train
        public short[] nConditEnable;
        public int[] lConditNumPulses;
        public float[] fBaselineDuration;
        public float[] fBaselineLevel;
        public float[] fStepDuration;
        public float[] fStepLevel;
        public float[] fPostTrainPeriod;
        public float[] fPostTrainLevel;
        public float[] fCTStartLevel;
        public float[] fCTEndLevel;
        public float[] fCTIntervalDuration;
        public float[] fCTStartToStartInterval;
        #endregion

        #region GROUP 11b - Membrane Test Between Sweeps
        public short[] nMembTestEnable;
        public float[] fMembTestPreSettlingTimeMS;
        public float[] fMembTestPostSettlingTimeMS;
        #endregion

        #region GROUP 11c - PreSignal test pulse
        public short[] nPreSignalEnable;
        public float[] fPreSignalPreStepDuration;
        public float[] fPreSignalPreStepLevel;
        public float[] fPreSignalStepDuration;
        public float[] fPreSignalStepLevel;
        public float[] fPreSignalPostStepDuration;
        public float[] fPreSignalPostStepLevel;
        #endregion

        #region GROUP 11d - Hum Silncer Adapt between sweeps
        public short nAdaptEnable;
        public float fInterSweepAdaptTimeS;
        #endregion

        #region GROUP 12 - Variable parameter user list
        public short[] nULEnable;
        public short[] nULParamToVary;
        public short[] nULRepeat;
        public string sULParamValueList;
        #endregion

        #region GROUP 13 - Statistics measurements
        public short nStatsEnable;
        public ushort nStatsActiveChannels;
        public ushort nStatsSearchRegionFlags;
        public short nStatsSmoothing;
        public short nStatsSmoothingEnable;
        public short nStatsBaseline;
        public short nStatsBaselineDAC;
        public int lStatsBaselineStart;
        public int lStatsBaselineEnd;
        public int[] lStatsMeasurements;
        public int[] lStatsStart;
        public int[] lStatsEnd;
        public short[] nRiseBottomPercentile;
        public short[] nRiseTopPercentile;
        public short[] nDecayBottomPercentile;
        public short[] nDecayTopPercentile;
        public short[] nStatsChannelPolarity;
        public short[] nStatsSearchMode;
        public short[] nStatsSearchDAC;
        #endregion

        #region GROUP 14 - Channel Arithmetic
        public short nArithmeticEnable;
        public short nArithmeticExpression;
        public float fArithmeticUpperLimit;
        public float fArithmeticLowerLimit;
        public short nArithmeticADCNumA;
        public short nArithmeticADCNumB;
        public float fArithmeticK1;
        public float fArithmeticK2;
        public float fArithmeticK3;
        public float fArithmeticK4;
        public float fArithmeticK5;
        public float fArithmeticK6;
        public string sArithmeticOperator;
        public string sArithmeticUnits;
        #endregion

        #region GROUP 15 - Leak subtraction
        public short nPNPosition;
        public short nPNNumPulses;
        public short nPNPolarity;
        public float fPNSettlingTime;
        public float fPNInterpulse;
        public short[] nLeakSubtractType;
        public float[] fPNHoldingLevel;
        public short[] nLeakSubtractADCIndex;
        #endregion

        #region GROUP 16 - Miscellaneous variables
        public short nLevelHysteresis;
        public int lTimeHysteresis;
        public short nAllowExternalTags;
        public short nAverageAlgorithm;
        public float fAverageWeighting;
        public short nUndoPromptStrategy;
        public short nTrialTriggerSource;
        public short nStatisticsDisplayStrategy;
        public short nExternalTagType;
        public int lHeaderSize;
        public short nStatisticsClearStrategy;
        public short nEnableFirstLastHolding;

        #endregion

        #region GROUP 17 - Trains parameters
        public int[] lEpochPulsePeriod;
        public int[] lEpochPulseWidth;
        #endregion

        #region GROUP 18 - Application version data
        public short nCreatorMajorVersion;
        public short nCreatorMinorVersion;
        public short nCreatorBugfixVersion;
        public short nCreatorBuildVersion;
        public short nModifierMajorVersion;
        public short nModifierMinorVersion;
        public short nModifierBugfixVersion;
        public short nModifierBuildVersion;
        #endregion

        #region GROUP 19 - LTP protocol
        public short nLTPType;
        public short[] nLTPUsageOfDAC;
        public short[] nLTPPresynapticPulses;
        #endregion

        #region GROUP 20 - Digidata 132x Trigger out flag
        public short nScopeTriggerOut;
        #endregion

        #region GROUP 21 - Epoch resistance
        public string sEpochResistanceSignalName;
        public short[] nEpochResistanceState;
        #endregion

        #region GROUP 22 - Alternating episodic mode
        public short nAlternateDACOutputState;
        public short nAlternateDigitalOutputState;
        public short[] nAlternateDigitalValue;
        public short[] nAlternateDigitalTrainValue;
        #endregion

        #region GROUP 23 - Post-processing actions
        public float[] fPostProcessLowpassFilter;
        public string nPostProcessLowpassFilterType;
        #endregion

        #region GROUP 24 - Legacy gear shift info
        public float fLegacyADCSequenceInterval;
        public float fLegacyADCSecondSequenceInterval;
        public int lLegacyClockChange;
        public int lLegacyNumSamplesPerEpisode;
        #endregion

        #region GROUP 25 - Gap-Free Config
        public short[] nGapFreeEpochType;
        public float[] fGapFreeEpochLevel;
        public int[] lGapFreeEpochDuration;
        public byte[] nGapFreeDigitalValue;
        public short nGapFreeEpochStart;
        #endregion
    }
}
