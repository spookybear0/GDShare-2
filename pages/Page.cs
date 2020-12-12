using System;
using System.Windows.Controls;

namespace gdtools_cpp {
    namespace Pages {
        public class Page : StackPanel {
            public Page(GDTWindow _w) {
                this.Margin = Theme.Const.Page.Padding;
            }
        }

        public class None : Page {
            public None(GDTWindow _w) : base(_w) {
                this.Children.Add(new Elem.Header("Page not found!"));
                this.Children.Add(new Elem.Newline());
                this.Children.Add(new Elem.Text("We'we vewy sowwy fow dis and wiww get it fixed asap."));
                this.Children.Add(new Elem.Text("Pwease dun get mad at meee...... rawr >w<"));
                this.Children.Add(new Elem.Newline());
                this.Children.Add(new Elem.Img("teehee.jpg", 250));
            }
        }
    }
}