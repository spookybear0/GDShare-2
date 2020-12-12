using System;
using System.Windows;
using System.Windows.Controls;

namespace gdtools_cpp {
    namespace Pages {
        public class Home : Page {
            public Home(GDTWindow _w) : base(_w) {
                this.Children.Add(new Elem.Text("swag home page"));
                this.Children.Add(new Elem.But("Decode", default(Size), (s, e) => {
                    gdtools_cpp.Settings.DecodedCCLocalLevels = true;
                }));
            }
        }
    }
}