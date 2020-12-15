using System;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace gdtools_cpp {
    namespace Pages {
        public class Home : Page {
            public Home(GDTWindow _w) : base(_w) {
                this.Children.Add(new Elem.Organized(new UIElement[] {
                    new Elem.Text("swag home page"),
                    new Elem.But("Load absolutely fucking nothing", default(Size), async (s, e) => {
                        Elem.ProgressBar pb = _w.Contents.ProgressBars.AddBar();

                        string msg = "";
                        for (int i = 0; i <= 100; i++) {
                            if (i % 10 == 0)
                                msg = Theme.GetLoadingMessage();
                            pb.SetProgress((ushort)i, msg);
                            if (new Random().Next(150) == 5) {
                                pb.Fail("Failure loading :(");
                                break;
                            }
                            await Task.Delay(50);
                        }

                        pb.Finish("Finished!");
                    })
                }));
            }
        }
    }
}