using System;
using System.Windows;
using System.Windows.Controls;

namespace gdtools_cpp {
    namespace Pages {
        public class Settings : Page {
            public Settings(GDTWindow _w) : base(_w) {
                this.Children.Add(new Elem.Header(new UIElement[] {
                    Theme.LoadIcon("Cog"),
                    new Elem.Pad(),
                    new Elem.Text("Settings")
                }));
                this.Children.Add(new Elem.Newline());
                this.Children.Add(new Elem.Checkbox("Transparent window", Userdata.UserdataType.Window.IsTransparent, (s, e) =>
                    Userdata.UserdataType.Window.IsTransparent = e.Selected));
            }
        }
    }
}