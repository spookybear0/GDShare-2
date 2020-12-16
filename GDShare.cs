using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace gdtools_cpp {
    public static class GDShare {
        public delegate void CCDataReturn(string s);
        public delegate void ProgressReturn(ushort p, string s);
        public delegate void LevelReturn(string lvls);
        public delegate void BoolReturn(bool succ);

        public static string DecodedCCData = null;
        public static bool DecodedCCLocalLevels = false;
        public static string[] DecodedLevels = null;

        [DllImport("cpp/GDShare.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DecodeCCFile(string _text, CCDataReturn _f, ProgressReturn _prog);

        [DllImport("cpp/GDShare.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetLevels(string _CCData, LevelReturn _lvls);

        [DllImport("cpp/GDShare.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ExportLevel(string _name, string _path, BoolReturn _succ);
    }
}