using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Windows;
using System.Runtime.InteropServices;

public class Replay {
    public static void startRecorder() {
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = "/C py -c \"import os; os.chdir('D:\\code\\github\\GDShare-2\\python'); import replay; replay.start_replay_recorder()\""; // we need to change to correct dir
        startInfo.RedirectStandardOutput = true;
        process.StartInfo = startInfo;
        process.Start();
        Console.WriteLine(process.StandardOutput.ReadToEnd());
    }
}

namespace gdtools_cpp {
    public static class Settings {
        public static string UserdataName = "user";
        public static string AppName = "GDShare_2";
        public static HorizontalAlignment Alignment = HorizontalAlignment.Center;
        public static bool ShowShortcuts = false;
        public static string ThemeName = $"DefaultStyle.{Ext.Data}";

        public static class Ext {
            public static string Data = "gdt";
            public static string LevelStandard = "gmd2";
            public static string LevelCompressed = "gmdc";
            public static string LvlShare = "lvl";
            public static string GDShare = "gmd";
            public static string[] LevelList = new string[] {
                LevelStandard, LevelCompressed, LvlShare, GDShare
            };
            public static string LevelFilter =
                $"Level Files|*.{String.Join(";*.", LevelList)}" +
                "|GDShare 2 Files|*.gmd2;*.gmdc;*.gmd" +
                "|GDShare Files|*.gmd" +
                "|LvlShare Files|*.lvl" +
                "|All Files|*.*";
            public static string ExportFilter =
                "Level|*.gmd2" +
                "|Compressed Level|*.gmdc" +
                "|GDShare|*.gmd" +
                "|LvlShare|*.lvl";
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
            Console.WriteLine("d");
            Thread replayThread = new Thread(Replay.startRecorder);
            replayThread.Start();
            Userdata.LoadData();

            Theme.Init();

            app.Run(new GDTWindow(new MainWindow()));
        }
    }
}