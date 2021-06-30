using System;
using System.Collections.Generic;
using System.Text;

namespace AbfSharp.ABFFIO
{
    public static class Constants
    {
        /// <summary>
        /// number of ADC channels supported
        /// </summary>
        public const int ABF_ADCCOUNT = 16;

        /// <summary>
        /// length of actual ADC channel name strings
        /// </summary>
        public const int ABF_ADCNAMELEN = 10;

        /// <summary>
        /// length of user-entered ADC channel name strings
        /// </summary>
        public const int ABF_ADCNAMELEN_USER = 8;

        /// <summary>
        /// length of ADC units strings
        /// </summary>
        public const int ABF_ADCUNITLEN = 8;

        /// <summary>
        /// length of the Arithmetic operator field
        /// </summary>
        public const int ABF_ARITHMETICOPLEN = 2;

        /// <summary>
        /// length of arithmetic units string
        /// </summary>
        public const int ABF_ARITHMETICUNITSLEN = 8;

        /// <summary>
        /// Size of block alignment in ABF files
        /// </summary>
        public const int ABF_BLOCKSIZE = 512; 

        /// <summary>
        /// length of file creator info string
        /// </summary>
        public const int ABF_CREATORINFOLEN = 16;

        /// <summary>
        /// number of DAC channels supported
        /// </summary>
        public const int ABF_DACCOUNT = 8; 

        /// <summary>
        /// length of DAC channel name strings
        /// </summary>
        public const int ABF_DACNAMELEN = 10;

        /// <summary>
        /// length of DAC units strings
        /// </summary>
        public const int ABF_DACUNITLEN = 8;

        /// <summary>
        /// number of waveform epochs supported
        /// </summary>
        public const int ABF_EPOCHCOUNT = 50; 

        /// <summary>
        /// length of file comment string (V1.6)
        /// </summary>
        public const int ABF_FILECOMMENTLEN = 128;

        /// <summary>
        /// The maximum number of sweeps that can be combined into a cumulative average
        /// </summary>
        public const int ABF_MAX_SWEEPS_PER_AVERAGE = 65500;

        /// <summary>
        /// Maximum length of acquisition supported (samples)
        /// </summary>
        public const int ABF_MAX_TRIAL_SAMPLES = 0x7FFFFFFF;

        /// <summary>
        /// length of file comment string (pre V1.6)
        /// </summary>
        public const int ABF_OLDFILECOMMENTLEN = 56;

        /// <summary>
        /// length of full path, used for DACFile and Protocol name
        /// </summary>
        public const int ABF_PATHLEN = 256;

        /// <summary>
        /// number of independent statistics regions
        /// </summary>
        public const int ABF_STATS_REGIONS = 24; 

        /// <summary>
        /// length of tag comment string
        /// </summary>
        public const int ABF_TAGCOMMENTLEN = 56;

        /// <summary>
        /// number of independent user lists (V1.6)       
        /// </summary>
        public const int ABF_USERLISTCOUNT = ABF_DACCOUNT;

        /// <summary>
        /// length of the user list (V1.6)
        /// </summary>
        public const int ABF_USERLISTLEN = 256;

        /// <summary>
        /// helps calculate the pre-epoch duration
        /// </summary>
        public const int ABFH_HOLDINGFRACTION = 64;

        /// <summary>
        /// Maximum per channel sweep length supported by pCLAMP11 apps
        /// </summary>
        public const int PCLAMP11_MAXSWEEPLEN_PERCHAN = 5161290; 

        /// <summary>
        /// Maximum multiplexed sweep length supported by pCLAMP6 apps
        /// </summary>
        public const int PCLAMP6_MAXSWEEPLENGTH = 16384;

        /// <summary>
        /// Maximum per channel sweep length supported by pCLAMP7 apps
        /// </summary>
        public const int PCLAMP7_MAXSWEEPLEN_PERCHAN = 1032258;
    }
}
