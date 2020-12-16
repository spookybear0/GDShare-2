using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace gdtools_cpp {
    namespace Elem {
        public class Shortcut : Border {
            public Shortcut(string _text = "", uint _size = 0) {
                Elem.Text Tx = new Elem.Text(_text, _size / 1.5, Theme.Colors.Text);
                Tx.Margin = new Thickness(0);
                Tx.VerticalAlignment = VerticalAlignment.Center;
                Tx.HorizontalAlignment = HorizontalAlignment.Center;

                this.Child = Tx;
                this.VerticalAlignment = VerticalAlignment.Center;
                this.HorizontalAlignment = HorizontalAlignment.Center;
                this.Height = _size;
                this.Padding = new Thickness(
                    Theme.Const.BorderSize, 0,
                    Theme.Const.BorderSize, 0
                );
                this.Background = Theme.Colors.Shortcut;
                this.CornerRadius = new CornerRadius(Theme.Const.BorderSize);

                this.Visibility = Settings.ShowShortcuts ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    
        public class Dual : Border {
            public Dualing Left;
            public Dualing Right;

            public class Dualing : Border {
                public Brush Color;
                public bool Selected = false;

                public Dualing(ushort _side, string _text, Handlers.CheckEventHandler _click = null) {
                    this.HorizontalAlignment = HorizontalAlignment.Stretch;
                    this.VerticalAlignment = VerticalAlignment.Stretch;
                    this.CornerRadius = _side == 0 ? new CornerRadius(
                        Theme.Const.Dual.Height / 2.1, 0,
                        0, Theme.Const.Dual.Height / 2.1
                    ) : new CornerRadius(
                        0, Theme.Const.Dual.Height / 2.1,
                        Theme.Const.Dual.Height / 2.1, 0
                    );
                    this.Color = _side == 0 ? Theme.Colors.Main : Theme.Colors.Secondary;

                    Elem.Text text = new Elem.Text(_text);
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.Margin = new Thickness(0);
                    text.FontSize = Theme.Const.Dual.Height / 2.5;

                    Grid.SetColumn(this, _side);

                    this.Child = text;

                    this.MouseEnter += (s, e) =>
                        this.Background = this.Selected ? Color : Theme.Colors.TitlebarHover;
                    
                    this.MouseLeave += (s, e) =>
                        this.Background = this.Selected ? Color : new SolidColorBrush(Colors.Transparent);
                    
                    this.MouseDown += (s, e) => {
                        this.Select(true);
                        if (_click != null)
                            _click(this, new Handlers.CheckEventArgs(_side == 0));
                    };
                }

                public void Select(bool _sel) {
                    this.Background = _sel ? Color : new SolidColorBrush(Colors.Transparent);
                    this.Selected = _sel;
                }
            }

            public Dual(string _textLeft, string _textRight, bool _def = false, Handlers.CheckEventHandler _click = null) {
                this.Width = Theme.Const.Dual.Width;
                this.Height = Theme.Const.Dual.Height;
                this.CornerRadius = new CornerRadius(Theme.Const.Dual.Height / 2);
                this.BorderThickness = new Thickness(Theme.Const.Dual.Border);
                this.BorderBrush = Theme.GenerateTwoColorBrush (
                    ((SolidColorBrush)Theme.Colors.Main).Color,
                    ((SolidColorBrush)Theme.Colors.Secondary).Color,
                    .5
                );

                this.ClipToBounds = true;

                Left = new Dualing(0, _textLeft, _click);
                Right = new Dualing(1, _textRight, _click);

                Left.MouseDown += (s, e) => Right.Select(false);
                Right.MouseDown += (s, e) => Left.Select(false);
                
                if (_def) Right.Select(true);
                else Left.Select(true);

                Grid Contents = new Grid();

                Contents.HorizontalAlignment = HorizontalAlignment.Stretch;
                Contents.VerticalAlignment = VerticalAlignment.Stretch;

                ColumnDefinition cl = new ColumnDefinition();
                cl.Width = new GridLength(50, GridUnitType.Star);

                ColumnDefinition cr = new ColumnDefinition();
                cr.Width = new GridLength(50, GridUnitType.Star);

                Contents.ColumnDefinitions.Add(cl);
                Contents.ColumnDefinitions.Add(cr);

                Contents.Children.Add(Left);
                Contents.Children.Add(Right);

                this.Child = Contents;
            }
        }
    }
}