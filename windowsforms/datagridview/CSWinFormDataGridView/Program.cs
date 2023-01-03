using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CSWinFormDataGridView
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new CustomDataGridViewColumn.MainForm());
            //Application.Run(new DataGridViewPaging.MainForm());
            //Application.Run(new EditingControlHosting.MainForm());
            //Application.Run(new JustInTimeDataLoading.MainForm());
            //Application.Run(new MultipleLayeredColumnHeader.MainForm());
        }
    }
}
