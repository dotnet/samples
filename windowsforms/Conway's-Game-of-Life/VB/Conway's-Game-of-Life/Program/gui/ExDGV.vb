''' <summary>
''' Extended DataGridView
''' DoubleBuffered. Restricts user selection of cells to facilitate seamless highlighting line drawing.
''' </summary>
''' <remarks></remarks>
Public Class ExDGV
    Inherits DataGridView

    Private ReadOnly _wmLeftButtonDown As Integer = &H201
    Private ReadOnly _wmLeftButtonDoubleLeftClick As Integer = &H203
    Private ReadOnly _wmKeyDown As Integer = &H100

    Public Sub New()
        DoubleBuffered = True
    End Sub

    <DebuggerNonUserCode()>
    Protected Overrides Sub OnRowPrePaint(e As DataGridViewRowPrePaintEventArgs)
        e.PaintParts = e.PaintParts And Not DataGridViewPaintParts.Focus
        MyBase.OnRowPrePaint(e)
    End Sub

    <DebuggerNonUserCode()>
    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = _wmLeftButtonDoubleLeftClick OrElse m.Msg = _wmKeyDown OrElse m.Msg = _wmLeftButtonDown Then
            Return
        End If
        MyBase.WndProc(m)
    End Sub

End Class
