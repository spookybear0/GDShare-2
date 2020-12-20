using System;
using System.Windows;
using System.Windows.Controls;
using static System.Collections.IEnumerable;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace gdtools_cpp {
    namespace Pages {
        public class Replay : Page {

            private void installRequirements() {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C py -m pip install -r D:\\code\\github\\GDShare-2\\python\\requirements.txt";
                startInfo.RedirectStandardOutput = true;
                process.StartInfo = startInfo;
                process.Start();
                Console.WriteLine(process.StandardOutput.ReadToEnd());
            }

            private void callPython(string function) {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C py -c \"import os; os.chdir('D:\\code\\github\\GDShare-2\\replay'); import replay; replay.export_replay()\""; // we need to change to correct dir
                process.StartInfo = startInfo;
                process.Start();
            }
            public Replay(GDTWindow _w) : base(_w) {
                this.Children.Add(new Elem.Centered(new UIElement[] {
                    new Elem.Header(new UIElement[] {
                        new Elem.Pad(),
                        new Elem.Text("Replay")
                    }),
                    new Elem.Newline(),
                    new Elem.But("Export", default(Size), (s, e) => {
                        callPython("export_replay()");
                    }),
                    new Elem.But("Import", default(Size), (s, e) => {
                        callPython("import_replay()");
                    }),
                    new Elem.But("Install req", default(Size), (s, e) => {
                        installRequirements();
                    })
                }));
            }
        }
    }
}