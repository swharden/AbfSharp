using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AbfSharp.HeaderData
{
    /// <summary>
    /// A version-agnostic ABF header using idiomatic C# names and XML documentation
    /// </summary>
    public class Header
    {
        // varible names and descriptions are inspired by official documentation
        // http://mdc.custhelp.com/euf/assets/software/FSP_ABFHelp_2.03.pdf

        /// <summary>
        /// Access to low-level ABF1 header variables in memory with names that match the official documentation.
        /// Will be null if this is not an ABF1 file.
        /// </summary>
        public readonly Abf1.Abf1Header Abf1Header;

        /// <summary>
        /// True if ABF contains a valid ABF1-formatted header
        /// </summary>
        public bool IsAbf1 => Abf1Header != null;

        /// <summary>
        /// Access to low-level ABF2 header variables in memory with names that match the official documentation.
        /// Will be null if this is not an ABF2 file.
        /// </summary>
        public readonly Abf2.Abf2Header Abf2Header;

        /// <summary>
        /// True if ABF contains a valid ABF2-formatted header
        /// </summary>
        public bool IsAbf2 => Abf2Header != null;

        /// <summary>
        /// File format version stored in the data file during acquisition
        /// </summary>
        public readonly float FileVersionNumber;

        /// <summary>
        /// Data mode (episodic, gap-free, oscilloscope, etc.)
        /// </summary>
        public OperationMode OperationMode => (OperationMode)nOperationMode;

        /// <summary>
        /// Operation mode: 
        /// 1 = Event-driven, variable length
        /// 2 = Oscilloscope, loss free (Same as Event-driven, fixed length);
        /// 3 = Gap-free
        /// 4 = Oscilloscope, high-speed
        /// 5 = episodic stimulation (Clampex only)
        /// </summary>
        public readonly int nOperationMode;

        /// <summary>
        /// Total number of ADC samples in the ABF file (sample length * channel count)
        /// </summary>
        public readonly int lActualAcqLength;

        /// <summary>
        /// Name of the program that created this ABF file
        /// </summary>
        public readonly string Creator;

        /// <summary>
        /// Version of the program that created this ABF file
        /// </summary>
        public readonly string CreatorVersion;

        /// <summary>
        /// Name of the program that last modified this ABF file
        /// </summary>
        public readonly string Modifier;

        /// <summary>
        /// Version of the program that last modified this ABF file
        /// </summary>
        public readonly string ModifierVersion;

        /// <summary>
        /// A NON-unique ABF file identifier.
        /// Sequential recordings using the same protocol have the same GUID...
        /// </summary>
        public readonly Guid FileGUID;

        /// <summary>
        /// Number of ADC channels
        /// </summary>
        public int ChannelCount => nADCNumChannels;

        /// <summary>
        /// Number of channels recorded.
        /// </summary>
        public readonly short nADCNumChannels;

        /// <summary>
        /// Number of sweeps (gap-free ABFs have 1 sweep)
        /// </summary>
        public int SweepCount => Math.Max(1, lActualEpisodes);

        /// <summary>
        /// Number of episodic sweeps
        /// </summary>
        public readonly int lActualEpisodes;

        /// <summary>
        /// Day the ABF recording was started (Format YYMMDD)
        /// </summary>
        public readonly UInt32 uFileStartDate;

        /// <summary>
        /// Time (milliseconds after midnight) the ABF recording was started.
        /// </summary>
        public readonly UInt32 uFileStartTimeMS;

        /// <summary>
        /// When the ABF recording was started.
        /// </summary>
        public DateTime FileStart => AbfDateTime(uFileStartDate, uFileStartTimeMS);

        // TODO: document this
        public readonly Int16[] nADCPtoLChannelMap; // 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        // TODO: document this
        public readonly Int16[] nADCSamplingSeq; // 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0

        /// <summary>
        /// Holding level for the command signal. 
        /// This level is used when epochs are not changing the command waveform.
        /// </summary>
        public readonly float[] fDACHoldingLevel;

        /// <summary>
        /// Full path (on the original filesystem) to the protocol file.
        /// Null if a protocol file was not used.
        /// </summary>
        public readonly string sProtocolPath;

        /// <summary>
        /// A text comment placed in the waveform editor window.
        /// </summary>
        public readonly string sFileComment;

        /// <summary>
        /// Number of points per second (Hz)
        /// </summary>
        public double SampleRate => 1e6 / fADCSequenceInterval;

        /// <summary>
        /// ADC Channel Names
        /// </summary>
        [Obsolete("not yet implemented")]
        public string[] sADCChannelName;

        /// <summary>
        /// ADC Channel Units
        /// </summary>
        [Obsolete("not yet implemented")]
        public string[] sADCUnits;

        /// <summary>
        /// Location of the tag section in the ABF file.
        /// Multiply this value by 512 to get the byte position.
        /// </summary>
        public uint lTagSectionPtr;

        /// <summary>
        /// Number of tags in this ABF.
        /// </summary>
        public uint lNumTagEntries;

        /// <summary>
        /// Number of microseconds between samples (divided by number of channels).
        /// For a single-channel 20 kHz recording this will be 50.
        /// </summary>
        public readonly float fADCSequenceInterval;

        /// <summary>
        /// The ABF Synch array is an important array that stores the start time and length of each portion 
        /// of the data if the data are not part of a continuous gap-free acquisition. 
        /// The data section might contain equal length or variable length sweeps of data. 
        /// The Synch Array contains a record to indicate the start time and length of every sweep or Event in the data file. 
        /// The ABF reading routines automatically decode the Synch Array when providing information about the data.
        /// 
        /// A Synch array is created and used in the following acquisition modes: 
        /// ABF_VARLENEVENTS, ABF_FIXLENEVENTS & ABF_HIGHSPEEDOSC.
        /// 
        /// The acquisition modes ABF_GAPFREEFILE and ABF_WAVEFORMFILE do not always use a Synch array.
        /// </summary>
        public readonly float fSynchTimeUnit;

        /// <summary>
        /// Block number of start of the Synch Array section.
        /// </summary>
        public readonly int lSynchArrayPtr;

        /// <summary>
        /// Number of pairs of entries in the Synch Array section.
        /// If averaging is enabled, this includes the entry for the averaged sweep.
        /// </summary>
        public readonly int lSynchArraySize;

        /// <summary>
        /// Start time of each sweep (fSynchTimeUnit units)
        /// </summary>
        public int[] SynchStartTimes;

        /// <summary>
        /// Length of each sweep (time points * channel count)
        /// </summary>
        public int[] SynchLengths;

        /// <summary>
        /// Block number of start of Data section
        /// </summary>
        public readonly int lDataSectionPtr;

        /// <summary>
        /// Format of data points in memory (0 = 2-byte integer; 1 = IEEE 4 byte float)
        /// </summary>
        public readonly int nDataFormat;

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
        /// Number of bytes for each sample.
        /// Each sample has one data point for each channel.
        /// </summary>
        public int BytesPerSample => BytesPerValue * ChannelCount;

        public readonly float[] fInstrumentOffset;

        public readonly float[] fSignalOffset;

        public readonly float[] fInstrumentScaleFactor;

        public readonly float[] fSignalGain;

        public readonly float[] fADCProgrammableGain;

        public readonly int lADCResolution;

        public readonly float fADCRange;

        public readonly short[] nTelegraphEnable;
        public readonly short[] nTelegraphInstrument;
        public readonly float[] fTelegraphAdditGain;
        public readonly float[] fTelegraphFilter;
        public readonly float[] fTelegraphMembraneCap;
        public readonly short[] nTelegraphMode;
        public readonly short[] nTelegraphDACScaleFactorEnable;

        /// <summary>
        /// Populate the AbfSharp header by reading the binary content of the file
        /// </summary>
        public Header(BinaryReader reader)
        {
            byte[] signatureBytes = reader.ReadBytes(4);
            string signature = Encoding.ASCII.GetString(signatureBytes);

            if (signature == "ABF ")
                Abf1Header = new(reader);
            else if (signature == "ABF2")
                Abf2Header = new(reader);
            else
                throw new FileLoadException($"invalid ABF signature: {BitConverter.ToString(signatureBytes)}");

            // basic information about the ABF
            FileVersionNumber = IsAbf1 ? Abf1Header.fFileVersionNumber : Abf2Header.HeaderSection.fFileVersionNumber;
            nOperationMode = IsAbf1 ? Abf1Header.nOperationMode : Abf2Header.ProtocolSection.nOperationMode;
            lActualAcqLength = IsAbf1 ? Abf1Header.lActualAcqLength : (int)Abf2Header.DataSection.SectionCount;
            FileGUID = IsAbf1 ? MakeGuid(Abf1Header.FileGuid) : MakeGuid(Abf2Header.HeaderSection.FileGUID);
            nADCNumChannels = IsAbf1 ? Abf1Header.nADCNumChannels : (short)Abf2Header.AdcSection.SectionCount;
            lActualEpisodes = IsAbf1 ? Abf1Header.lActualEpisodes : (int)Abf2Header.HeaderSection.lActualEpisodes;
            nADCPtoLChannelMap = IsAbf1 ? Abf1Header.nADCPtoLChannelMap : Abf2Header.AdcSection.nADCPtoLChannelMap;
            nADCSamplingSeq = IsAbf1 ? Abf1Header.nADCSamplingSeq : Abf2Header.AdcSection.nADCSamplingSeq;
            Creator = IsAbf1 ? Abf1Header.sCreatorInfo : Abf2Header.StringsSection.Strings[Abf2Header.HeaderSection.uCreatorNameIndex];
            CreatorVersion = IsAbf1 ? Abf1Header.CreatorVersion : Abf2Header.HeaderSection.CreatorVersion;
            Modifier = IsAbf1 ? Abf1Header.sModifierInfo : Abf2Header.StringsSection.Strings[Abf2Header.HeaderSection.uModifierNameIndex];
            ModifierVersion = IsAbf1 ? Abf1Header.ModifierVersion : Abf2Header.HeaderSection.ModifierVersion;
            uFileStartDate = IsAbf1 ? Abf1Header.uFileStartDate : Abf2Header.HeaderSection.uFileStartDate;
            uFileStartTimeMS = IsAbf1 ? ((uint)Abf1Header.lFileStartTime) * 1000 + (uint)Abf1Header.nFileStartMillisecs : Abf2Header.HeaderSection.uFileStartTimeMS;
            fDACHoldingLevel = IsAbf1 ? Abf1Header.fDACHoldingLevel : Abf2Header.DacSection.fDACHoldingLevel;
            sProtocolPath = IsAbf1 ? Abf1Header.sProtocolPath : Abf2Header.StringsSection.Strings[Abf2Header.HeaderSection.uProtocolPathIndex];
            sFileComment = IsAbf1 ? Abf1Header.sFileComment : Abf2Header.StringsSection.Strings[Abf2Header.ProtocolSection.lFileCommentIndex];
            lTagSectionPtr = IsAbf1 ? (uint)Abf1Header.lTagSectionPtr : Abf2Header.TagSection.SectionBlock;
            lNumTagEntries = IsAbf1 ? (uint)Abf1Header.lNumTagEntries : Abf2Header.TagSection.SectionCount;
            fADCSequenceInterval = IsAbf1 ? Abf1Header.fADCSampleInterval * Abf1Header.nADCNumChannels : Abf2Header.ProtocolSection.fADCSequenceInterval;
            fSynchTimeUnit = IsAbf1 ? Abf1Header.fSynchTimeUnit : Abf2Header.ProtocolSection.fSynchTimeUnit;
            lSynchArrayPtr = IsAbf1 ? Abf1Header.lSynchArrayPtr : (int)Abf2Header.SynchSection.SectionBlock;
            lSynchArraySize = IsAbf1 ? Abf1Header.lSynchArraySize : (int)Abf2Header.SynchSection.SectionCount;
            lDataSectionPtr = IsAbf1 ? Abf1Header.lDataSectionPtr : (int)Abf2Header.DataSection.SectionBlock;
            nDataFormat = IsAbf1 ? Abf1Header.nDataFormat : Abf2Header.HeaderSection.nDataFormat;
            fInstrumentOffset = IsAbf1 ? Abf1Header.fInstrumentOffset : Abf2Header.AdcSection.fInstrumentOffset;
            fSignalOffset = IsAbf1 ? Abf1Header.fSignalOffset : Abf2Header.AdcSection.fSignalOffset;
            fInstrumentScaleFactor = IsAbf1 ? Abf1Header.fInstrumentScaleFactor : Abf2Header.AdcSection.fInstrumentScaleFactor;
            fSignalGain = IsAbf1 ? Abf1Header.fSignalGain : Abf2Header.AdcSection.fSignalGain;
            fADCProgrammableGain = IsAbf1 ? Abf1Header.fADCProgrammableGain : Abf2Header.AdcSection.fADCProgrammableGain;
            lADCResolution = IsAbf1 ? Abf1Header.lADCResolution : (int)Abf2Header.ProtocolSection.lADCResolution;
            fInstrumentOffset = IsAbf1 ? Abf1Header.fInstrumentOffset : Abf2Header.AdcSection.fInstrumentOffset;
            fADCRange = IsAbf1 ? Abf1Header.fADCRange : Abf2Header.ProtocolSection.fADCRange;
            SynchStartTimes = IsAbf1 ? Abf1Header.SynchSection_lStart : Abf2Header.SynchSection.lStart;
            SynchLengths = IsAbf1 ? Abf1Header.SynchSection_lLength : Abf2Header.SynchSection.lLength;

            nTelegraphEnable = IsAbf1 ? Abf1Header.nTelegraphEnable : Abf2Header.AdcSection.nTelegraphEnable;
            nTelegraphInstrument = IsAbf1 ? Abf1Header.nTelegraphInstrument : Abf2Header.AdcSection.nTelegraphInstrument;
            fTelegraphAdditGain = IsAbf1 ? Abf1Header.fTelegraphAdditGain : Abf2Header.AdcSection.fTelegraphAdditGain;
            fTelegraphFilter = IsAbf1 ? Abf1Header.fTelegraphFilter : Abf2Header.AdcSection.fTelegraphFilter;
            fTelegraphMembraneCap = IsAbf1 ? Abf1Header.fTelegraphMembraneCap : Abf2Header.AdcSection.fTelegraphMembraneCap;
            nTelegraphMode = IsAbf1 ? Abf1Header.nTelegraphMode : Abf2Header.AdcSection.nTelegraphMode;
            nTelegraphDACScaleFactorEnable = IsAbf1 ? Abf1Header.nTelegraphDACScaleFactorEnable : Abf2Header.DacSection.nTelegraphDACScaleFactorEnable;
        }

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
                return new DateTime();
            }
        }

        /// <summary>
        /// Use the ABF convention to rearrange bytes and return them as a .NET GUID
        /// </summary>
        private static Guid MakeGuid(byte[] bytes)
        {
            if (bytes is null || bytes.Length != 16)
                throw new ArgumentException("bytes must be length 16");

            byte[] bytes2 = new byte[16];
            int[] indexes = { 3, 2, 1, 0, 5, 4, 7, 6, 8, 9, 10, 11, 12, 13, 14, 15 };
            for (int i = 0; i < 16; i++)
                bytes2[i] = bytes[indexes[i]];

            return new Guid(bytes);
        }
    }
}
