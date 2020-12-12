using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace gdtools_cpp {
    namespace Elem {
        public class ButtonStyled : Button {
            public Style __Style;

            public void InitStyle(
                Brush _Text, Brush _BG, Brush _BGHover, uint _bsize = 0,
                HorizontalAlignment _h = HorizontalAlignment.Center, Thickness _p = default(Thickness),
                Brush _TextHover = null
            ) {
                Style style = new Style(typeof(Button));

                ControlTemplate temp = new ControlTemplate();
                temp.TargetType = typeof(Button);

                FrameworkElementFactory b = new FrameworkElementFactory(typeof(Border));
                b.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Border.BackgroundProperty));
                b.SetValue(Border.PaddingProperty, _p);
                if (_bsize > 0)
                    b.SetValue(Border.CornerRadiusProperty, new CornerRadius(_bsize));

                FrameworkElementFactory p = new FrameworkElementFactory(typeof(ContentPresenter));
                p.SetValue(ContentPresenter.HorizontalAlignmentProperty, _h);
                p.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

                b.AppendChild(p);

                temp.VisualTree = b;

                style.Setters.Add(new Setter(Button.ForegroundProperty, _Text));
                style.Setters.Add(new Setter(Button.BackgroundProperty, _BG));
                style.Setters.Add(new Setter(Button.TemplateProperty, temp));

                Trigger HoverE = new Trigger();
                HoverE.Property = UIElement.IsMouseOverProperty;
                HoverE.Value = true;
                HoverE.Setters.Add(new Setter(Button.BackgroundProperty, _BGHover));
                if (_TextHover != null)
                    HoverE.Setters.Add(new Setter(Button.ForegroundProperty, _TextHover));

                style.Triggers.Add(HoverE);
                
                __Style = style;
            }

            public ButtonStyled() {}
        }

        public class But : ButtonStyled {
            public But(string _text = "", Size _size = default(Size), RoutedEventHandler _click = null) {
                if (_text != "") this.Content = _text;
                if (_size != default(Size)) {
                    this.Width = _size.Width;
                    this.Height = _size.Height;
                }
                if (_click != null) this.Click += _click;

                this.Padding = Theme.Const.Button.Padding;
                this.HorizontalAlignment = Settings.Alignment;

                this.InitStyle(
                    Theme.Colors.Text,
                    Theme.Colors.ButtonBG,
                    Theme.Colors.ButtonBGHover,
                    Theme.Const.BorderSize,
                    HorizontalAlignment.Center,
                    this.Padding
                );

                this.Style = this.__Style;
                this.Margin = Theme.Const.Margin;
            }
        }
        
        public class BrowserBut : ButtonStyled {
            public bool Selected;
            public Brush BGColor = new SolidColorBrush(Colors.Transparent);
            public Brush BGHover = Theme.Colors.BrowserHover;
            public Brush TXColor = Theme.Colors.BrowserIcon;
            public Brush TXHover = Theme.Colors.Text;
            public Elem.Text text;

            public void Select(bool _sel) {
                if (_sel) {
                    this.Background = Theme.Colors.BrowserSelected;
                    text.Foreground = TXHover;
                } else {
                    this.Background = this.BGColor;
                    text.Foreground = TXColor;
                }

                this.Selected = _sel;
            }

            public BrowserBut(string _text = "", Canvas _icon = null, int _c = 0) {
                SolidColorBrush IconHover = Theme.ColorArrays.IconColors[_c];

                text = new Elem.Text(_text, (uint)(Theme.Const.Browser.TabHeight / 2.5));
                text.Margin = new Thickness(0);
                if (_icon != null) {
                    StackPanel p = new StackPanel();
                    p.Orientation = Orientation.Horizontal;

                    Viewbox v = new Viewbox();
                    v.Width = Theme.Const.Browser.TabHeight - Theme.Const.Browser.IconPadding;
                    v.Height = Theme.Const.Browser.TabHeight - Theme.Const.Browser.IconPadding;
                    v.Child = _icon;
                    p.Children.Add(v);

                    text.Foreground = Theme.Colors.BrowserIcon;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.Padding = Theme.Const.Browser.TextPadding;
                    p.Children.Add(text);

                    this.Content = p;
                } else this.Content = text;

                this.Width = Theme.Const.Browser.Width;
                this.Height = Theme.Const.Browser.TabHeight;
                this.VerticalAlignment = VerticalAlignment.Top;

                this.InitStyle(
                    this.TXColor,
                    this.BGColor,
                    this.BGHover,
                    0,
                    HorizontalAlignment.Left,
                    Theme.Const.Browser.TextPadding,
                    this.TXHover
                );

                this.MouseEnter += (s, e) => {
                    if (!this.Selected)
                        text.Foreground = this.TXHover;
                    if (_icon != null)
                        Theme.UpdateCanvasColor(_icon, IconHover);
                    if (!this.Selected)
                        this.Background = this.BGHover;
                };
                this.MouseLeave += (s, e) => {
                    if (!this.Selected)
                        text.Foreground = this.TXColor;
                    if (_icon != null)
                        Theme.UpdateCanvasColor(_icon, Theme.Colors.BrowserIcon);
                    if (!this.Selected)
                        this.Background = this.BGColor;
                };

                this.Style = this.__Style;
            }
        }

        public class Checkbox : UserControl {
            public bool Selected;
            public Border Box;

            public class CheckEventArgs : EventArgs {
                public bool Selected;

                public CheckEventArgs(bool _sel) {
                    Selected = _sel;
                }
            }
            public delegate void CheckEventHandler(object sender, CheckEventArgs e);

            public static class C {
                public static Brush Selected = Theme.Colors.ButtonBG;
                public static Brush SelectedHover = Theme.Colors.ButtonBGHover;
                public static Brush BG = Theme.Colors.Dark;
                public static Brush BGHover = Theme.Colors.Light;
            }

            public Checkbox(string _text = "", bool _select = false, CheckEventHandler _click = null) {
                Grid contents = new Grid();

                Box = new Border();
                Box.Width = Box.Height = Theme.Const.Checkbox.Size;
                Box.CornerRadius = Theme.Const.Checkbox.CornerRadius;
                Box.Background = _select ? C.Selected : C.BG;

                this.Selected = _select;

                Canvas Tick = Theme.LoadIcon("Tick");
                Tick.Visibility = _select ? Visibility.Visible : Visibility.Hidden;

                Viewbox v = new Viewbox();
                v.Width = v.Height = Theme.Const.Checkbox.Size / Theme.Const.Checkbox.TickRatio;
                v.VerticalAlignment = VerticalAlignment.Center;
                v.Child = Tick;
                v.Margin = new Thickness(Box.Width / 2 - v.Width / 2);

                Box.Child = v;
                Box.HorizontalAlignment = HorizontalAlignment.Left;

                Grid vv = new Grid();
                vv.HorizontalAlignment = HorizontalAlignment.Stretch;
                vv.VerticalAlignment = VerticalAlignment.Stretch;

                /*
                uint marg = Settings.Alignment == HorizontalAlignment.Center ?
                    Theme.Const.Checkbox.Margin * 5 :
                    Theme.Const.Checkbox.Margin
                    + (uint)Theme.Const.Checkbox.Size;      //*/

                vv.Margin = new Thickness(Theme.Const.Checkbox.Margin + (uint)Theme.Const.Checkbox.Size, 0, 0, 0);

                Elem.Text t = new Elem.Text(_text);
                t.Margin = new Thickness(0);
                t.VerticalAlignment = VerticalAlignment.Center;
                t.HorizontalAlignment = HorizontalAlignment.Left;

                vv.Children.Add(t);

                contents.Children.Add(Box);
                contents.Children.Add(vv);

                contents.HorizontalAlignment = HorizontalAlignment.Stretch;

                this.Content = contents;
                this.HorizontalAlignment = Settings.Alignment == HorizontalAlignment.Center ? HorizontalAlignment.Stretch : Settings.Alignment;
                this.Margin = Theme.Const.Margin;

                this.MouseEnter += (s, e) => {
                    if (this.Selected)
                        Box.Background = C.SelectedHover;
                    else Box.Background = C.BGHover;
                };
                this.MouseLeave += (s, e) => {
                    if (this.Selected)
                        Box.Background = C.Selected;
                    else Box.Background = C.BG;
                };
            
                this.MouseDown += (s, e) => {
                    this.Selected = !this.Selected;

                    if (this.Selected) {
                        Box.Background = C.SelectedHover;
                        Tick.Visibility = Visibility.Visible;
                    } else {
                        Box.Background = C.BGHover;
                        Tick.Visibility = Visibility.Hidden;
                    }

                    if (_click != null)
                        _click(s, new CheckEventArgs(this.Selected));
                };
            }
        }
    }
}