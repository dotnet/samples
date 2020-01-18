using System;
using System.Windows.Forms;

namespace CoreWinFormsApp1
{
    class Program
    {
        [STAThread]
        static void Main()

        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WindowsFormsApp1.Form1());
        }
    }
}
