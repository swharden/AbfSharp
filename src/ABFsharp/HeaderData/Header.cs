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
        /// A NON-unique ABF file identifier.
        /// Sequential recordings using the same protocol have the same GUID...
        /// </summary>
        public Guid GUID { get; private set; }

        /// <summary>
        /// Number of ADC channels
        /// </summary>
        public readonly uint ChannelCount;

        /// <summary>
        /// Stores data scaling information for each ADC channel.
        /// </summary>
        public readonly AdcDataInfo[] AdcDataInfo;

        public readonly Int16[] nADCPtoLChannelMap;

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
            GUID = IsAbf1 ? MakeGuid(Abf1Header.uFileGUID) : MakeGuid(Abf2Header.HeaderSection.FileGUID);
            ChannelCount = IsAbf1 ? (uint)Abf1Header.nADCNumChannels : Abf2Header.AdcSection.Count;
            nADCPtoLChannelMap = IsAbf1 ? Abf1Header.nADCPtoLChannelMap : Abf2Header.AdcSection.nADCPtoLChannelMap;

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
