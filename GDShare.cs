using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace gdtools_cpp {
    public static class GDShare {
        [DllImport("cpp/GDShare.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern string DecodeCCFile(string _name);
    }
}