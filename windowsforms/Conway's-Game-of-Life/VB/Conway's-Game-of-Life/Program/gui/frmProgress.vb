Public Class frmProgress

    Public Sub New(max As Integer, caption As String)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Text = caption
        ProgressBar1.Maximum = max
    End Sub

    Public Sub performStep()
        ProgressBar1.PerformStep()
        Label1.Text = ((ProgressBar1.Value / ProgressBar1.Maximum) * 100).ToString & "% complete"
        Refresh()
        Application.DoEvents()
        If ProgressBar1.Value = ProgressBar1.Maximum Then Close()
    End Sub

End Class
