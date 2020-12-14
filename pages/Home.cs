using System;
using System.Windows;
using System.Windows.Controls;

namespace gdtools_cpp {
    namespace Pages {
        public class Home : Page {
            public Home(GDTWindow _w) : base(_w) {
                this.Children.Add(new Elem.Organized(new UIElement[] {
                    new Elem.Text("swag home page"),
                    new Elem.But("Decode", default(Size), (s, e) => {
                        for (int i = 0; i < 100; i += 20) {
                            _w.Contents.ProgressBar.SetProgress((ushort)i);
                        }
                    })
                }));
            }
        }
    }
}