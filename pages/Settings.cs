using System;
using System.Diagnostics;
using System.Reflection;
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
                this.Children.Add(new Elem.Pad());

                Elem.Highlight reload = new Elem.Highlight(Elem.Highlight.Types.Normal,
                    new Elem.Div(new UIElement[] {
                        new Elem.Text("A reload is required to apply these settings.", 0, Theme.Colors.Main),
                        new Elem.But("Reload", default(Size), (s, e) => App.Reload())
                    }, Orientation.Vertical)
                );
                reload.Visibility = Visibility.Collapsed;

                this.Children.Add(reload);

                this.Children.Add(new Elem.Organized(new UIElement[] {
                    new Elem.Checkbox("Transparent window", App.Userdata.Data.IsTransparent, (s, e) => {
                        App.Userdata.Data.IsTransparent = e.Selected;
                        reload.Visibility = Visibility.Visible;
                    }),

                    new Elem.Checkbox("Save window size", App.Userdata.Data.SaveWindowState, (s, e) => {
                        App.Userdata.Data.SaveWindowState = e.Selected;
                    }),

                    new Elem.Checkbox("Center content", App.Userdata.Data.CenterContent, (s, e) => {
                        App.Userdata.Data.CenterContent = e.Selected;
                        reload.Visibility = Visibility.Visible;
                    })
                }));
            }
        }
    }
}