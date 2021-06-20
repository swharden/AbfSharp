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
        public Abf1.Abf1Header Abf1Header { get; private set; }

        /// <summary>
        /// True if ABF contains a valid ABF1-formatted header
        /// </summary>
        public bool IsAbf1 => Abf1Header != null;

        /// <summary>
        /// Access to low-level ABF2 header variables in memory with names that match the official documentation.
        /// Will be null if this is not an ABF2 file.
        /// </summary>
        public Abf2.Abf2Header Abf2Header { get; private set; }

        /// <summary>
        /// True if ABF contains a valid ABF2-formatted header
        /// </summary>
        public bool IsAbf2 => Abf2Header != null;

        /// <summary>
        /// File format version stored in the data file during acquisition
        /// </summary>
        public float FileVersionNumber { get; private set; }

        /// <summary>
        /// Data mode (episodic, gap-free, oscilloscope, etc.)
        /// </summary>
        public OperationMode OperationMode { get; private set; }

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
        public Guid GUID { get; private set; }

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
        public readonly int SweepCount;

        /// <summary>
        /// Stores data scaling information for each ADC channel.
        /// </summary>
        public readonly AdcDataInfo[] AdcDataInfo;

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
        public DateTime FileStart;

        // TODO: document this
        public readonly Int16[] nADCPtoLChannelMap;

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
        public int SampleRate;

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
        /// Block number of start of Data section
        /// </summary>
        public readonly int lDataSectionPtr;

        /// <summary>
        /// Format of data points in memory (0 = 2-byte integer; 1 = IEEE 4 byte float)
        /// </summary>
        public readonly int nDataFormat;

        public int BytesPerDataPoint => nDataFormat == 0 ? 2 : 4;


        /// <summary>
        /// Populate the AbfSharp header using an ABFFIO struct
        /// </summary>
        public Header(ABFFIO.Structs.ABFFileHeader header)
        {
            FileVersionNumber = header.fFileVersionNumber;
            OperationMode = (OperationMode)header.nOperationMode;
        }

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
            OperationMode = IsAbf1 ? (OperationMode)Abf1Header.nOperationMode : (OperationMode)Abf2Header.ProtocolSection.nOperationMode;
            GUID = IsAbf1 ? MakeGuid(Abf1Header.FileGuid) : MakeGuid(Abf2Header.HeaderSection.FileGUID);
            nADCNumChannels = IsAbf1 ? Abf1Header.nADCNumChannels : (short)Abf2Header.AdcSection.SectionCount;
            SweepCount = IsAbf1 ? Abf1Header.lActualEpisodes : (int)Abf2Header.HeaderSection.lActualEpisodes;
            nADCPtoLChannelMap = IsAbf1 ? Abf1Header.nADCPtoLChannelMap : Abf2Header.AdcSection.nADCPtoLChannelMap;
            Creator = IsAbf1 ? Abf1Header.sCreatorInfo : Abf2Header.StringsSection.Strings[Abf2Header.HeaderSection.uCreatorNameIndex];
            CreatorVersion = IsAbf1 ? Abf1Header.CreatorVersion : Abf2Header.HeaderSection.CreatorVersion;
            Modifier = IsAbf1 ? Abf1Header.sModifierInfo : Abf2Header.StringsSection.Strings[Abf2Header.HeaderSection.uModifierNameIndex];
            ModifierVersion = IsAbf1 ? Abf1Header.ModifierVersion : Abf2Header.HeaderSection.ModifierVersion;
            uFileStartDate = IsAbf1 ? Abf1Header.uFileStartDate : Abf2Header.HeaderSection.uFileStartDate;
            uFileStartTimeMS = IsAbf1 ? ((uint)Abf1Header.lFileStartTime) * 1000 + (uint)Abf1Header.nFileStartMillisecs : Abf2Header.HeaderSection.uFileStartTimeMS;
            FileStart = AbfDateTime(uFileStartDate, uFileStartTimeMS);
            fDACHoldingLevel = IsAbf1 ? Abf1Header.fDACHoldingLevel : Abf2Header.DacSection.fDACHoldingLevel;
            sProtocolPath = IsAbf1 ? Abf1Header.sProtocolPath : Abf2Header.StringsSection.Strings[Abf2Header.HeaderSection.uProtocolPathIndex];
            sFileComment = IsAbf1 ? Abf1Header.sFileComment : Abf2Header.StringsSection.Strings[Abf2Header.ProtocolSection.lFileCommentIndex];
            SampleRate = IsAbf1 ? (int)(1e6 / Abf1Header.fADCSampleInterval / Abf1Header.nADCNumChannels) : (int)(1e6 / Abf2Header.ProtocolSection.fADCSequenceInterval);
            lTagSectionPtr = IsAbf1 ? (uint)Abf1Header.lTagSectionPtr : Abf2Header.TagSection.SectionBlock;
            lNumTagEntries = IsAbf1 ? (uint)Abf1Header.lNumTagEntries : Abf2Header.TagSection.SectionCount;
            fADCSequenceInterval = IsAbf1 ? Abf1Header.fADCSampleInterval * Abf1Header.nADCNumChannels : Abf2Header.ProtocolSection.fADCSequenceInterval;
            fSynchTimeUnit = IsAbf1 ? Abf1Header.fSynchTimeUnit : Abf2Header.ProtocolSection.fSynchTimeUnit;
            lSynchArrayPtr = IsAbf1 ? Abf1Header.lSynchArrayPtr : (int)Abf2Header.SynchSection.SectionBlock;
            lSynchArraySize = IsAbf1 ? Abf1Header.lSynchArraySize : (int)Abf2Header.SynchSection.SectionCount;
            lDataSectionPtr = IsAbf1 ? Abf1Header.lDataSectionPtr : (int)Abf2Header.DataSection.SectionBlock;
            nDataFormat = IsAbf1 ? Abf1Header.nDataFormat : Abf2Header.HeaderSection.nDataFormat;

            // TODO: refactor this
            // scaling information required to convert ADC bytes to final values
            AdcDataInfo = new AdcDataInfo[ChannelCount];
            for (int i = 0; i < ChannelCount; i++)
            {
                AdcDataInfo[i] = new(
                    nDataFormat: IsAbf1 ? (uint)Abf1Header.nDataFormat : Abf2Header.HeaderSection.nDataFormat,
                    fInstrumentOffset: IsAbf1 ? Abf1Header.fInstrumentOffset[i] : Abf2Header.AdcSection.fInstrumentOffset[i],
                    fSignalOffset: IsAbf1 ? Abf1Header.fSignalOffset[i] : Abf2Header.AdcSection.fSignalOffset[i],
                    fInstrumentScaleFactor: IsAbf1 ? Abf1Header.fInstrumentScaleFactor[i] : Abf2Header.AdcSection.fInstrumentScaleFactor[i],
                    fSignalGain: IsAbf1 ? Abf1Header.fSignalGain[i] : Abf2Header.AdcSection.fSignalGain[i],
                    fADCProgrammableGain: IsAbf1 ? Abf1Header.fADCProgrammableGain[i] : Abf2Header.AdcSection.fADCProgrammableGain[i],
                    lADCResolution: IsAbf1 ? (uint)Abf1Header.lADCResolution : Abf2Header.ProtocolSection.lADCResolution,
                    fADCRange: IsAbf1 ? Abf1Header.fADCRange : Abf2Header.ProtocolSection.fADCRange
                    );
            }
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
