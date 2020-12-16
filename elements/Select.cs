using System;
using System.Windows.Controls;

namespace gdtools_cpp {
    namespace Elem {
        public class Select : ComboBox {
            public Select(string[] _opt = null) {
                if (_opt != null)
                    foreach (string opt in _opt) {
                        ComboBoxItem i = new ComboBoxItem();
                        i.Content = opt;
                        this.AddChild(i);
                    }
            }
        }
    }
}