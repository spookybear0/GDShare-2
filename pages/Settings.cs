using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace gdtools_cpp {
    namespace Pages {
        public class SettingsPage : Page {
            public SettingsPage(GDTWindow _w) : base(_w) {
                Elem.Highlight reload = new Elem.Highlight(Elem.Highlight.Types.Normal,
                    new Elem.Div(new UIElement[] {
                        new Elem.Text("A reload is required to apply these settings.", 0, Theme.Colors.Main),
                        new Elem.But("Reload", default(Size), (s, e) => App.Reload())
                    }, Orientation.Vertical)
                );
                reload.Visibility = Visibility.Collapsed;

                this.Children.Add(new Elem.Organized(new UIElement[] {
                    new Elem.Header(new UIElement[] {
                        Theme.LoadIcon("Cog"),
                        new Elem.Pad(),
                        new Elem.Text("Settings")
                    }),
                    new Elem.Pad(),
                    reload,
                    new Elem.Organized(new UIElement[] {
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
                    }),
                    new Elem.Pad(),
                    new Elem.Toggle("Show Keyboard Shortcuts", "Hide Keyboard Shortcuts", gdtools_cpp.Settings.ShowShortcuts, (s, e) => {
                        foreach (Elem.Shortcut sc in GDTWindow.FindVisualChildren<Elem.Shortcut>(_w))
                            sc.Visibility = e.Selected ? Visibility.Visible : Visibility.Collapsed;
                    })
                }));
            }
        }
    }
}