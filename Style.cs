using System;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

/*
private void LoadThemes() {
            List<dynamic> _t = new List<dynamic> ();
            foreach (string f in Directory.GetFiles("resources/themes"))
                if (f.EndsWith(Ext.Theme))
                    _t.Add(new {
                        Name = File.ReadAllLines(f)[0].Split("=")[1],
                        NoClose = true,
                        Click = new EventHandler((s, e) => {
                            IDictionary<string, Object> _style = new ExpandoObject() as IDictionary<string, Object>;
                            foreach (string t in File.ReadAllLines(Path.GetFullPath(f)))
                                if (!t.Contains("="))
                                    _style.Add(t.Substring(0, t.IndexOf(" ")), $"#{Regex.Replace(t, @"\s+", " ").Substring(t.IndexOf(" ") + 1)}");
                            Style.LoadStyle(_style, this);
                            Dat.SaveToUserData("theme", File.ReadAllLines(f)[0].Split("=")[1]);
                        })
                    });
            Themes = _t.ToArray<dynamic>();
        }

        public void LoadTheme(string _name) {
            foreach (dynamic t in Themes)
                if (t.Name == _name)
                    t.Click(this, new EventArgs());
        }
*/

namespace gdtools_cpp {
    public class StyleObj {
        public object this[string propertyName] {
            get {
                PropertyInfo property = GetType().GetProperty(propertyName);
                return property.GetValue(this, null);
            }
            set {
                PropertyInfo property = GetType().GetProperty(propertyName);
                property.SetValue(this,value, null);
            }
        }

        public Dictionary<string, object> Colors = new Dictionary<string, object>();
        public Dictionary<string, object> ColorArrays = new Dictionary<string, object>();
        public string Name, Author, Description;

        public StyleObj(string[] _data) {
            string ParsingArray = null;
            List<SolidColorBrush> Arr = null;

            foreach(string l in _data)
                if (!l.StartsWith("::") && !String.IsNullOrWhiteSpace(l) && !l.Trim().StartsWith("!"))
                    if (l.Contains('=')) {
                        string key = l.Substring(0, l.IndexOf('=')).Trim(), val = l.Substring(l.IndexOf('=') + 1).Trim();

                        switch (key) {
                            case "Name": case "Title": this.Name = val; break;
                            case "Author": case "Auth": this.Author = val; break;
                            case "Description": case "Desc": this.Description = val; break;
                        }
                    } else if (l.Trim().StartsWith("'")) {
                        if (l.Trim() == "'") {
                            ColorArrays[ParsingArray] = Arr;
                            ParsingArray = null;
                            Arr = null;
                        } else {
                            ParsingArray = l.Substring(1);
                            Arr = new List<SolidColorBrush> ();
                        }
                    } else {
                        if (ParsingArray != null) {
                            string col = l.Trim();
                            if (!col.StartsWith('#')) col = $"#{col}";
                            Arr.Add(new SolidColorBrush((Color)ColorConverter.ConvertFromString(col)));
                            continue;
                        }

                        string key = l.Trim().Substring(0,  l.IndexOf(' ')).Trim(),
                            val = l.Trim().Substring(l.LastIndexOf(' ')).Trim();
                        if (!val.StartsWith('#')) val = $"#{val}";
                        if (val.Contains('>') && !val.Contains(">#")) val = val.Replace(">", ">#");
                        this.Colors[key] = val;
                    }
        }
    }

    public static class Theme {
        public static dynamic Colors = new ExpandoObject();
        public static dynamic ColorArrays = new ExpandoObject();
        public static List<StyleObj> Styles = new List<StyleObj> ();
        public static List<string> LoadingMessage = new List<string> ();
        public static string ResourceFolder = "resources";

        public static class Const {
            public static class Titlebar {
                public static uint Height = 30;
            }
            public static class Browser {
                public static uint Width = 180;
                public static uint TabHeight = 30;
                public static Thickness TextPadding = new Thickness(15, 0, 0, 0);
                public static uint IconPadding = 15;
            }
            public static class Icon {
                public static uint Thickness = 50;
            }
            public static class Page {
                public static Thickness Padding = new Thickness(30);
                public static int Newline = 30;
            }
            public static class Text {
                public static int Size = 14;
                public static int Large = 20;
                public static int HeaderSize = 35;
                public static int Padding = 15;
            }
            public static class Checkbox {
                public static int Size = (int)(Text.Size * 1.5);
                public static CornerRadius CornerRadius = new CornerRadius(BorderSize);
                public static uint Margin = 10;
                public static double TickRatio = 1.75;
            }
            public static class Select {
                public static Thickness Padding = new Thickness(20, 0, 20, 20);
                public static CornerRadius Corner = new CornerRadius(15);
                public static uint DefaultItemShow = 5;
                public static uint Width = 450;
                public static uint TitleHeight = 50;

                public static class Option {
                    public static uint Height = 35;
                    public static Thickness Padding = new Thickness(0);
                    public static CornerRadius Corner = new CornerRadius(8);
                }
            }
            public static class Input {
                public static uint Height = 50;
                public static Thickness Padding = new Thickness(10);
                public static CornerRadius Corner = new CornerRadius(8);
            }
            public static class Button {
                public static Thickness Padding = new Thickness(15, 8, 15, 8);
            }
            public static class ProgressBar {
                public static uint Height = 45;
                public static Thickness Padding = new Thickness(20);
            }
            public static uint BorderSize = 5;
            public static Thickness Margin = new Thickness(0, 0, 0, 10);
        }

        public static StyleObj ParseStyle(string _f) {
            string[] lines = System.IO.File.ReadAllLines(_f);

            if (lines[0] != GetGDTFileHead("Style"))
                return null;

            StyleObj s = new StyleObj(lines);
            Styles.Add(s);
            return s;
        }

        public static void LoadStyle(StyleObj _style, GDTWindow _rel = null) {
            Console.WriteLine($"Loading theme {_style.Name} by {_style.Author}...");

            Color ParseColor(string _c) {
                if (_c.Contains(",")) {
                    Color c = (Color)ColorConverter.ConvertFromString(_c.Split(",")[0]);
                    c.A = (Byte)Int16.Parse(_c.Split(",")[1]);
                    return c;
                } else
                    return (Color)ColorConverter.ConvertFromString(_c);
            };
            
            foreach (KeyValuePair<string, object> key in _style.ColorArrays)
                ((IDictionary<String, Object>)ColorArrays)[key.Key] = key.Value;

            foreach (KeyValuePair<string, object> key in _style.Colors)
                if (key.Value.ToString().Contains(">"))
                    ((IDictionary<String, Object>)Colors)[key.Key]
                        = new LinearGradientBrush(
                            ParseColor(key.Value.ToString().Split('>')[0]),
                            ParseColor(key.Value.ToString().Split('>')[1]),
                            new Point(0,0),
                            new Point(1,1)
                        );
                else
                    ((IDictionary<String, Object>)Colors)[key.Key]
                        = new SolidColorBrush(ParseColor(key.Value.ToString()));

            if (_rel != null)
                _rel.Refresh();
            
            /*
            PrivateFontCollection fc = new PrivateFontCollection();
            foreach (string file in Directory.GetFiles(@"resources/fonts"))
                if (file.EndsWith(".ttf")) {
                    fc.AddFontFile(Path.GetFullPath(file));
                    string name = Path.GetFileNameWithoutExtension(file);
                    ((IDictionary<String, Object>)Fonts)[name.Substring(0, name.IndexOf("_"))] = new FontFamily(name.Substring(name.IndexOf("_") + 1), fc);
                }*/
        }

        public static void Init() {
            Console.WriteLine("Loading app data...");

            LoadStyle(ParseStyle($"DefaultStyle.{Settings.Ext.Data}"));
            
            bool LoadedMsgs = true;
            if (System.IO.File.Exists($"{ResourceFolder}/Loading.gdt")) {
                string[] ss = System.IO.File.ReadAllLines($"{ResourceFolder}/Loading.gdt");
                if (ss[0] == GetGDTFileHead("Loading"))
                    foreach (string s in ss.Where(s => s != GetGDTFileHead("Loading") && !String.IsNullOrWhiteSpace(s)))
                        LoadingMessage.Add($"{s.Trim()}...");
                else LoadedMsgs = false;
            } else LoadedMsgs = false;

            if (!LoadedMsgs)
                LoadingMessage.Add("Loading...");
        }

        public static void UpdateCanvasColor(Canvas _c, Brush _col) {
            Brush b = new SolidColorBrush(System.Windows.Media.Colors.Transparent);
            foreach (FrameworkElement Framework_Element in _c.Children) {
                if (Framework_Element is Shape) {
                    if (((Shape)Framework_Element).Stroke != null)
                        ((Shape)Framework_Element).Stroke = _col;
                    if (((Shape)Framework_Element).Fill != null)
                        ((Shape)Framework_Element).Fill = _col;
                }
            }
        }

        public static Canvas LoadIcon(string _name, Brush _col = null, uint _thick = 0) {
            string[] lines = System.IO.File.ReadAllLines($"{ResourceFolder}\\Icons\\Icon.{_name}.{Settings.Ext.Data}");

            if (lines[0] != GetGDTFileHead("Icon"))
                lines = System.IO.File.ReadAllLines($"{ResourceFolder}\\Icons\\Icon.Error.{Settings.Ext.Data}");

            Brush col = _col == null ? Theme.Colors.Text : _col;
            uint thick = _thick == 0 ? Theme.Const.Icon.Thickness : _thick;

            Canvas c = new Canvas();

            foreach (string l in lines) {
                if (!String.IsNullOrWhiteSpace(l) && !l.Trim().StartsWith("!"))
                    if (l.StartsWith("."))
                        switch (l.Substring(1, l.IndexOf(" ") - 1)) {
                            case "Size":
                                c.Width = Int16.Parse(l.Substring(l.IndexOf(" ")).Split(",")[0]);
                                c.Height = Int16.Parse(l.Substring(l.IndexOf(" ")).Split(",")[1]);
                                break;
                            case "Stroke":
                                if (_thick == 0)
                                    thick = (uint)Int16.Parse(l.Substring(l.IndexOf(" ")));
                                break;
                        }
                    else {
                        string Key = l.Substring(0, l.IndexOf(" "));
                        string Points = l.Substring(l.IndexOf(" ") + 1).Trim();

                        string Tags = "";
                        if (Key.Contains('<')) {
                            Tags = Key.Substring(Key.IndexOf('<')).ToLower();
                            Key = Key.Substring(0, Key.IndexOf('<'));
                        }

                        switch (Key.ToLower().Trim()) {
                            case "polyline": {
                                Polyline line = new Polyline();

                                PointCollection points = new PointCollection();

                                foreach (string p in Points.Split(":"))
                                    points.Add(new Point(Int16.Parse(p.Split(",")[0]), Int16.Parse(p.Split(",")[1])));

                                line.Points = points;

                                line.Stroke = col;
                                line.StrokeThickness = thick;
                                line.StrokeStartLineCap = PenLineCap.Round;
                                line.StrokeEndLineCap = PenLineCap.Round;

                                if (Tags.Contains("fill"))
                                    line.Fill = col;

                                c.Children.Add(line);
                            } break;
                            case "arc": {
                                Path path = new Path();

                                PointCollection points = new PointCollection();

                                foreach (string p in Points.Split(":"))
                                    points.Add(new Point(Int16.Parse(p.Split(",")[0]), Int16.Parse(p.Split(",")[1])));

                                BezierSegment arc = new BezierSegment(points[0], points[1], points[2], true);

                                PathGeometry pg = new PathGeometry();
                                PathFigure f = new PathFigure(points[0], new PathSegment[] { arc }, false);

                                pg.Figures = new PathFigureCollection(new PathFigure[] { f });

                                path.Stroke = col;
                                path.StrokeThickness = thick;
                                path.StrokeStartLineCap = PenLineCap.Round;
                                path.StrokeEndLineCap = PenLineCap.Round;

                                path.Data = pg;

                                c.Children.Add(path);
                            } break;
                            case "circle": {
                                Ellipse e = new Ellipse();

                                string p = Points.Split(";")[0].Trim();
                                string z = Points.Split(";")[1].Trim();

                                e.Width = Int16.Parse(z.Split(":")[0]);
                                e.Height = Int16.Parse(z.Split(":")[1]);

                                e.Margin = new Thickness(
                                    Int16.Parse(p.Split(',')[0]) - e.Width / 2,
                                    Int16.Parse(p.Split(',')[1]) - e.Height / 2,
                                    0, 0
                                );

                                e.StrokeThickness = thick;

                                if (Tags.Contains("fill"))
                                    e.Fill = col;
                                else e.Stroke = col;
                                
                                c.Children.Add(e);
                            } break;
                        }
                    }
            }

            return c;
        }
    
        public static string GetLoadingMessage() {
            return LoadingMessage[new Random().Next(LoadingMessage.Count)];
        }
    
        public static string GetGDTFileHead(string _gdt) {
            return $"::GDTools {_gdt}::";
        }
    }
}