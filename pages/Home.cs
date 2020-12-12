using System;
using System.Windows.Controls;

namespace gdtools_cpp {
    namespace Pages {
        public class Home : Page {
            public Home(GDTWindow _w) : base(_w) {
                this.Children.Add(new Elem.Text("swag home page"));
            }
        }
    }
}