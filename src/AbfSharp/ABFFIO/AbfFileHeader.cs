using System;
using System.Runtime.InteropServices;

namespace AbfSharp.ABFFIO
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public unsafe struct AbfFileHeader
    {
        // GROUP #1 - File ID and size information
        public Single fFileVersionNumber;
        public Int16 nOperationMode;
        public Int32 lActualAcqLength;
        public Int16 nNumPointsIgnored;
        public Int32 lActualEpisodes;
        public UInt32 uFileStartDate;
        public UInt32 uFileStartTimeMS;
        public Int32 lStopwatchTime;
        public Single fHeaderVersionNumber;
        public Int16 nFileType;

        // GROUP #2 - File Structure
        public Int32 lDataSectionPtr;
        public Int32 lTagSectionPtr;
        public Int32 lNumTagEntries;
        public Int32 lScopeConfigPtr;
        public Int32 lNumScopes;
        public Int32 lDeltaArrayPtr;
        public Int32 lNumDeltas;
        public Int32 lVoiceTagPtr;
        public Int32 lVoiceTagEntries;
        public Int32 lSynchArrayPtr;
        public Int32 lSynchArraySize;
        public Int16 nDataFormat;
        public Int16 nSimultaneousScan;
        public Int32 lStatisticsConfigPtr;
        public Int32 lAnnotationSectionPtr;
        public Int32 lNumAnnotations;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Int32[] lDACFilePtr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Int32[] lDACFileNumEpisodes;

        // GROUP #3 - Trial hierarchy information
        public Int16 nADCNumChannels;
        public Single fADCSequenceInterval;
        public UInt32 uFileCompressionRatio;
        public Byte bEnableFileCompression;
        public Single fSynchTimeUnit;
        public Single fSecondsPerRun;
        public Int32 lNumSamplesPerEpisode;
        public Int32 lPreTriggerSamples;
        public Int32 lEpisodesPerRun;
        public Int32 lRunsPerTrial;
        public Int32 lNumberOfTrials;
        public Int16 nAveragingMode;
        public Int16 nUndoRunCount;
        public Int16 nFirstEpisodeInRun;
        public Single fTriggerThreshold;
        public Int16 nTriggerSource;
        public Int16 nTriggerAction;
        public Int16 nTriggerPolarity;
        public Single fScopeOutputInterval;
        public Single fEpisodeStartToStart;
        public Single fRunStartToStart;
        public Single fTrialStartToStart;
        public Int32 lAverageCount;
        public Int16 nAutoTriggerStrategy;
        public Single fFirstRunDelayS;
        public UInt32 nTriggerTimeout;

        // GROUP #4 - Display Parameters
        public Int16 nDataDisplayMode;
        public Int16 nChannelStatsStrategy;
        public Int32 lSamplesPerTrace;
        public Int32 lStartDisplayNum;
        public Int32 lFinishDisplayNum;
        public Int16 nShowPNRawData;
        public Single fStatisticsPeriod;
        public Int32 lStatisticsMeasurements;
        public Int16 nStatisticsSaveStrategy;

        // GROUP #5 - Hardware information
        public Single fADCRange;
        public Single fDACRange;
        public Int32 lADCResolution;
        public Int32 lDACResolution;
        public Int16 nDigitizerADCs;
        public Int16 nDigitizerDACs;
        public Int16 nDigitizerTotalDigitalOuts;
        public Int16 nDigitizerSynchDigitalOuts;
        public Int16 nDigitizerType;

        // GROUP #6 Environmental Information
        public Int16 nExperimentType;
        public Int16 nManualInfoStrategy;
        public Single fCellID1;
        public Single fCellID2;
        public Single fCellID3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.ABF_PATHLEN)] public string sProtocolPath;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.ABF_CREATORINFOLEN)] public string sCreatorInfo;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.ABF_CREATORINFOLEN)] public string sModifierInfo;
        public Int16 nCommentsEnable;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.ABF_FILECOMMENTLEN)] public string sFileComment;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Int16[] nTelegraphEnable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Int16[] nTelegraphInstrument;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Single[] fTelegraphAdditGain;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Single[] fTelegraphFilter;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Single[] fTelegraphMembraneCap;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Single[] fTelegraphAccessResistance;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Int16[] nTelegraphMode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Int16[] nTelegraphDACScaleFactorEnable;
        public Int16 nAutoAnalyseEnable;
        public Guid FileGUID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fInstrumentHoldingLevel;
        public UInt32 ulFileCRC;
        public Int16 nCRCEnable;

        // GROUP #7 - Multi-channel information
        public Int16 nSignalType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Int16[] nADCPtoLChannelMap;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Int16[] nADCSamplingSeq;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Single[] fADCProgrammableGain;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Single[] fADCDisplayAmplification;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Single[] fADCDisplayOffset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Single[] fInstrumentScaleFactor;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Single[] fInstrumentOffset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Single[] fSignalGain;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Single[] fSignalOffset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Single[] fSignalLowpassFilter;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Single[] fSignalHighpassFilter;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Byte[] nLowpassFilterType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Byte[] nHighpassFilterType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Byte[] bHumFilterEnable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public FixedLengthStrings.CharArray10[] sADCChannelName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public FixedLengthStrings.CharArray8[] sADCUnits;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fDACScaleFactor;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fDACHoldingLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fDACCalibrationFactor;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fDACCalibrationOffset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public FixedLengthStrings.CharArray10[] sDACChannelName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public FixedLengthStrings.CharArray8[] sDACChannelUnits;

        // GROUP #9 - Epoch Waveform and Pulses
        public Int16 nDigitalEnable;
        public Int16 nActiveDACChannel;
        public Int16 nDigitalDACChannel;
        public Int16 nDigitalHolding;
        public Int16 nDigitalInterEpisode;
        public Int16 nDigitalTrainActiveLogic;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_EPOCHCOUNT)] public Int16[] nDigitalValue;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_EPOCHCOUNT)] public Int16[] nDigitalTrainValue;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_EPOCHCOUNT)] public Byte[] bEpochCompression;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Int16[] nWaveformEnable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Int16[] nWaveformSource;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Int16[] nInterEpisodeLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT * Constants.ABF_EPOCHCOUNT)] public Int16[] nEpochType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT * Constants.ABF_EPOCHCOUNT)] public Single[] fEpochInitLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT * Constants.ABF_EPOCHCOUNT)] public Single[] fEpochFinalLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT * Constants.ABF_EPOCHCOUNT)] public Single[] fEpochLevelInc;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT * Constants.ABF_EPOCHCOUNT)] public Int32[] lEpochInitDuration;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT * Constants.ABF_EPOCHCOUNT)] public Int32[] lEpochDurationInc;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Int16[] nEpochTableRepetitions;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fEpochTableStartToStartInterval;
        // GROUP #10 - DAC Output File
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fDACFileScale;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fDACFileOffset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Int32[] lDACFileEpisodeNum;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Int16[] nDACFileADCNum;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.ABF_DACCOUNT * Constants.ABF_PATHLEN)] public string sDACFilePath;

        // GROUP #11a - Presweep (conditioning) pulse train
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Int16[] nConditEnable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Int32[] lConditNumPulses;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fBaselineDuration;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fBaselineLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fStepDuration;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fStepLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fPostTrainPeriod;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fPostTrainLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT * Constants.ABF_EPOCHCOUNT)] public Single[] fCTStartLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT * Constants.ABF_EPOCHCOUNT)] public Single[] fCTEndLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT * Constants.ABF_EPOCHCOUNT)] public Single[] fCTIntervalDuration;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fCTStartToStartInterval;

        // GROUP #11b - Membrane Test Between Sweeps
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Int16[] nMembTestEnable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fMembTestPreSettlingTimeMS;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fMembTestPostSettlingTimeMS;

        // GROUP #11c - PreSignal test pulse
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Int16[] nPreSignalEnable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fPreSignalPreStepDuration;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fPreSignalPreStepLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fPreSignalStepDuration;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fPreSignalStepLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fPreSignalPostStepDuration;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fPreSignalPostStepLevel;

        // GROUP #11d - Hum Silncer Adapt between sweeps
        public Int16 nAdaptEnable;
        public Single fInterSweepAdaptTimeS;

        // GROUP #12 - Variable parameter user list
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_USERLISTCOUNT)] public Int16[] nULEnable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_USERLISTCOUNT)] public Int16[] nULParamToVary;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_USERLISTCOUNT)] public Int16[] nULRepeat;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.ABF_USERLISTCOUNT * Constants.ABF_USERLISTLEN)] public string sULParamValueList;

        // GROUP #13 - Statistics measurements
        public Int16 nStatsEnable;
        public UInt16 nStatsActiveChannels;
        public UInt16 nStatsSearchRegionFlags;
        public Int16 nStatsSmoothing;
        public Int16 nStatsSmoothingEnable;
        public Int16 nStatsBaseline;
        public Int16 nStatsBaselineDAC;
        public Int32 lStatsBaselineStart;
        public Int32 lStatsBaselineEnd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_STATS_REGIONS)] public Int32[] lStatsMeasurements;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_STATS_REGIONS)] public Int32[] lStatsStart;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_STATS_REGIONS)] public Int32[] lStatsEnd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_STATS_REGIONS)] public Int16[] nRiseBottomPercentile;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_STATS_REGIONS)] public Int16[] nRiseTopPercentile;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_STATS_REGIONS)] public Int16[] nDecayBottomPercentile;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_STATS_REGIONS)] public Int16[] nDecayTopPercentile;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Int16[] nStatsChannelPolarity;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_STATS_REGIONS)] public Int16[] nStatsSearchMode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_STATS_REGIONS)] public Int16[] nStatsSearchDAC;

        // GROUP #14 - Channel Arithmetic
        public Int16 nArithmeticEnable;
        public Int16 nArithmeticExpression;
        public Single fArithmeticUpperLimit;
        public Single fArithmeticLowerLimit;
        public Int16 nArithmeticADCNumA;
        public Int16 nArithmeticADCNumB;
        public Single fArithmeticK1;
        public Single fArithmeticK2;
        public Single fArithmeticK3;
        public Single fArithmeticK4;
        public Single fArithmeticK5;
        public Single fArithmeticK6;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.ABF_ARITHMETICOPLEN)] public string sArithmeticOperator;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.ABF_ARITHMETICUNITSLEN)] public string sArithmeticUnits;

        // GROUP #15 - Leak subtraction
        public Int16 nPNPosition;
        public Int16 nPNNumPulses;
        public Int16 nPNPolarity;
        public Single fPNSettlingTime;
        public Single fPNInterpulse;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Int16[] nLeakSubtractType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Single[] fPNHoldingLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Int16[] nLeakSubtractADCIndex;

        // GROUP #16 - Miscellaneous variables
        public Int16 nLevelHysteresis;
        public Int32 lTimeHysteresis;
        public Int16 nAllowExternalTags;
        public Int16 nAverageAlgorithm;
        public Single fAverageWeighting;
        public Int16 nUndoPromptStrategy;
        public Int16 nTrialTriggerSource;
        public Int16 nStatisticsDisplayStrategy;
        public Int16 nExternalTagType;
        public Int32 lHeaderSize;
        public Int16 nStatisticsClearStrategy;
        public Int16 nEnableFirstLastHolding;

        // GROUP #17 - Trains parameters
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT * Constants.ABF_EPOCHCOUNT)] public Int32[] lEpochPulsePeriod;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT * Constants.ABF_EPOCHCOUNT)] public Int32[] lEpochPulseWidth;

        // GROUP #18 - Application version data
        public Int16 nCreatorMajorVersion;
        public Int16 nCreatorMinorVersion;
        public Int16 nCreatorBugfixVersion;
        public Int16 nCreatorBuildVersion;
        public Int16 nModifierMajorVersion;
        public Int16 nModifierMinorVersion;
        public Int16 nModifierBugfixVersion;
        public Int16 nModifierBuildVersion;

        // GROUP #19 - LTP protocol
        public Int16 nLTPType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Int16[] nLTPUsageOfDAC;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Int16[] nLTPPresynapticPulses;

        // GROUP #20 - Digidata 132x Trigger out flag
        public Int16 nScopeTriggerOut;

        // GROUP #21 - Epoch resistance
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.ABF_DACCOUNT * Constants.ABF_ADCNAMELEN)] public string sEpochResistanceSignalName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT)] public Int16[] nEpochResistanceState;

        // GROUP #22 - Alternating episodic mode
        public Int16 nAlternateDACOutputState;
        public Int16 nAlternateDigitalOutputState;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_EPOCHCOUNT)] public Int16[] nAlternateDigitalValue;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_EPOCHCOUNT)] public Int16[] nAlternateDigitalTrainValue;

        // GROUP #23 - Post-processing actions
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_ADCCOUNT)] public Single[] fPostProcessLowpassFilter;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.ABF_ADCCOUNT)] public string nPostProcessLowpassFilterType;

        // GROUP #24 - Legacy gear shift info
        public Single fLegacyADCSequenceInterval;
        public Single fLegacyADCSecondSequenceInterval;
        public Int32 lLegacyClockChange;
        public Int32 lLegacyNumSamplesPerEpisode;

        // GROUP #25 - Gap-Free Config
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT * Constants.ABF_EPOCHCOUNT)] public Int16[] nGapFreeEpochType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT * Constants.ABF_EPOCHCOUNT)] public Single[] fGapFreeEpochLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT * Constants.ABF_EPOCHCOUNT)] public Int32[] lGapFreeEpochDuration;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.ABF_DACCOUNT * Constants.ABF_EPOCHCOUNT)] public Byte[] nGapFreeDigitalValue;
        public Int16 nGapFreeEpochStart;
    };
}
