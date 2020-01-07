Public Class Form1

    Private WithEvents Game As New Game
    Private ReadOnly _r As New Random

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DataGridView1.Rows.Add(9)
        ComboBox1.SelectedIndex = 0
        btnNew.PerformClick()
    End Sub

    Private Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click
        Game.NewGame(_r)
    End Sub

    Private Sub DataGridView1_Paint(sender As Object, e As PaintEventArgs) Handles DataGridView1.Paint
        e.Graphics.DrawLine(New Pen(Color.Black, 2), 75, 0, 75, 228)
        e.Graphics.DrawLine(New Pen(Color.Black, 2), 152, 0, 152, 228)
        e.Graphics.DrawLine(New Pen(Color.Black, 2), 0, 75, 228, 75)
        e.Graphics.DrawLine(New Pen(Color.Black, 2), 0, 153, 228, 153)
    End Sub

    Private Sub btnSolution_Click(sender As Object, e As EventArgs) Handles btnSolution.Click
        Game.ShowGridSolution()
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        btnNew.PerformClick()
    End Sub

    Public Sub game_ShowClues(grid()() As Integer) Handles Game.ShowClues
        For y As Integer = 0 To 8
            Dim cells As New List(Of Integer)(New Integer() {1, 2, 3, 4, 5, 6, 7, 8, 9})
            For c As Integer = 1 To 9 - (5 - ComboBox1.SelectedIndex)
                Dim randomNumber As Integer = cells(_r.Next(0, cells.Count))
                cells.Remove(randomNumber)
            Next
            For x As Integer = 0 To 8
                If cells.Contains(x + 1) Then
                    DataGridView1.Rows(y).Cells(x).Value = grid(y)(x)
                    DataGridView1.Rows(y).Cells(x).Style.ForeColor = Color.Red
                    DataGridView1.Rows(y).Cells(x).ReadOnly = True
                Else
                    DataGridView1.Rows(y).Cells(x).Value = ""
                    DataGridView1.Rows(y).Cells(x).Style.ForeColor = Color.Black
                    DataGridView1.Rows(y).Cells(x).ReadOnly = False
                End If
            Next
        Next
    End Sub

    Public Sub game_ShowSolution(grid()() As Integer) Handles Game.ShowSolution
        For y As Integer = 0 To 8
            For x As Integer = 0 To 8
                If DataGridView1.Rows(y).Cells(x).Style.ForeColor = Color.Black Then
                    If String.IsNullOrEmpty(DataGridView1.Rows(y).Cells(x).Value.ToString) Then
                        DataGridView1.Rows(y).Cells(x).Style.ForeColor = Color.Blue
                        DataGridView1.Rows(y).Cells(x).Value = grid(y)(x)
                    Else
                        If grid(y)(x).ToString <> DataGridView1.Rows(y).Cells(x).Value.ToString Then
                            DataGridView1.Rows(y).Cells(x).Style.ForeColor = Color.Blue
                            DataGridView1.Rows(y).Cells(x).Value = grid(y)(x)
                        End If
                    End If
                End If
            Next
        Next
    End Sub

End Class
