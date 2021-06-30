using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace AbfSharp.ABFFIO
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct TagStruct
    {
        public Int32 lTagTime;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.ABF_TAGCOMMENTLEN)] public string sComment;
        public Int16 nTagType;
        public Int16 nVoiceTagNumber;
    };
}
