using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace gdtools_cpp {
    namespace Elem {
        public class ProgressBar : Grid {
            public Border Bar;
            public Border BarBG;
            public double BarWidth;

            public ProgressBar() {
                this.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.VerticalAlignment = VerticalAlignment.Bottom;
                this.Height = Theme.Const.ProgressBar.Height;
                this.Margin = new Thickness(Theme.Const.Browser.Width, 0, 0, 0);
                this.Background = Theme.Colors.Darkest;

                this.Bar = new Border();
                this.Bar.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.Bar.VerticalAlignment = VerticalAlignment.Stretch;
                this.Bar.Background = Theme.Colors.Main;
                this.Bar.Margin = Theme.Const.ProgressBar.Padding;
                this.Bar.CornerRadius = new CornerRadius(
                    (Theme.Const.ProgressBar.Height - Theme.Const.ProgressBar.Padding.Top * 2) / 2
                );

                this.BarBG = new Border();
                this.BarBG.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.BarBG.VerticalAlignment = VerticalAlignment.Stretch;
                this.BarBG.Background = Theme.Colors.BGDark;
                this.BarBG.Margin = Theme.Const.ProgressBar.Padding;
                this.BarBG.CornerRadius = new CornerRadius(
                    (Theme.Const.ProgressBar.Height - Theme.Const.ProgressBar.Padding.Top * 2) / 2
                );

                this.Children.Add(this.BarBG);
                this.Children.Add(this.Bar);

                this.Visibility = Visibility.Collapsed;
                this.Name = "__ProgessBar";
                this.Bar.Visibility = Visibility.Hidden;
            }

            public void SetProgress(ushort _prog) {
                Console.WriteLine(this.BarBG.ActualWidth);
                this.Bar.Width = this.BarBG.ActualWidth * (_prog / 100);
                this.Visibility = Visibility.Visible;
            }
        }
    }
}