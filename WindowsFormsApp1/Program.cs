using System;
using System.Windows.Forms;

namespace MobileStoreApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Batteries.Init();  ← ЭТОЙ СТРОКИ НЕ ДОЛЖНО БЫТЬ!

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}