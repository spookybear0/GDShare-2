using System;
using System.Windows;
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
                        Console.WriteLine(GDShare.DecodeCCFile("LocalLevels").Substring(0, 100));
                    })
                });

                LoadedPage = new Elem.Organized(new UIElement[] {
                    new Elem.Text("Sharing")
                });

                LoadedPage.Visibility = Visibility.Collapsed;

                this.Children.Add(UnloadedPage);
                this.Children.Add(LoadedPage);
            }

            public override void Load() {
                if (gdtools_cpp.Settings.DecodedCCLocalLevels) {
                    LoadedPage.Visibility = Visibility.Visible;
                    UnloadedPage.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}