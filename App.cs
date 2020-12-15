using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Runtime.InteropServices;

namespace gdtools_cpp {
    public static class Settings {
        public static string UserdataName = "user";
        public static string AppName = "GDShare_2";
        public static HorizontalAlignment Alignment = HorizontalAlignment.Center;

        public static class Ext {
            public static string Data = "gdt";
            public static string LevelStandard = "gmd2";
            public static string LevelCompressed = "gmdc";
            public static string LvlShare = "lvl";
            public static string GDShare = "gmd";
            public static string[] LevelList = new string[] {
                LevelStandard, LevelCompressed, LvlShare, GDShare
            };
        }
    }

    public partial class App : Application {
        [DllImport( "kernel32.dll" )]
        static extern bool AttachConsole( int dwProcessId );

        public static Userdata Userdata = new Userdata();

        public static void Reload() {
            ProcessStartInfo p = new ProcessStartInfo();
            p.WorkingDirectory = System.IO.Directory.GetCurrentDirectory();
            p.FileName = System.AppDomain.CurrentDomain.FriendlyName;

            System.Diagnostics.Process.Start(p);
            Application.Current.Shutdown();
        }

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