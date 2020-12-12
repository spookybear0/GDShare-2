using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace gdtools_cpp {
    namespace Elem {
        public enum Aligns {
            Left, Middle, Right
        }
        
        public class Align {
            public static HorizontalAlignment Get(Aligns _a) {
                switch (_a) {
                    case Aligns.Left:
                        return HorizontalAlignment.Left;
                    case Aligns.Right:
                        return HorizontalAlignment.Right;
                    case Aligns.Middle:
                        return HorizontalAlignment.Center;
                    default:
                        return HorizontalAlignment.Left;
                }
            }
        }
    }
}