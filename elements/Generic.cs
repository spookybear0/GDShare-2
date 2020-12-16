using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace gdtools_cpp {
    namespace Elem {
        public class Text : TextBlock {
            public Text(string _text = "", double _size = 0, Brush _c = null) {
                this.Text = _text;
                this.Foreground = _c == null ? Theme.Colors.Text : _c;
                this.VerticalAlignment = VerticalAlignment.Center;
                this.HorizontalAlignment = Settings.Alignment;
                this.FontSize = _size > 0 ? _size : Theme.Const.Text.Size;
                this.Margin = Theme.Const.Margin;
            }
        }

        public class Header : UserControl {
            public Header(string _text = "") {
                this.Content = _text;
                this.Foreground = Theme.Colors.Text;
                this.FontWeight = FontWeights.Bold;
                this.FontSize = Theme.Const.Text.HeaderSize;
                this.HorizontalAlignment = Settings.Alignment;
            }

            public Header(UIElement[] _elems) {
                this.FontWeight = FontWeights.Bold;
                this.FontSize = Theme.Const.Text.HeaderSize;
                this.HorizontalAlignment = Settings.Alignment;

                StackPanel s = new StackPanel();
                s.Orientation = Orientation.Horizontal;

                foreach (UIElement e in _elems) {
                    UIElement x = null;
                    if (e is Elem.Text) {
                        ((Elem.Text)e).FontSize = this.FontSize;
                        ((Elem.Text)e).FontWeight = FontWeights.Bold;
                    } else if (e is Canvas) {
                        x = new Viewbox();
                        ((Viewbox)x).Width = ((Viewbox)x).Height = Theme.Const.Text.HeaderSize;
                        ((Viewbox)x).Child = e;
                    }

                    if (x != null)
                        s.Children.Add(x);
                    else s.Children.Add(e);
                }

                this.Content = s;
            }
        }

        public class Newline : TextBlock {
            public Newline() {
                this.Height = this.Width = Theme.Const.Page.Newline;
            }
        }

        public class Pad : TextBlock {
            public Pad() {
                this.Height = this.Width = Theme.Const.Text.Padding;
            }
        }

        public class Img : Image {
            public Img(string _src = "", uint _w = 0, uint _h = 0, HorizontalAlignment? _a = null) {
                if (!_src.Contains("\\") && !_src.Contains("/"))
                    _src = $"resources\\Images\\{_src}";

                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri(_src, UriKind.RelativeOrAbsolute);
                bi3.EndInit();

                this.HorizontalAlignment = (_a is HorizontalAlignment) ? (HorizontalAlignment)_a : Settings.Alignment;

                this.Stretch = Stretch.Fill;
                if (_w > 0)
                    this.Width = _w;
                if (_h > 0)
                    this.Height = _h;
                this.Source = bi3;
            }
        }
    }
}