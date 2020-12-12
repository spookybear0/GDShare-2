using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace gdtools_cpp {
    namespace Elem {
        public class TitlebarBut : ButtonStyled {
            public TitlebarBut(Canvas _icon, Size _size, RoutedEventHandler _click = null) {
                Viewbox v = new Viewbox();
                v.Width = _size.Width / 2;
                v.Height = _size.Height / 2;

                v.Child = _icon;
                this.Content = v;
                
                this.Width = _size.Width;
                this.Height = _size.Height;

                if (_click != null) this.Click += _click;

                this.InitStyle(Theme.Colors.TitlebarText, new SolidColorBrush(Colors.Transparent), Theme.Colors.TitlebarHover);

                this.HorizontalAlignment = HorizontalAlignment.Right;
                
                this.Style = this.__Style;
            }
        }

        public class Titlebar : Grid {
            public Titlebar(GDTWindow _w) {
                this.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.VerticalAlignment = VerticalAlignment.Top;
                this.Background = Theme.Colors.TitlebarBG;
                this.Height = Theme.Const.Titlebar.Height;
                
                this.MouseDown += (s, e) => {
                    if (e.ClickCount == 2)
                        if (_w.WindowState == WindowState.Maximized)
                            _w.WindowState = WindowState.Normal;
                        else _w.WindowState = WindowState.Maximized;
                };

                Elem.TitlebarBut cB(string _icon, RoutedEventHandler _r) {
                    return new Elem.TitlebarBut(
                        Theme.LoadIcon(_icon, Theme.Colors.TitlebarText),
                        new Size(
                            Theme.Const.Titlebar.Height,
                            Theme.Const.Titlebar.Height
                        ), _r
                    );
                }

                foreach (Elem.TitlebarBut x in (IEnumerable<Elem.TitlebarBut>)(new Elem.TitlebarBut[] {
                    cB("Close", _w.CloseWindow)
                })) this.Children.Add(x);

                Elem.Header title = new Elem.Header(_w.Name.Replace("_", " "));

                title.HorizontalAlignment = HorizontalAlignment.Center;
                title.VerticalAlignment = VerticalAlignment.Center;
                title.FontSize = Theme.Const.Titlebar.Height / 2;
                title.Foreground = Theme.Colors.TitlebarText;

                this.Children.Add(title);
            }
        }
    }
}