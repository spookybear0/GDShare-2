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
    }
}