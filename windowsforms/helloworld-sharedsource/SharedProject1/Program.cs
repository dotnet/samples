using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsForm;

namespace WindowsFormsApp
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
            Application.SetCompatibleTextRenderingDefault(false);
            var form1 = new Form1();
            UpdateForm(form1);
            Application.Run(form1);
        }

        static void UpdateForm(Form1 form)
        {
            var dict = new Dictionary<string, string>();
#if NETCOREAPP3_0
            dict.Add("label1", "Hello .NET Core");
#else
            dict.Add("label1", "Hello .NET Framework");
#endif

            form.UpdateLabels(dict);
        }
    }
}
