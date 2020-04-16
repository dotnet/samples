using System;
using System.Reflection;
using Visio = Microsoft.Office.Interop.Visio;

namespace VisioDemo
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                // Start Visio and get Application object.
                var vApp = new Visio.Application();
                vApp.Visible = true;

                // Get a new document, based on a template.
                var vDoc = vApp.Documents.Add("Basic Flowchart.vst");
                var stencilDoc = vApp.Documents["Basic Flowchart Shapes.vss"];

                var startMst = stencilDoc.Masters.ItemU["Start/End"];
                var processMst = stencilDoc.Masters.ItemU["Process"];
                var decisionMst = stencilDoc.Masters.ItemU["Decision"];

                var vPag = vApp.ActivePage;

                //Drop initial shape using X/Y Drop method
                var startShp = vPag.Drop(startMst, 1, 4);
                startShp.Text = "A";

                //Drop subsequent shapes using DropConnected by way of a simple alternative
                var nextShp = vPag.DropConnected(processMst, startShp, Visio.VisAutoConnectDir.visAutoConnectDirRight);
                nextShp.Text = "B";

                nextShp = vPag.DropConnected(decisionMst, nextShp, Visio.VisAutoConnectDir.visAutoConnectDirRight);
                nextShp.Text = "C";

                var lowerShp = vPag.DropConnected(processMst, nextShp, Visio.VisAutoConnectDir.visAutoConnectDirDown);
                lowerShp.Text = "D";

                nextShp = vPag.DropConnected(processMst, nextShp, Visio.VisAutoConnectDir.visAutoConnectDirRight);
                nextShp.Text = "E";

                var endShp = vPag.DropConnected(processMst, nextShp, Visio.VisAutoConnectDir.visAutoConnectDirRight);
                endShp.Text = "F";

                //Set 'Parallel' theme
                vPag.SetTheme(40);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message} Line: {e.Source}");
            }
        }
    }
}
