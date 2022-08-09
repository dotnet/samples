using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HelloWorld
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text += RuntimeInformation.FrameworkDescription;
        }
    }
}
