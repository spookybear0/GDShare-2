using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;

namespace gdtools_cpp {
    public class Userdata {
        public class DataT {
            public Size WindowSize { get; set; }
            public bool IsTransparent { get; set; } = true;
            public bool SaveWindowState { get; set; } = true;
            public bool CenterContent { get; set; } = true;
        }

        public DataT Data;
        public bool Loaded = false;
        public string UserdataPath = Settings.UserdataName + "." + Settings.Ext.Data;

        public void LoadData() {
            if (System.IO.File.Exists(UserdataPath)) {
                string data = System.IO.File.ReadAllText(UserdataPath);
                
                if (data.StartsWith($"{Theme.GetGDTFileHead("User")}\n")) {
                    Data = JsonSerializer.Deserialize<DataT>(data.Substring(data.IndexOf("\n") + 1));
                    Loaded = true;

                    Settings.Alignment = Data.CenterContent ? HorizontalAlignment.Center : HorizontalAlignment.Left;
                } else Data = new DataT();
            } else Data = new DataT();
        }

        public void SaveData() {
            System.IO.File.WriteAllText(UserdataPath, $"{Theme.GetGDTFileHead("User")}\n{JsonSerializer.Serialize(Data)}");
        }
    }

    public partial class GDTWindow : Window {
        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref AcrylicWPF.WindowCompositionAttributeData data);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hwnd, uint msg, uint wparam, uint hparam);

        public short FadeTransition = 0;
        public short FadeTransitionLength = 60;
        public short Fading = 0;
        public float FadeMove = .5f;
        public EventHandler eDrawFade;
        public gWindowContent Contents;

        public void AddShortcut(KeyGesture _k, ExecutedRoutedEventHandler _e) {
            RoutedCommand c = new RoutedCommand();
            c.InputGestures.Add(_k);

            CommandBinding cc = new CommandBinding();
            cc.Command = c;
            cc.Executed += _e;

            this.CommandBindings.Add(cc);
        }

        public static void AllowUIToUpdate() {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(parameter => {
                frame.Continue = false;
                return null;
            }), null);

            Dispatcher.PushFrame(frame);
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject {
            if (depObj != null)
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)  {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
        }

        public GDTWindow(gWindowContent _wc) {
            this.Contents = _wc;

            _wc.Initialize(this);

            this.Title = _wc.Name.Replace("_", " ");
            if (App.Userdata.Loaded && _wc.Name == Settings.AppName && App.Userdata.Data.SaveWindowState) {
                this.Width = App.Userdata.Data.WindowSize.Width;
                this.Height = App.Userdata.Data.WindowSize.Height;
            } else {
                this.Width = _wc.Size.Width;
                this.Height = _wc.Size.Height;
            }
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (App.Userdata.Loaded && !App.Userdata.Data.IsTransparent) {
                this.AllowsTransparency = false;
                Contents.Global.Titlebar.Visibility = Visibility.Hidden;
                Contents.Global.Main.Margin = new Thickness(0);
            } else {
                this.WindowStyle = WindowStyle.None;
                this.AllowsTransparency = true;
                this.ResizeMode = ResizeMode.CanResizeWithGrip;
                this.SizeChanged += (s, e) =>
                    this.BorderThickness = this.WindowState == WindowState.Maximized ? new Thickness(7) : new Thickness(0);
            }
            this.Background = new SolidColorBrush(Colors.Transparent);

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("resources/g.ico", UriKind.Relative);
            bi3.EndInit();
            this.Icon = bi3;

            this.Content = _wc.Global;

            this.Closing += (s, e) => {
                App.Userdata.Data.WindowSize = new Size(this.Width, this.Height);

                App.Userdata.SaveData();
            };

            this.Loaded += (s, e) => this.EnableBlur();
        }

        public void DrawFade(object sender, EventArgs e) {
            if (Fading != 0) {
                FadeTransition += Fading;
                if (FadeTransition <= 0) {
                    FadeTransition = 0;
                    Fading = 0;
                    CompositionTarget.Rendering -= DrawFade;
                } else if (FadeTransition >= FadeTransitionLength) {
                    FadeTransition = FadeTransitionLength;
                    Fading = 0;
                    CompositionTarget.Rendering -= DrawFade;
                }

                this.Opacity = 1f - ((float)FadeTransition / FadeTransitionLength) * FadeMove;
            } else
                CompositionTarget.Rendering -= DrawFade;
        }

        private void EnableBlur() {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AcrylicWPF.AccentPolicy {
                AccentState = AcrylicWPF.AccentState.ACCENT_ENABLE_BLURBEHIND
            };

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new AcrylicWPF.WindowCompositionAttributeData {
                Attribute = AcrylicWPF.WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        public void Refresh() {}

        public void CloseWindow(object s, EventArgs e) {
            this.Close();
        }

        public void MoveWindow(object s, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}