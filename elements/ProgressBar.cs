using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;

namespace gdtools_cpp {
    namespace Elem {
        public class ProgressBars : StackPanel {
            public ProgressBars() {
                this.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.VerticalAlignment = VerticalAlignment.Bottom;
                this.Background = new SolidColorBrush(Colors.Transparent);
            }

            public Elem.ProgressBar AddBar() {
                Elem.ProgressBar pb = new Elem.ProgressBar();
                this.Children.Add(pb);
                return pb;
            }
        }

        public class ProgressBar : Grid {
            public Border Bar;
            public Border BarBG;
            public Elem.Text ProgressText;
            public Storyboard FadeAnimation;
            public Brush EndColor = Theme.Colors.Success;
            public Brush FailColor = Theme.Colors.Failure;
            public Brush LoadColor = Theme.Colors.TitlebarBG;
            public bool Hiding = false;

            public ProgressBar() {
                this.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.VerticalAlignment = VerticalAlignment.Bottom;
                this.Height = Theme.Const.ProgressBar.Height;
                this.Margin = new Thickness(Theme.Const.Browser.Width, 0, 0, 0);
                this.Background = Theme.Colors.Darkest;

                this.Bar = new Border();
                this.Bar.HorizontalAlignment = HorizontalAlignment.Left;
                this.Bar.VerticalAlignment = VerticalAlignment.Stretch;
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

                this.ProgressText = new Elem.Text();
                this.ProgressText.HorizontalAlignment = HorizontalAlignment.Center;
                this.ProgressText.VerticalAlignment = VerticalAlignment.Top;
                this.ProgressText.Margin = new Thickness(0);
                this.ProgressText.Foreground = Theme.Colors.Text;

                this.FadeAnimation = new Storyboard();

                this.Children.Add(this.BarBG);
                this.Children.Add(this.Bar);
                this.Children.Add(this.ProgressText);

                this.Visibility = Visibility.Collapsed;
                this.Name = "__ProgessBar";
            }

            public void Hide() {
                if (this.Hiding)
                    return;
                this.Hiding = true;
                DoubleAnimation a = new DoubleAnimation {
                    From = Theme.Const.ProgressBar.Height,
                    To = 0.0,
                    FillBehavior= FillBehavior.Stop,
                    BeginTime = TimeSpan.FromSeconds(1),
                    Duration = new Duration(TimeSpan.FromSeconds(1))
                };

                this.FadeAnimation.Children.Add(a);
                Storyboard.SetTarget(a, this);
                Storyboard.SetTargetProperty(a, new PropertyPath(HeightProperty));
                this.FadeAnimation.Completed += (s, e) => {
                    ((Elem.ProgressBars)this.Parent).Children.Remove(this);
                };
                this.FadeAnimation.Begin();
            }

            public void Finish(string _txt = null) {
                if (this.Hiding)
                    return;
                this.Bar.Width = Double.NaN;
                this.Bar.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.Bar.Background = this.EndColor;
                if (_txt != null)
                    this.ProgressText.Text = _txt;
                this.Hide();
            }

            public void Fail(string _txt = null) {
                this.Bar.Background = this.FailColor;
                if (_txt != null)
                    this.ProgressText.Text = _txt;
                this.Hide();
            }

            public void SetProgress(ushort _prog, string _txt) {
                if (_prog == 0)
                    GDTWindow.AllowUIToUpdate();
                this.Bar.Background = this.LoadColor;

                this.Bar.Width = this.BarBG.ActualWidth * (_prog / 100d);
                this.Visibility = Visibility.Visible;
                this.ProgressText.Text = _txt;
                this.Bar.HorizontalAlignment = HorizontalAlignment.Left;

                this.FadeAnimation.Pause();
                this.FadeAnimation.Seek(new TimeSpan(0));

                GDTWindow.AllowUIToUpdate();
            }
        }
    }
}