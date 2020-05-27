Public Class Form1

    Private ReadOnly _animation As Animation

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        Dim progress As New frmProgress(200, "Loading...")
        progress.Show()

        DataGridView1.SuspendLayout()
        For x As Integer = 0 To 99
            DataGridView1.Columns.Add("", "")
            DataGridView1.Columns(x).Width = 4
            progress.performStep()
        Next

        DataGridView1.Rows.Add(100)

        For x As Integer = 0 To 99
            DataGridView1.Rows(x).Height = 4
            progress.performStep()
        Next

        DataGridView1.ResumeLayout()

        _animation = New Animation(DataGridView1)

        Me.Text = $"Conway's Game of Life -- {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription} ({System.Runtime.InteropServices.RuntimeInformation.OSArchitecture})"

    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        ComboBox1.SelectedIndex = 0
        DataGridView1.CurrentCell = Nothing
        DataGridView1.ShowCellToolTips = False
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        _animation.setSeed(ComboBox1.SelectedIndex)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim backgroundThread As New Threading.Thread(AddressOf _animation.animate)
        backgroundThread.Start(ComboBox1.SelectedIndex)
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        _animation.Cancelled = True
    End Sub
End Class
