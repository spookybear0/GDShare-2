using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace gdtools_cpp {
    public class gWindowContent {
        public Elem.Global Global;
        public GDTWindow Window;
        public Elem.ProgressBars ProgressBars;

        public gWindowContent() {
            this.ProgressBars = new Elem.ProgressBars();
            this.Global = new Elem.Global();
        }

        public void InitializeBase(GDTWindow _w) {
            this.Global.InitGlobal(_w);
            this.Window = _w;

            this.Global.Titlebar.MouseDown += (s, e) => {
                if (e.ClickCount == 1 && e.ChangedButton == MouseButton.Left) {
                    this.Window.Fading = 2;
                    CompositionTarget.Rendering += this.Window.DrawFade;
                }
            };
            this.Global.Titlebar.MouseUp += (s, e) => {
                this.Window.Fading = -4;
                CompositionTarget.Rendering += this.Window.DrawFade;
            };
            this.Global.Titlebar.MouseDown += this.Window.MoveWindow;
        }

        public virtual void Initialize(GDTWindow _w) {}

        public Size Size { get; set; }
        public string Name { get; set; }
    }

    public class MainWindow : gWindowContent {
        public MainWindow() {
            this.Name = Settings.AppName;
        }

        struct TabType {
            public string Name;
            public Canvas Icon;
            public Pages.Page Page;
            public bool IsBottomSplit;
        }

        public override void Initialize(GDTWindow _w) {
            _w.Name = this.Name;

            this.InitializeBase(_w);

            this.Size = new Size(1280, 720);

            Elem.Content c = (Elem.Content)this.Global.Add(new Elem.Content());
            Elem.Browser b = (Elem.Browser)this.Global.Add(new Elem.Browser());
            this.Global.Add(this.ProgressBars);
            Elem.Overlay o = (Elem.Overlay)this.Global.Add(new Elem.Overlay());

            StackPanel bTop = new StackPanel();
            StackPanel bBot = new StackPanel();
            bBot.VerticalAlignment = VerticalAlignment.Bottom;

            this.Global.AllowDrop = true;

            this.Global.DragEnter += (s, e) => o.Show((string[])e.Data.GetData(DataFormats.FileDrop));
            this.Global.DragLeave += (s, e) => o.Hide();

            this.Global.Drop += (s, e) => {
                if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                    foreach (string file in files)
                        Console.WriteLine(file);
                }
                o.Hide();
            };

            b.Children.Add(bBot);
            b.Children.Add(bTop);

            StackPanel targ = bTop;

            int ix = 0;
            foreach (TabType t in new TabType[] {
                new TabType { Name = "Home", Icon = Theme.LoadIcon("Home", Theme.Colors.BrowserIcon), Page = new Pages.Home(_w) },
                new TabType { Name = "Sharing", Icon = Theme.LoadIcon("Share", Theme.Colors.BrowserIcon), Page = new Pages.Sharing(_w) },
                new TabType { Name = "Backups", Icon = Theme.LoadIcon("Backup", Theme.Colors.BrowserIcon), Page = new Pages.None(_w) },
                new TabType { Name = "Collab", Icon = Theme.LoadIcon("Collab", Theme.Colors.BrowserIcon), Page = new Pages.None(_w) },
                new TabType { Name = "Live", Icon = Theme.LoadIcon("Live", Theme.Colors.BrowserIcon), Page = new Pages.None(_w) },
                new TabType { Name = "External", Icon = Theme.LoadIcon("Hack", Theme.Colors.BrowserIcon), Page = new Pages.None(_w) },
                new TabType { IsBottomSplit = true },
                new TabType { Name = "About", Icon = Theme.LoadIcon("About", Theme.Colors.BrowserIcon), Page = new Pages.None(_w) },
                new TabType { Name = "Settings", Icon = Theme.LoadIcon("Cog", Theme.Colors.BrowserIcon), Page = new Pages.SettingsPage(_w) }
            }) {
                if (!t.IsBottomSplit) {
                    Elem.BrowserBut bt = new Elem.BrowserBut(t.Name, t.Icon, ix++, $"Ctrl + {ix}");

                    bt.Click += (s, e) => {
                        foreach (Elem.BrowserBut b in GDTWindow.FindVisualChildren<Elem.BrowserBut>(_w))
                            b.Select(false);
                        bt.Select(true);

                        c.Children.Clear();
                        c.Children.Add(t.Page);

                        t.Page.Load();
                    };

                    if (ix <= 9)
                        _w.AddShortcut(new KeyGesture((Key)(34 + ix), ModifierKeys.Control), (s, e) => {
                            bt.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        });

                    targ.Children.Add(bt);

                    if (ix == 1) {
                        bt.Select(true);
                        c.Children.Add(t.Page);
                    }
                } else targ = bBot;
            }
        }
    }
}