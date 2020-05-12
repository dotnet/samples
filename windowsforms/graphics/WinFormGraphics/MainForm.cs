/************************************* Module Header **************************************\
* Module Name:  MainForm.cs
* Project:      WinFormGraphics
* Copyright (c) Microsoft Corporation.
*
* The Graphics sample demonstrates the fundamentals of GDI+ programming.
* GDI+ allows you to create graphics, draw text, and manipulate graphical images as objects.
* GDI+ is designed to offer performance as well as ease of use.
* You can use GDI+ to render graphical images on Windows Forms and controls.
* GDI+ has fully replaced GDI, and is now the only way to render graphics programmatically
* in Windows Forms applications.
*
* In this sample, there're 5 examples:
*
* 1. Draw A Line.
*    Demonstrates how to draw a solid/dash/dot line.
* 2. Draw A Curve.
*    Demonstrates how to draw a curve, and the difference between antialiasing rendering mode
*    and no antialiasing rendering mode.
* 3. Draw An Arrow.
*    Demonstrates how to draw an arrow.
* 4. Draw A Vertical String.
*    Demonstrates how to draw a vertical string.
* 5. Draw A Ellipse With Gradient Brush.
*    Demonstrates how to draw a shape with gradient effect.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
* ***************************************************************************/

#region Using directives
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
#endregion

namespace WinFormGraphics
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            using (var p = new Pen(Color.Black))
            {
                #region Example 1 -- Draw A Line

                // Draw a solid line starts at point(40,90) and ends at point(240,90).
                e.Graphics.DrawLine(p, 40, 90, 240, 90);

                // Draw a dash line starts at point(40,110) and ends at point(240,110).
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                e.Graphics.DrawLine(p, 40, 110, 240, 110);

                // Draw a dot line starts at point(40,130) and ends at point(240,130).
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                e.Graphics.DrawLine(p, 40, 130, 240, 130);

                // Restore the pen dash style for the next example.
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

                #endregion

                #region Example 2 -- Draw A Curve

                // Draw a curve with default rendering mode. (No antialiasing.)

                // Specify a collection of points for the curve.
                var ps = new Point[]{
                    new Point(40,250),
                    new Point(80,300),
                    new Point(120,200)};

                e.Graphics.DrawCurve(p, ps);

                // Specify a collection of points for the curve.
                var ps2 = new Point[]{
                    new Point(150,250),
                    new Point(190,300),
                    new Point(230,200)};

                // Draw a curve with antialiasing rendering mode.
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                e.Graphics.DrawCurve(p, ps2);

                // Restore the Graphics object for the next example.
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

                #endregion

                #region Example 3 -- Draw An Arrow

                // To draw an arrow, set the EndCap property to LineCap.ArrowAnchor for the pen.
                p.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                p.Width = 5;
                e.Graphics.DrawLine(p, 40, 400, 240, 400);

                // Restore the pen for the next example.
                p.EndCap = System.Drawing.Drawing2D.LineCap.NoAnchor;

                #endregion

                #region Example 4 -- Draw A Vertical String

                using (var br = new SolidBrush(Color.Red))
                {
                    var sf = new StringFormat();
                    sf.FormatFlags = StringFormatFlags.DirectionVertical;

                    e.Graphics.DrawString(
                        "This is a vertical text.",
                        this.Font, br, 450, 90, sf);
                }

                #endregion

                #region Example 5 -- Draw A Ellipse With Gradient Brush

                // Specify a bound for the ellipse.
                var r = new Rectangle(350, 280, 280, 150);

                // Use a LinearGradientBrush to draw the ellipse.
                using (var br = new LinearGradientBrush(
                        r, Color.Silver,
                        Color.Black,
                        LinearGradientMode.Vertical))
                {
                    e.Graphics.FillEllipse(br, r);
                }

                #endregion
            }
        }
    }
}
