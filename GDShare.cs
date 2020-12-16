using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace gdtools_cpp {
    public static class GDShare {
        public delegate void CCDataReturn(string s);
        public delegate void ProgressReturn(ushort p, string s);
        public delegate void LevelReturn(string[] _lvls);

        public static string DecodedCCData = null;
        public static bool DecodedCCLocalLevels = false;

        [DllImport("cpp/GDShare.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DecodeCCFile(string _text, CCDataReturn _f, ProgressReturn _prog);

        [DllImport("cpp/GDShare.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetLevels(string _CCData, LevelReturn _lvls);
    }
}