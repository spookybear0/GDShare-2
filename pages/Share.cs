using System;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace gdtools_cpp {
    namespace Pages {
        public class Sharing : Page {
            private Elem.Organized LoadedPage;
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
                    foreach (string lvl in lvls)
                        Console.WriteLine(lvl);
                });

                LoadedPage = new Elem.Organized(new UIElement[] {
                    new Elem.Text("Sharing"),
                    new Elem.Select(new string[] {
                        "epic",
                        "awesome",
                        "stupid"
                    })
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