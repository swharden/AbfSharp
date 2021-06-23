using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbfSharp
{
    /// <summary>
    /// This class contains information about the ABF as well as public fields for all original ABFFIO header structure variables.
    /// Most of the important fields are fully implemented, tested, and documented. However, numerous obscure or legacy fields are not. 
    /// It's a safe bet that fields with XML documentation are fully supported.
    /// </summary>
    public abstract class Header
    {
        #region Helpful information with simple names and XML docs

        /// <summary>
        /// Number of ADC samples per second (Hz).
        /// One sample may contain multiple values if multiple ADC channels are in use.
        /// </summary>
        public int SampleRate => (int)(1e6 / fADCSequenceInterval / nADCNumChannels);

        /// <summary>
        /// Time (seconds) between recorded samples.
        /// One sample may contain multiple values if multiple ADC channels are in use.
        /// </summary>
        public float SamplePeriod => 1.0f / SampleRate;

        /// <summary>
        /// Time (milliseconds) between recorded samples.
        /// One sample may contain multiple values if multiple ADC channels are in use.
        /// </summary>
        public float SamplePeriodMS => SamplePeriod / 1e3f;

        /// <summary>
        /// Number of bytes for each sample.
        /// Each sample has one data point for each channel.
        /// </summary>
        /// 
        public int BytesPerSample => BytesPerValue * ChannelCount;

        /// <summary>
        /// Byte location in the ABF file where the data begins.
        /// Note that data values are interleaved across multiple channels.
        /// </summary>
        public int DataPosition => lDataSectionPtr * 512;

        /// <summary>
        /// Number of bytes for the entire data section in the ABF file.
        /// </summary>
        public int DataSize => lActualAcqLength * BytesPerValue;

        /// <summary>
        /// File format version stored in the data file during acquisition
        /// </summary>
        public readonly float FileVersionNumber;

        /// <summary>
        /// Number of sweeps (episodes)
        /// </summary>
        public int SweepCount => lActualEpisodes;

        /// <summary>
        /// Total number of ADC channels
        /// </summary>
        public int ChannelCount => nADCNumChannels;

        /// <summary>
        /// Date and time the ABF recording was initiated
        /// </summary>
        public DateTime StartDateTime
        {
            get
            {
                int datecode = (int)uFileStartDate;

                int day = datecode % 100;
                datecode /= 100;

                int month = datecode % 100;
                datecode /= 100;

                int year = datecode;

                try
                {
                    if (year < 1980 || year >= 2080)
                        throw new InvalidOperationException("unexpected creation date year in header");
                    return new DateTime(year, month, day).AddMilliseconds(uFileStartTimeMS);
                }
                catch
                {
                    return new DateTime(0);
                }
            }
        }

        #endregion

        #region ABFFIO constants
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

        public Tag[] Tags { get; protected set; }

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
        /// Start time of each sweep (fSynchTimeUnit units).
        /// If fixed length sweeps this will be an empty array.
        /// </summary>
        public int[] SynchStartTimes;

        /// <summary>
        /// Length of each sweep (time points * channel count)
        /// </summary>
        public int[] SynchLengths;

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

        #region GROUP 6 - Environmental Information

        /// <summary>
        /// Experiment type: 
        /// 0 = Voltage Clamp
        /// 1 = Current Clamp
        /// </summary>
        public short nExperimentType;

        /// <summary>
        /// Type of experiment (voltage or current clamp)
        /// </summary>
        public ClampType ExperimentType => (ClampType)nExperimentType;

        /// <summary>
        /// Strategy for writing the manually entered information:
        /// 0 = Do not write
        /// 1 = Write each trial
        /// 2 = Prompt each trial
        /// </summary>
        public short nManualInfoStrategy;

        /// <summary>
        /// Numeric identifier #1, e.g. cell identifier
        /// </summary>
        public float fCellID1;

        /// <summary>
        /// Numeric identifier #2, e.g. temperature in °C
        /// </summary>
        public float fCellID2;

        /// <summary>
        /// Numeric identifier #3.
        /// </summary>
        public float fCellID3;

        /// <summary>
        /// Full path to the protocol file used at the time of recording
        /// </summary>
        public string sProtocolPath;

        /// <summary>
        /// Name and version of the software used to record the ABF
        /// </summary>
        public string sCreatorInfo;

        /// <summary>
        /// Name and version of the software used to last modify the ABF
        /// </summary>
        public string sModifierInfo;

        /// <summary>
        /// Whether the comments form is enabled.
        /// WARNING: inconsistent between file and ABFFIO (some extra logic may be requried to properly set this)
        /// </summary>
        public short nCommentsEnable;

        /// <summary>
        /// A text comment for the ABF typed in the protocol editor window
        /// </summary>
        public string sFileComment;

        /// <summary>
        /// Indicates which ADC channels have telegraph enabled.
        /// 1 = enabled, 0 = disabled, index = ADC channel.
        /// </summary>
        public short[] nTelegraphEnable;

        /// <summary>
        /// Telegraphs instrument identifier (index = ADC channel)
        /// </summary>
        public short[] nTelegraphInstrument;

        /// <summary>
        /// Additional gain multiplier of the telegraphed instrument (index = ADC channel)
        /// </summary>
        public float[] fTelegraphAdditGain;

        /// <summary>
        /// Lowpass filter cutoff frequency of Instrument connected to nAutosampleADCNum (index = ADC channel)
        /// </summary>
        public float[] fTelegraphFilter;

        /// <summary>
        /// Patch-clamp membrane capacitance compensation (index = ADC channel)
        /// </summary>
        public float[] fTelegraphMembraneCap;

        /// <summary>
        /// Patch-clamp access resistance (index = ADC channel)
        /// WARNING: values read from file are inconsistent with ABFFIO values
        /// </summary>
        public float[] fTelegraphAccessResistance;

        /// <summary>
        /// I-Clamp (0) or V-Clamp (1) mode per ADC channel.
        /// Currently this field is supported only for MultiClamp.
        /// </summary>
        public short[] nTelegraphMode;

        /// <summary>
        /// Determines whether fDACScaleFactor was telegraphed: 
        /// 1 = telegraphed
        /// 0 = not telegraphed
        /// WARNING: values read from file are inconsistent with ABFFIO values
        /// </summary>
        public short[] nTelegraphDACScaleFactorEnable;

        /// <summary>
        /// I think this is an obsolete field from ABF1 files
        /// </summary>
        [Obsolete("intentionally not implemented")] public short nAutoAnalyseEnable;

        /// <summary>
        /// A GUID which isn't actually GU :-P
        /// I think this uniquely identifies the protocol (not the ABF)
        /// </summary>
        public Guid FileGUID;

        /// <summary>
        /// Use the ABF convention to rearrange bytes and return them as a .NET GUID
        /// </summary>
        protected static Guid MakeGuid(byte[] bytes)
        {
            if (bytes is null || bytes.Length != 16)
                throw new ArgumentException("bytes must be length 16");

            byte[] bytes2 = new byte[16];
            int[] indexes = { 3, 2, 1, 0, 5, 4, 7, 6, 8, 9, 10, 11, 12, 13, 14, 15 };
            for (int i = 0; i < 16; i++)
                bytes2[i] = bytes[indexes[i]];

            return new Guid(bytes);
        }

        /// <summary>
        /// DAC channel holding level (user units)
        /// </summary>
        public float[] fInstrumentHoldingLevel;

        /// <summary>
        /// WARNING: this is calcualted, not read from file
        /// </summary>
        public uint ulFileCRC;

        /// <summary>
        /// WARNING: values read from file are inconsistent with ABFFIO values
        /// </summary>
        public short nCRCEnable;

        #endregion

        #region GROUP 7 - Multi-channel information

        /// <summary>
        /// Type of signal conditioner that was used
        /// 0 = None
        /// 1 = CyberAmp 320/380
        /// </summary>
        public short nSignalType;

        /// <summary>
        /// ADC physical-to-logical channel map. 
        /// The entries are in the physical order 0, 1, 2,..., 14, 15. 
        /// If there are fewer than 16 logical channels in the system, the array is padded with -1.
        /// All channels supported by the hardware are present, even if only a subset is used. 
        /// For example, for the TL-2 the entries would be 7, 6, 5, 4, 3, 2, 1, 0, -1,..., -1.
        /// </summary>
        public short[] nADCPtoLChannelMap;

        /// <summary>
        /// ADC channel sampling sequence. 
        /// This is the order in which the physical ADC channels are sampled. 
        /// If fewer than the maximum number of channels are sampled, pad with -1. 
        /// For example, if two channels are sampled on the TL-2, 
        /// this array will contain 6, 7, -1,..., -1. 
        /// If two channels are sampled on the TL-1, this array will contain 14, 15, -1,..., -1.
        /// </summary>
        public short[] nADCSamplingSeq;

        /// <summary>
        /// ADC programmable gain in physical channel number order (dimensionless)
        /// </summary>
        public float[] fADCProgrammableGain;

        /// <summary>
        /// ADC channel display amplification in physical channel number order (dimensionless)
        /// </summary>
        public float[] fADCDisplayAmplification;

        /// <summary>
        /// ADC channel display offset in physical channel number order (user units)
        /// </summary>
        public float[] fADCDisplayOffset;

        /// <summary>
        /// Instrument scale factor in physical ADC channel number order (Volts at ADC / user unit). 
        /// Programs would normally display this information to the user as user units / volt at ADC.
        /// </summary>
        public float[] fInstrumentScaleFactor;

        /// <summary>
        /// Instrument offset in physical ADC channel number order (user units corresponding to 0 V at the ADC).
        /// </summary>
        public float[] fInstrumentOffset;

        /// <summary>
        /// Signal conditioner gain in physical ADC channel number order (dimensionless).
        /// </summary>
        public float[] fSignalGain;

        /// <summary>
        /// Signal conditioner offset in physical ADC channel number order (user units).
        /// </summary>
        public float[] fSignalOffset;

        /// <summary>
        /// Signal-conditioner lowpass filter corner frequency in physical ADC channel number order (Hz). 
        /// 100000 means lowpass filter is bypassed (i.e. wideband). 
        /// Default = 100000.
        /// </summary>
        public float[] fSignalLowpassFilter;

        /// <summary>
        /// Signal-conditioner highpass filter corner frequency in physical ADC channel number order (Hz). 
        /// 0 means highpass filter is bypassed (i.e. DC coupled). 
        /// -1 means inputs are grounded. 
        /// </summary>
        public float[] fSignalHighpassFilter;

        /// <summary>
        /// Type of Low Pass filter for each ADC channel:
        /// 0 = None
        /// 1 = External
        /// 2 = Simple RC
        /// 3 = Bessell
        /// 4 = Butterworth
        /// </summary>
        public byte[] nLowpassFilterType;

        /// <summary>
        /// Type of High Pass filter for each ADC channel:
        /// 0 = None
        /// 1 = External
        /// 2 = Simple RC
        /// 3 = Bessell
        /// 4 = Butterworth
        /// </summary>
        public byte[] nHighpassFilterType;

        /// <summary>
        /// Only supported in ABF2 files
        /// </summary>
        public byte[] bHumFilterEnable;

        /// <summary>
        /// ADC channel name in physical channel number order
        /// </summary>
        public string[] sADCChannelName;

        /// <summary>
        /// The user units for ADC channels in physical channel number order
        /// </summary>
        public string[] sADCUnits;

        /// <summary>
        /// Determines whether fDACScaleFactor was telegraphed: 
        /// 1 = telegraphed
        /// 0 = not telegraphed
        /// </summary>
        public float[] fDACScaleFactor;

        /// <summary>
        /// DAC channel holding level (user units)
        /// </summary>
        public float[] fDACHoldingLevel;

        /// <summary>
        /// Calibration factor for each DAC
        /// </summary>
        public float[] fDACCalibrationFactor;

        /// <summary>
        /// Calibration offset for each DAC
        /// </summary>
        public float[] fDACCalibrationOffset;

        /// <summary>
        /// Name for each DAC channel
        /// </summary>
        public string[] sDACChannelName;

        /// <summary>
        /// User units for each DAC channel
        /// </summary>
        public string[] sDACChannelUnits;

        #endregion

        #region GROUP 9 - Epoch Waveform and Pulses

        /// <summary>
        /// Enable digital outputs: 
        /// 0 = No
        /// 1 = Yes
        /// </summary>
        public short nDigitalEnable;

        /// <summary>
        /// Active DAC channel, i.e. the one used for waveform generation.
        /// </summary>
        public short nActiveDACChannel;

        /// <summary>
        /// Not used in ABF1 files
        /// </summary>
        public short nDigitalDACChannel;

        /// <summary>
        /// Holding value for digital output.
        /// </summary>
        public short nDigitalHolding;

        /// <summary>
        /// Inter-sweep digital holding value:
        /// 0 = Use holding value
        /// 1 = Use last epoch value
        /// </summary>
        public short nDigitalInterEpisode;

        /// <summary>
        /// Epoch value for digital train output in epoch order:
        /// 0000 = Disabled
        /// 0*000 = Generates digital train on bit 3
        /// Train period and pulse width can be controlled by the user list.
        /// </summary>
        public short nDigitalTrainActiveLogic;

        /// <summary>
        /// Epoch value for digital output (0...15)
        /// </summary>
        public short[] nDigitalValue;

        /// <summary>
        /// Epoch duration increment in physical DAC channel order then epoch order (in sequence counts)
        /// </summary>
        public short[] nDigitalTrainValue;

        public byte[] bEpochCompression;

        /// <summary>
        /// Analog waveform enabled (by ADC).
        /// 0 = No
        /// 1 = Yes
        /// </summary>
        public short[] nWaveformEnable;

        /// <summary>
        /// Analog waveform source: 
        /// 0 = Disable
        /// 1 = Generate waveform from epoch definitions
        /// 2 = Generate waveform from a DAC file.
        /// </summary>
        public short[] nWaveformSource;

        /// <summary>
        /// Inter-sweep holding level:
        /// 0 = Use holding level
        /// 1 = Use last epoch amplitude
        /// </summary>
        public short[] nInterEpisodeLevel;

        /// <summary>
        /// Epoch type (one per epoch):
        /// 0 = Disabled
        /// 1 = Step
        /// 2 = Ramp
        /// </summary>
        public short[] nEpochType;

        /// <summary>
        /// Epoch initial level (user units)
        /// </summary>
        public float[] fEpochInitLevel;

        [Obsolete("I think this is calculated, not read from file")]
        public float[] fEpochFinalLevel;

        /// <summary>
        /// Epoch level increment (user units)
        /// </summary>
        public float[] fEpochLevelInc;

        /// <summary>
        /// Epoch initial duration (in sequence counts)
        /// </summary>
        public int[] lEpochInitDuration;

        /// <summary>
        /// Epoch duration increment (in sequence counts)
        /// </summary>
        public int[] lEpochDurationInc;

        [Obsolete("I think this is calculated, not read from file")]
        public short[] nEpochTableRepetitions;

        [Obsolete("I think this is calculated, not read from file")]
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

        #region helper functions to convert data to/from weird ABF formats

        protected string VersionString(int a, int b, int c, int d)
        {
            int[] p = { a, b, c, d };
            for (int i = 0; i < p.Length; i++)
            {
                if (p[i] < 0)
                    p[i] = 0;
                if (p[i] > 999)
                    p[i] = 0;
            }
            return $"{p[0]}.{p[1]}.{p[2]}.{p[3]}";
        }

        #endregion

        #region funtions to read arrays from data streams

        protected static Int16[] ReadArrayInt16(BinaryReader reader, int size)
        {
            Int16[] arr = new Int16[size];
            for (int i = 0; i < size; i++)
                arr[i] = reader.ReadInt16();
            return arr;
        }

        protected static Int32[] ReadArrayInt32(BinaryReader reader, int size)
        {
            Int32[] arr = new Int32[size];
            for (int i = 0; i < size; i++)
                arr[i] = reader.ReadInt32();
            return arr;
        }

        protected static Single[] ReadArraySingle(BinaryReader reader, int size)
        {
            Single[] arr = new Single[size];
            for (int i = 0; i < size; i++)
                arr[i] = reader.ReadSingle();
            return arr;
        }

        protected static string[] ReadArrayStrings(BinaryReader reader, int stringCount, int stringSize)
        {
            string[] strings = new string[stringCount];
            for (int i = 0; i < stringCount; i++)
                strings[i] = Encoding.ASCII.GetString(reader.ReadBytes(stringSize)).Replace("\u0000", "").Trim();
            return strings;
        }

        protected static string ReadString(BinaryReader reader, int length)
        {
            byte[] bytes = reader.ReadBytes(length);
            string path = System.Text.Encoding.ASCII.GetString(bytes).Trim();
            return path.StartsWith("?") ? "" : path;
        }

        #endregion
    }
}
