using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace gdtools_cpp {
    public static class GDShare {
        public delegate void CCDataReturn(string s);
        public delegate void ProgressReturn(ushort p);

        [DllImport("cpp/GDShare.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DecodeCCFile(string _text, CCDataReturn _f, ProgressReturn _prog);
    }
}