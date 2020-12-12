using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text.Json;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace gdtools_cpp {
    public static class Userdata {
        public class UserdataType {
            public static class Window {
                public static Size Size;
                public static bool IsTransparent;
            }
        }

        public static UserdataType Data;
        public static bool Loaded = false;
        public static string UserdataPath = Settings.UserdataName + "." + Settings.Ext.Data;

        public static bool LoadData() {
            if (System.IO.File.Exists(UserdataPath)) {
                string data = System.IO.File.ReadAllText(UserdataPath);
                
                if (data.StartsWith("::GDTools User::\n")) {
                    Data = JsonSerializer.Deserialize<UserdataType>(data.Substring(data.IndexOf("\n") + 1));
                    Loaded = true;
                    return true;
                } else return false;
            } else return false;
        }

        public static void SaveData() {
            System.IO.File.WriteAllText(UserdataPath, $"::GDTools User::\n{JsonSerializer.Serialize(Data)}");
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
            this.Width = _wc.Size.Width;
            this.Height = _wc.Size.Height;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (Userdata.Loaded && !Userdata.UserdataType.Window.IsTransparent) {
                this.AllowsTransparency = false;
                Contents.Global.Titlebar.Visibility = Visibility.Hidden;
                Contents.Global.Main.Margin = new Thickness(0);
            } else {
                this.WindowStyle = WindowStyle.None;
                this.AllowsTransparency = true;
            }
            this.Background = new SolidColorBrush(Colors.Transparent);
            this.SizeChanged += (s, e) =>
                this.BorderThickness = this.WindowState == WindowState.Maximized ? new Thickness(8) : new Thickness(0);

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("resources/g.ico", UriKind.Relative);
            bi3.EndInit();
            this.Icon = bi3;

            this.Content = _wc.Global;

            this.Closing += (s, e) => {
                Userdata.UserdataType.Window.Size = new Size(this.Width, this.Height);

                Userdata.SaveData();
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
