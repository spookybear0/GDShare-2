using System;
using System.Linq;
using System.Windows;
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
    
        public class Div : StackPanel {
            public Div(UIElement[] _elem, Orientation _or = Orientation.Horizontal) {
                this.Orientation = _or;
                this.HorizontalAlignment = Settings.Alignment;

                foreach (UIElement elem in _elem)
                    this.Children.Add(elem);
            }
        }

        public class Highlight : Border {
            public enum Types {
                Normal
            }

            public Highlight(Types _type, UIElement _child) {
                this.CornerRadius = new CornerRadius(Theme.Const.BorderSize);
                this.Padding = Theme.Const.Page.Padding;
                this.HorizontalAlignment = Settings.Alignment;
                this.Margin = Theme.Const.Margin;

                switch (_type) {
                    case Types.Normal:
                        this.Background = Theme.Colors.Darker;
                        break;
                }

                if (_child is Div)
                    ((FrameworkElement)((Div)_child).Children[((Div)_child).Children.Count - 1]).Margin = new Thickness(0);

                this.Child = _child;
            }
        }

        public class Organized : StackPanel {
            public Organized(UIElement[] _elem) {
                this.HorizontalAlignment = Settings.Alignment;

                foreach (UIElement elem in _elem)
                    this.Children.Add(elem);
            }
        }

        public class Generic : Grid {
            public Generic() {
                this.HorizontalAlignment = Settings.Alignment;
            }
        }
    }
}