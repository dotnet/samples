using System.Diagnostics;
using System.Windows.Forms;

namespace ComWrappersIDispatch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.webBrowser1.DocumentText =
$@"<button onClick=""dispatch()"">Send to .NET!</button>
<input type=""text"" id=""message"" value=""From the Web Browser!""></input>

<script>
    function dispatch() {{
        window.external.Func1();
        window.external.Func2(27);
        window.external.Func3(document.getElementById(""message"").value);
    }}
</script>";

            this.Controls.Add(this.webBrowser1);

            this.AttachObject(new ExposedObject(this));
        }

        private void AttachObject(object obj)
        {
            var proxy = new AnyObjectProxy(obj);
            this.webBrowser1.ObjectForScripting = proxy;
        }

        private class ExposedObject
        {
            private readonly Form1 form;
            public ExposedObject(Form1 form)
            {
                this.form = form;
            }
            public void Func1()
            {
                Debug.WriteLine($"{nameof(Func1)}");
            }
            public void Func2(int a)
            {
                Debug.WriteLine($"{nameof(Func2)}({a})");
            }
            public void Func3(string msg)
            {
                Debug.WriteLine($"{nameof(Func3)}({msg})");
                this.form.textBox1.Text = msg;
            }
        }
    }
}
