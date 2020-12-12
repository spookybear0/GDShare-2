using System;
using System.Windows;
using System.Windows.Controls;

namespace gdtools_cpp {
    namespace Pages {
        public class Sharing : Page {
            private Elem.Generic LoadedPage;
            private Elem.Generic UnloadedPage;

            public Sharing(GDTWindow _w) : base(_w) {
                LoadedPage = new Elem.Generic();
                UnloadedPage = new Elem.Generic();

                LoadedPage.Visibility = Visibility.Collapsed;

                UnloadedPage.Children.Add(new Elem.Text("You need to decode CCLocalLevels to use Sharing!"));

                LoadedPage.Children.Add(new Elem.Text("Sharing"));

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