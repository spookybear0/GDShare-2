using System;
using System.Windows;
using System.Windows.Controls;

namespace gdtools_cpp {
    namespace Pages {
        public class Page : Grid {
            public Page(GDTWindow _w) {
                this.Margin = Theme.Const.Page.Padding;
            }

            public virtual void Load() {}
        }
        

        public class None : Page {
            public None(GDTWindow _w) : base(_w) {
                this.Children.Add(new Elem.Centered(new UIElement[] {
                    new Elem.Header("Page not found!"),
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