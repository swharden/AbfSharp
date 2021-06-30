using System;
using System.Collections.Generic;
using System.Text;

namespace AbfSharp.ABFFIO
{
    public static class AbfError
    {
        public static void AssertSuccess(Int32 errorCode, bool exceptOnError = true)
        {
            if (exceptOnError && errorCode != 0)
                throw new Exception($"ABFFIO error code: {errorCode} ({GetDescription(errorCode)})");
        }

        public static string GetDescription(Int32 errorCode)
        {
            string description;

            if (errorCode == 0) description = "ABF_SUCCESS";
            else if (errorCode == 1001) description = "ABF_EUNKNOWNFILETYPE";
            else if (errorCode == 1002) description = "ABF_EBADFILEINDEX";
            else if (errorCode == 1003) description = "ABF_TOOMANYFILESOPEN";
            else if (errorCode == 1004) description = "ABF_EOPENFILE - could not open file";
            else if (errorCode == 1005) description = "ABF_EBADPARAMETERS";
            else if (errorCode == 1006) description = "ABF_EREADDATA";
            else if (errorCode == 1008) description = "ABF_OUTOFMEMORY";
            else if (errorCode == 1009) description = "ABF_EREADSYNCH";
            else if (errorCode == 1010) description = "ABF_EBADSYNCH";
            else if (errorCode == 1011) description = "ABF_EEPISODERANGE - invalid sweep number";
            else if (errorCode == 1012) description = "ABF_EINVALIDCHANNEL";
            else if (errorCode == 1013) description = "ABF_EEPISODESIZE";
            else if (errorCode == 1014) description = "ABF_EREADONLYFILE";
            else if (errorCode == 1015) description = "ABF_EDISKFULL";
            else if (errorCode == 1016) description = "ABF_ENOTAGS";
            else if (errorCode == 1017) description = "ABF_EREADTAG";
            else if (errorCode == 1018) description = "ABF_ENOSYNCHPRESENT";
            else if (errorCode == 1019) description = "ABF_EREADDACEPISODE";
            else if (errorCode == 1020) description = "ABF_ENOWAVEFORM";
            else if (errorCode == 1021) description = "ABF_EBADWAVEFORM";
            else if (errorCode == 1022) description = "ABF_BADMATHCHANNEL";
            else if (errorCode == 1023) description = "ABF_BADTEMPFILE";
            else if (errorCode == 1025) description = "ABF_NODOSFILEHANDLES";
            else if (errorCode == 1026) description = "ABF_ENOSCOPESPRESENT";
            else if (errorCode == 1027) description = "ABF_EREADSCOPECONFIG";
            else if (errorCode == 1028) description = "ABF_EBADCRC";
            else if (errorCode == 1029) description = "ABF_ENOCOMPRESSION";
            else if (errorCode == 1030) description = "ABF_EREADDELTA";
            else if (errorCode == 1031) description = "ABF_ENODELTAS";
            else if (errorCode == 1032) description = "ABF_EBADDELTAID";
            else if (errorCode == 1033) description = "ABF_EWRITEONLYFILE";
            else if (errorCode == 1034) description = "ABF_ENOSTATISTICSCONFIG";
            else if (errorCode == 1035) description = "ABF_EREADSTATISTICSCONFIG";
            else if (errorCode == 1036) description = "ABF_EWRITERAWDATAFILE";
            else if (errorCode == 1037) description = "ABF_EWRITEMATHCHANNEL";
            else if (errorCode == 1038) description = "ABF_EWRITEANNOTATION";
            else if (errorCode == 1039) description = "ABF_EREADANNOTATION";
            else if (errorCode == 1040) description = "ABF_ENOANNOTATIONS";
            else if (errorCode == 1041) description = "ABF_ECRCVALIDATIONFAILED";
            else if (errorCode == 1042) description = "ABF_EWRITESTRING";
            else if (errorCode == 1043) description = "ABF_ENOSTRINGS";
            else if (errorCode == 1044) description = "ABF_EFILECORRUPT";
            else description = "UNKNOWN";

            return description;
        }
    }
}
