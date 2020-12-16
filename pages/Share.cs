using System;
using System.Windows;
using System.Threading;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;

namespace gdtools_cpp {
    namespace Pages {
        public class Sharing : Page {
            private Elem.Centered LoadedPage;
            private Elem.Centered UnloadedPage;

            public Sharing(GDTWindow _w) : base(_w) {
                UnloadedPage = new Elem.Centered(new UIElement[] {
                    new Elem.Header(new UIElement[] {
                        Theme.LoadIcon("Cog"),
                        new Elem.Pad(),
                        new Elem.Text("Decode")
                    }),
                    new Elem.Pad(),
                    new Elem.Text("You need to decode your CCLocalLevels.dat file to use Sharing!", 0, Theme.Colors.TextDark),
                    new Elem.Text("Decoding may take a while.", 0, Theme.Colors.TextDark),
                    new Elem.Pad(),
                    new Elem.But("Decode", default(Size), (s, e) => {
                        Elem.ProgressBar pb = _w.Contents.ProgressBars.AddBar();
                        
                        Thread t = new Thread(() => {
                            this.Dispatcher.Invoke(() => {
                                pb.SetProgress(0, "Decoding LocalLevels...");
                                ((Elem.But)s).Disable("Decoding...");
                            });

                            GDShare.DecodeCCFile("LocalLevels", st => {
                                Console.WriteLine( st.Substring(0, 100) );
                                this.Dispatcher.Invoke(() => {
                                    GDShare.DecodedCCData = st;
                                    GDShare.DecodedCCLocalLevels = true;
                                    pb.Finish();
                                    LoadedPage.Visibility = Visibility.Visible;
                                    UnloadedPage.Visibility = Visibility.Collapsed;
                                });
                            }, (p, st) => this.Dispatcher.Invoke(() => pb.SetProgress(p, st)));
                        });
                        
                        t.SetApartmentState(ApartmentState.STA);
                        t.Start();
                    })
                });

                GDShare.GetLevels(GDShare.DecodedCCData, lvls => {
                    GDShare.DecodedLevels = lvls.Split(";");
                });

                Elem.Select LevelSelect = new Elem.Select(
                    "Select Level", GDShare.DecodedLevels, true, 10
                );

                Elem.Organized ExportPage = new Elem.Organized(new UIElement[] {
                    LevelSelect,
                    new Elem.Pad(),
                    new Elem.Search(LevelSelect),
                    new Elem.Pad(),
                    new Elem.But("Export", default(Size), (s, e) => {
                        Elem.ProgressBar pb = _w.Contents.ProgressBars.AddBar();
                        SaveFileDialog opf = new SaveFileDialog();

                        List<string> exports = LevelSelect.GetSelection();

                        if (exports.Count > 1) {
                            opf.Filter = Settings.Ext.ExportFilter;
                            opf.Title = "Export Levels";
                            opf.FileName = "Select Folder to Export to!";
                            opf.RestoreDirectory = true;
                            
                            if ((bool)opf.ShowDialog()) {
                                string dpath = Path.GetDirectoryName(opf.FileName);

                                pb.SetProgress(0, "Exporting...");
                                uint ix = 0;
                                List<string> failed = new List<string> ();
                                foreach (string exp in exports)
                                    GDShare.ExportLevel(exp, $"{dpath}\\{exp}{opf.FileName.Substring(opf.FileName.LastIndexOf("."))}", s => {
                                        if (s) pb.SetProgress((ushort)((++ix / exports.Count) * 100), $"Exported {exp} to {opf.FileName}");
                                        else failed.Add(exp);
                                    });
                                if (failed.Count == 0)
                                    pb.Finish("Exported all!");
                                else pb.Fail($"Error exporting {String.Join(",", failed)}");
                            }
                        } else {
                            opf.Filter = Settings.Ext.ExportFilter;
                            opf.Title = "Export Level";
                            opf.RestoreDirectory = true;
                            opf.DefaultExt = Settings.Ext.LevelStandard;
                            opf.FileName = exports[0];
                            
                            if ((bool)opf.ShowDialog())
                                GDShare.ExportLevel(exports[0], opf.FileName, s => {
                                    if (s) pb.Finish($"Exported to {opf.FileName}!");
                                    else pb.Fail("Unknown error exporting");
                                });
                        }
                    })
                });

                Elem.Organized ImportPage = new Elem.Organized(new UIElement[] {
                    new Elem.Text("swag")
                });

                ExportPage.Visibility = Visibility.Visible;
                ImportPage.Visibility = Visibility.Collapsed;

                LoadedPage = new Elem.Centered(new UIElement[] {
                    new Elem.Dual("Export", "Import", false, (s, e) => {
                        if (e.Selected) {
                            ExportPage.Visibility = Visibility.Visible;
                            ImportPage.Visibility = Visibility.Collapsed;
                        } else {
                            ExportPage.Visibility = Visibility.Collapsed;
                            ImportPage.Visibility = Visibility.Visible;
                        }
                    }),
                    new Elem.Newline(),
                    new Elem.Newline(),
                    ExportPage,
                    ImportPage
                });

                LoadedPage.Visibility = Visibility.Collapsed;

                this.Children.Add(UnloadedPage);
                this.Children.Add(LoadedPage);
            }

            public override void Load() {
                if (GDShare.DecodedCCLocalLevels) {
                    LoadedPage.Visibility = Visibility.Visible;
                    UnloadedPage.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}