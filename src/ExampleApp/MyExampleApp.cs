using System;
using System.Windows.Forms;

namespace ExampleApp
{
    static class MyExampleApp
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
