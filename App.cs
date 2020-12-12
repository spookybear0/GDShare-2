using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices;

namespace gdtools_cpp {
    public static class Settings {
        public static string UserdataName = "user";
        public static string AppName = "GDShare_2";

        public static class Ext {
            public static string Data = "gdt";
        }
    }

    public partial class App : Application {
        [DllImport( "kernel32.dll" )]
        static extern bool AttachConsole( int dwProcessId );

        [STAThread]
        public static void Main(String[] args) {
            AttachConsole(-1);
            App app = new App();

            Userdata.LoadData();

            Theme.Init();

            app.Run(new GDTWindow(new MainWindow()));
        }
    }
}