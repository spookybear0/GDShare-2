using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;

namespace gdtools_cpp {
    namespace Elem {
        public class Option : Border {
            public Grid Content;
            public bool Selected = false;
            public Brush BG = new SolidColorBrush(Colors.Transparent);
            public Brush BGHover = Theme.Colors.OptionHover;
            public Brush SL = Theme.Colors.TitlebarBG;
            public Brush SLHover = Theme.Colors.Secondary;
            public uint ID;
            public bool Hovering = false;
            public string Text = "";

            public Option(string _text, uint _id, Handlers.OptionSelectEventHandler _h) {
                this.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.VerticalAlignment = VerticalAlignment.Top;

                this.Background = new SolidColorBrush(Colors.Transparent);
                this.Padding = Theme.Const.Select.Option.Padding;
                this.CornerRadius = Theme.Const.Select.Option.Corner;
                this.Height = Theme.Const.Select.Option.Height;

                this.MouseEnter += (s, e) => {
                    this.Background = this.Selected ? this.SLHover : this.BGHover;
                    this.Hovering = true;
                };
                this.MouseLeave += (s, e) => {
                    this.Background = this.Selected ? this.SL : this.BG;
                    this.Hovering = false;
                };

                Elem.Text text = new Elem.Text(_text);
                text.Margin = new Thickness(0);
                text.VerticalAlignment = VerticalAlignment.Center;
                text.HorizontalAlignment = HorizontalAlignment.Center;

                this.Text = _text;

                this.Content = new Grid();
                this.Content.Children.Add(text);

                this.ID = _id;

                this.MouseDown += (s, e) => _h(this, new Handlers.OptionSelectEventArgs(this.ID, this.Text));

                this.Child = this.Content;
            }
        
            public void Select(bool _sel) {
                this.Selected = _sel;
                if (this.Hovering)
                    this.Background = this.Selected ? this.SLHover : this.BGHover;
                else
                    this.Background = this.Selected ? this.SL : this.BG;
            }
        }

        public class Select : Border {
            public StackPanel Options;
            public Elem.Text Title;
            public string TitleText;

            public Select(string _title = "", string[] _opt = null, bool _multi = false, uint _show = 0) {
                this.Background = Theme.Colors.Darker;
                this.Padding = Theme.Const.Select.Padding;
                this.CornerRadius = Theme.Const.Select.Corner;
                this.Height = (_show == 0 ? Theme.Const.Select.DefaultItemShow : _show) * 
                    (Theme.Const.Select.Option.Height) + 
                    Theme.Const.Select.Padding.Top * 2;
                this.Width = Theme.Const.Select.Width;

                Grid Content = new Grid();
                Content.HorizontalAlignment = HorizontalAlignment.Stretch;
                Content.VerticalAlignment = VerticalAlignment.Stretch;

                Title = new Elem.Text($"{_title} ({_opt.Length})", 0, Theme.Colors.TextDark);
                Title.HorizontalAlignment = HorizontalAlignment.Center;
                Title.VerticalAlignment = VerticalAlignment.Top;
                Title.Height = Theme.Const.Select.TitleHeight;
                Title.Margin = new Thickness(Theme.Const.Select.TitleHeight / 8);
                Title.IsHitTestVisible = false;
                Title.FontSize = Theme.Const.Select.TitleHeight / 3.5;

                TitleText = _title;

                ScrollViewer Scroll = new ScrollViewer();
                Scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                Scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                Scroll.HorizontalAlignment = HorizontalAlignment.Stretch;
                Scroll.VerticalAlignment = VerticalAlignment.Bottom;
                Scroll.Height = this.Height - Theme.Const.Select.TitleHeight;

                this.Options = new StackPanel();

                this.Options.Orientation = Orientation.Vertical;
                this.Options.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.Options.VerticalAlignment = VerticalAlignment.Stretch;

                if (_opt != null) {
                    uint ix = 0;
                    foreach (string opt in _opt)
                        this.Options.Children.Add(new Option(opt, ix++, (s, e) => {
                            bool ss = ((Option)s).Selected;
                            uint sel = !ss ? 1 : 0;

                            foreach (Option op in this.Options.Children) {
                                if (!_multi && op.ID != e.ID)
                                    op.Select(false);
                                else if (_multi && op.ID != e.ID && op.Selected)
                                    sel++;
                            }

                            if (sel > 0)
                                this.Title.Text = $"{this.TitleText} ({sel}/{this.Options.Children.Count})";
                            else this.Title.Text = $"{this.TitleText} ({this.Options.Children.Count})";

                            ((Option)s).Select(!ss);
                        }));
                }

                Scroll.Content = this.Options;

                Content.Children.Add(Scroll);
                Content.Children.Add(Title);
                
                this.Child = Content;
            }

            public List<string> GetSelection() {
                List<string> sel = new List<string> ();
                foreach (Option opt in this.Options.Children)
                    if (opt.Selected)
                        sel.Add(opt.Text);
                
                return sel;
            }

            public uint Search(string _kw) {
                uint found = 0;
                foreach (Option opt in this.Options.Children) {
                    if (opt.Text.ToLower().Trim().Contains(_kw.ToLower().Trim())) {
                        opt.Visibility = Visibility.Visible;
                        found++;
                    } else opt.Visibility = Visibility.Collapsed;
                }
                return found;
            }
        }
    
        public class Search : Border {
            public TextBox Input;

            public Search(Select _targ, string _placeholder = "Type here to search!") {
                this.Background = Theme.Colors.Darker;
                this.Padding = Theme.Const.Input.Padding;
                this.CornerRadius = Theme.Const.Input.Corner;
                this.Width = Theme.Const.Select.Width;
                this.Height = Theme.Const.Input.Height;

                Grid D = new Grid();

                this.Input = new TextBox();
                this.Input.HorizontalAlignment = HorizontalAlignment.Left;
                this.Input.VerticalAlignment = VerticalAlignment.Center;
                this.Input.Width = this.Width - Theme.Const.Input.Height;
                this.Input.Foreground = Theme.Colors.Text;
                this.Input.CaretBrush = Theme.Colors.Text;
                this.Input.Background = new SolidColorBrush(Colors.Transparent);
                this.Input.BorderThickness = new Thickness(0);
                this.Input.FontSize = Theme.Const.Input.Height / 3;

                Elem.Text ph = new Elem.Text(_placeholder, 0, Theme.Colors.TextDark);
                ph.HorizontalAlignment = HorizontalAlignment.Left;
                ph.VerticalAlignment = VerticalAlignment.Center;
                ph.Margin = new Thickness(0);

                Elem.Text ct = new Elem.Text();
                ct.HorizontalAlignment = HorizontalAlignment.Right;
                ct.VerticalAlignment = VerticalAlignment.Center;
                ct.Margin = new Thickness(0);
                ct.FontWeight = FontWeights.Bold;

                Viewbox v = new Viewbox();
                v.HorizontalAlignment = HorizontalAlignment.Right;
                v.VerticalAlignment = VerticalAlignment.Stretch;
                v.Width = Theme.Const.Input.Height - Theme.Const.Input.Padding.Top * 2.5;
                v.Height = Theme.Const.Input.Height - Theme.Const.Input.Padding.Top * 2.5;

                v.Child = Theme.LoadIcon("Search", Theme.Colors.TextDark);

                this.Input.TextChanged += (s, e) => {
                    ph.Visibility = this.Input.Text.Length > 0 ? Visibility.Hidden : Visibility.Visible;

                    uint found = _targ.Search(this.Input.Text);
                    if (_targ.Options.Children.Count == found) {
                        ct.Text = "";
                        v.Visibility = Visibility.Visible;
                    } else {
                        ct.Text = $"{found}";
                        v.Visibility = Visibility.Collapsed;
                    }
                };

                D.Children.Add(ph);
                D.Children.Add(ct);
                D.Children.Add(this.Input);
                D.Children.Add(v);

                this.Child = D;
            }
        }
    }
}