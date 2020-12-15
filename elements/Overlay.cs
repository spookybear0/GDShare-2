using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace gdtools_cpp {
    namespace Elem {
        public class Overlay : Grid {
            public Storyboard FadeTransition;
            public DoubleAnimation FadeAnimation;
            public ushort FadeDuration = 1;

            public Overlay() {
                this.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.VerticalAlignment = VerticalAlignment.Stretch;
                this.Background = Theme.Colors.Overlay;
                this.Opacity = 0;
                this.IsHitTestVisible = false;

                this.FadeTransition = new Storyboard();

                DoubleAnimation a = new DoubleAnimation {
                    From = 0.0,
                    To = 1.0,
                    FillBehavior = FillBehavior.Stop,
                    BeginTime = TimeSpan.FromSeconds(0),
                    Duration = new Duration(TimeSpan.FromSeconds(FadeDuration))
                };

                this.FadeTransition.Children.Add(a);
                this.FadeTransition.AutoReverse = true;
                Storyboard.SetTarget(a, this);
                Storyboard.SetTargetProperty(a, new PropertyPath(OpacityProperty));
                this.FadeTransition.Completed += (s, e) => {
                    this.Opacity = 1;
                };
            }

            public void Show() {
                this.FadeTransition.Begin();
            }

            public void Hide() {
                this.FadeTransition.Begin();
                this.FadeTransition.Pause();
                this.FadeTransition.Seek(new TimeSpan(FadeDuration / 2));
                this.FadeTransition.Resume();
            }
        }
    }
}