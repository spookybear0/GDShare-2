using System;
using System.Windows;
using System.Windows.Controls;

namespace gdtools_cpp {
    namespace Pages {
        public class Replay : Page {
            public Replay(GDTWindow _w) : base(_w) {
                this.Children.Add(new Elem.Centered(new UIElement[] {
                    new Elem.Header("Pae not found!"),
                    new Elem.Newline(),
                    new Elem.Text("We'we vewy sowwy fow dis and wiww get it fixed asap."),
                    new Elem.Text("Pwease dun get mad at meee...... rawr >w<"),
                    new Elem.Newline(),
                    new Elem.Img("teehee.jpg", 250)
                }));
            }
        }
    }
}