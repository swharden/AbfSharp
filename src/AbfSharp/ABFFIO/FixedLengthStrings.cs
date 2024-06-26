using System.Runtime.InteropServices;
using System.Text;

namespace AbfSharp.ABFFIO;

public static class FixedLengthStrings
{
    /// <summary>
    /// A fixed-length string with 8 characters
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct CharArray8
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public byte[] Chars;
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => Encoding.UTF8.GetString(Chars.Where(x => x > 0).ToArray()).Trim();

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (obj.GetType() != typeof(string))
                throw new NotImplementedException("invalid comparison type");

            string objString = (string)obj;

            if (string.IsNullOrWhiteSpace(objString) && Chars.Max() == 0)
                return true;

            return objString == ToString();
        }
    }

    /// <summary>
    /// A fixed-length string with 10 characters
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct CharArray10
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)] public byte[] Chars;
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => Encoding.UTF8.GetString(Chars.Where(x => x > 0).ToArray()).Trim();
        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (obj.GetType() != typeof(string))
                throw new NotImplementedException("invalid comparison type");

            string objString = (string)obj;

            if (string.IsNullOrWhiteSpace(objString) && Chars.Max() == 0)
                return true;

            return objString == ToString();
        }
    }

    /// <summary>
    /// The ABF header struct in memory identically to the latest ABFFIO DLL provided with Clampex 11
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct StringArray16x10
    {
        const int STRING_LENGTH = 10;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string4;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string5;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string6;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string7;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string8;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string9;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string10;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string11;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string12;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string13;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string14;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string15;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string16;

        public string[] GetArray()
        {
            string[] strings = new string[]
            {
                string1, string2, string3, string4, string5, string6, string7, string8,
                string9, string10, string11, string12, string13, string14, string15, string16,
            };

            for (int i = 0; i < strings.Length; i++)
                strings[i] = strings[i].Trim();

            return strings;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct StringArray16x8
    {
        const int STRING_LENGTH = 8;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string4;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string5;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string6;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string7;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string8;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string9;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string10;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string11;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string12;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string13;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string14;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string15;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRING_LENGTH)] public string string16;

        public string[] GetArray()
        {
            string[] strings = new string[]
            {
                string1, string2, string3, string4, string5, string6, string7, string8,
                string9, string10, string11, string12, string13, string14, string15, string16,
            };

            for (int i = 0; i < strings.Length; i++)
                strings[i] = strings[i].Trim();

            return strings;
        }
    }
}
