using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace gdtools_cpp {
    namespace Elem {
        public class Overlay : Grid {
            public Storyboard FadeTransition;
            public DoubleAnimation FadeAnimation;
            public ushort FadeDuration = 250;
            public bool Animating = false;
            public Elem.Div Text;
            public Elem.Header Head;

            public Overlay() {
                this.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.VerticalAlignment = VerticalAlignment.Stretch;
                this.Background = Theme.Colors.Overlay;
                this.Opacity = 0;
                this.IsHitTestVisible = false;

                Head = new Elem.Header("");

                this.Text = new Elem.Div(new UIElement[] {
                    Head,
                    new Elem.Text($"Accepted formats: {String.Join(", ", Settings.Ext.LevelList.Select(s => $".{s}"))}")
                }, Orientation.Vertical);

                this.Text.HorizontalAlignment = HorizontalAlignment.Center;
                this.Text.VerticalAlignment = VerticalAlignment.Center;
                this.Children.Add(this.Text);

                this.FadeTransition = new Storyboard();

                DoubleAnimation a = new DoubleAnimation {
                    From = 0.0,
                    To = 1.0,
                    FillBehavior = FillBehavior.Stop,
                    BeginTime = TimeSpan.FromSeconds(0),
                    Duration = new Duration(TimeSpan.FromMilliseconds(FadeDuration))
                };

                this.FadeTransition.Children.Add(a);
                Storyboard.SetTarget(a, this);
                Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
                this.FadeTransition.Completed += (s, e) => {
                    this.Opacity = 1;
                    Animating = false;
                };
            }

            public void Show(string[] _formats) {
                this.Background = Theme.Colors.Overlay;
                this.Head.Content = "Drop a file here!";

                foreach (string format in _formats)
                    if (!Settings.Ext.LevelList.Contains(format.Substring(format.LastIndexOf(".") + 1))) {
                        this.Background = Theme.Colors.OverlayError;
                        this.Head.Content = "File types not accepted!";
                    }

                if (!Animating) {
                    this.FadeTransition.AutoReverse = false;
                    this.FadeTransition.Begin();
                    Animating = true;
                }
            }

            public void Hide() {
                if (!Animating) {
                    Animating = true;
                    this.FadeTransition.AutoReverse = true;
                    this.FadeTransition.Begin(this, true);
                    ushort rem = (ushort)this.FadeTransition.GetCurrentTime().Milliseconds;
                    if (rem <= 0) rem = (ushort)(FadeDuration);
                    this.FadeTransition.Seek(
                        this,
                        new TimeSpan(0, 0, 0, 0, FadeDuration - rem),
                        TimeSeekOrigin.Duration
                    );
                }
            }
        }
    }
}