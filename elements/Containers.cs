using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Controls;

namespace gdtools_cpp {
    namespace Elem {
        public class Global : Grid {
            public Main Main;
            public Titlebar Titlebar;

            public Global() {
                this.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.VerticalAlignment = VerticalAlignment.Stretch;
                this.Background = new SolidColorBrush(Colors.Transparent);
            }

            public void InitGlobal(GDTWindow _w) {
                this.Main = new Main();
                this.Titlebar = new Titlebar(_w);

                this.Children.Add(Titlebar);
                this.Children.Add(Main);
            }

            public UIElement Add(UIElement _e) {
                this.Main.Children.Add(_e);
                return _e;
            }
        }

        public class Main : Grid {
            public Main() {
                this.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.VerticalAlignment = VerticalAlignment.Stretch;
                this.Background = new SolidColorBrush(Colors.Transparent);

                this.Margin = new Thickness(0, Theme.Const.Titlebar.Height, 0, 0);
            }
        }

        public class Content : Grid {
            public Content() {
                this.Background = Theme.Colors.BGDark;
                this.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.VerticalAlignment = VerticalAlignment.Stretch;

                this.Margin = new Thickness(Theme.Const.Browser.Width, 0, 0, 0);
            }
        }
    
        public class Browser : Grid {
            public Browser() {
                this.Background = Theme.Colors.BGDarker;
                this.HorizontalAlignment = HorizontalAlignment.Left;
                this.VerticalAlignment = VerticalAlignment.Stretch;

                this.Width = Theme.Const.Browser.Width;
            }
        }
    }
}