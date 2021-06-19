using System;
using System.Collections.Generic;
using System.Text;

namespace AbfSharp.HeaderData
{
    /// <summary>
    /// Holds information about a SINGLE CHANNEL necessary to convert signal bytes to ADC data.
    /// </summary>
    public class AdcDataInfo
    {
        public readonly Int16 nDataFormat;
        public readonly float fInstrumentOffset;
        public readonly float fSignalOffset;
        public readonly float fInstrumentScaleFactor;
        public readonly float fSignalGain;
        public readonly float fADCProgrammableGain;
        public readonly Int32 lADCResolution;
        public readonly float fADCRange;

        public AdcDataInfo(Int16 nDataFormat, float fInstrumentOffset, float fSignalOffset, float fInstrumentScaleFactor, float fSignalGain, float fADCProgrammableGain, Int32 lADCResolution, float fADCRange)
        {
            this.nDataFormat = nDataFormat;
            this.fInstrumentOffset = fInstrumentOffset;
            this.fSignalOffset = fSignalOffset;
            this.fInstrumentScaleFactor = fInstrumentScaleFactor;
            this.fSignalGain = fSignalGain;
            this.fADCProgrammableGain = fADCProgrammableGain;
            this.lADCResolution = lADCResolution;
            this.fADCRange = fADCRange;
        }

        public enum DataFormat
        {
            Int16 = 0,
            Float = 1,
            Double = 2,
        }

        public DataFormat Format => (DataFormat)nDataFormat;

        public float[] GetPoints()
        {
            /*
                When converting binary floating point data, a scaling algorithm is applied:
                  (DataPoint - fInstrumentOffset + fSignalOffset) * 
                  fInstrumentScaleFactor * fSignalGain * fADCProgrammableGain) * 
                  (lADCResolution / fADCRange)
            */
            return null;
        }
    }
}
