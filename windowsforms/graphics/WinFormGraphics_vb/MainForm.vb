Option Explicit Off
Option Infer On
Option Strict Off
' *********************************** Module Header **************************************\
'  Module Name:  MainForm.vb
'  Project:      WinFormGraphics
'  Copyright (c) Microsoft Corporation.
'
'  The Graphics sample demonstrates the fundamentals of GDI+ programming.
'  GDI+ allows you to create graphics, draw text, and manipulate graphical images as objects.
'  GDI+ is designed to offer performance as well as ease of use.
'  You can use GDI+ to render graphical images on Windows Forms and controls.
'  GDI+ has fully replaced GDI, and is now the only way to render graphics programmatically
'  in Windows Forms applications.
'
'  In this sample, there're 5 examples:
'
'  1. Draw A Line.
'     Demonstrates how to draw a solid/dash/dot line.
'  2. Draw A Curve.
'     Demonstrates how to draw a curve, and the difference between antialiasing rendering mode
'     and no antialiasing rendering mode.
'  3. Draw An Arrow.
'     Demonstrates how to draw an arrow.
'  4. Draw A Vertical String.
'     Demonstrates how to draw a vertical string.
'  5. Draw A Ellipse With Gradient Brush.
'     Demonstrates how to draw a shape with gradient effect.
'
'  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
'  EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
'  WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'  **************************************************************************

#Region "Using directives"
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Drawing.Drawing2D
#End Region


Namespace WinFormGraphics
    Public Partial Class MainForm
        Inherits Form
        Public Sub New()
            Me.InitializeComponent()
        End Sub

        Private Sub groupBox1_Paint(sender As Object, e As PaintEventArgs)
            Using p As New Pen(Color.Black)
#Region "Example 1 -- Draw A Line"

                ' Draw a solid line starts at point(40,90) and ends at point(240,90).
                e.Graphics.DrawLine(p, 40, 90, 240, 90)

                ' Draw a dash line starts at point(40,110) and ends at point(240,110).
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
                e.Graphics.DrawLine(p, 40, 110, 240, 110)

                ' Draw a dot line starts at point(40,130) and ends at point(240,130).
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot
                e.Graphics.DrawLine(p, 40, 130, 240, 130)

                ' Restore the pen dash style for the next example.
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid

#End Region

#Region "Example 2 -- Draw A Curve"

                ' Draw a curve with default rendering mode. (No antialiasing.)

                ' Specify a collection of points for the curve.
                Dim ps As Point() = New Point() { _
New Point(40, 250),
New Point(80, 300),
New Point(120, 200)}

                e.Graphics.DrawCurve(p, ps)

                ' Specify a collection of points for the curve.
                Dim ps2 As Point() = New Point() {
New Point(150, 250),
New Point(190, 300),
New Point(230, 200)}

                ' Draw a curve with antialiasing rendering mode.
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias

                e.Graphics.DrawCurve(p, ps2)

                ' Restore the Graphics object for the next example.
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None

#End Region

#Region "Example 3 -- Draw An Arrow"

                ' To draw an arrow, set the EndCap property to LineCap.ArrowAnchor for the pen.
                p.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor
                p.Width = 5
                e.Graphics.DrawLine(p, 40, 400, 240, 400)

                ' Restore the pen for the next example.
                p.EndCap = System.Drawing.Drawing2D.LineCap.NoAnchor
                Using br As New SolidBrush(Color.Red)
                    Using sf As New StringFormat With {
                        .FormatFlags = StringFormatFlags.DirectionVertical
                    }

                        e.Graphics.DrawString(
                            "This is a vertical text.",
                            Me.Font, br, 450, 90, sf)
                    End Using
                End Using

#End Region

#Region "Example 5 -- Draw A Ellipse With Gradient Brush"

                ' Specify a bound for the ellipse.
                Dim r As New Rectangle(350, 280, 280, 150)
                Using br As New LinearGradientBrush( _
                                        r, Color.Silver,
                                        Color.Black,
                                        LinearGradientMode.Vertical)
                    e.Graphics.FillEllipse(br, r)
                End Using

#End Region
            End Using
        End Sub
    End Class
End Namespace
