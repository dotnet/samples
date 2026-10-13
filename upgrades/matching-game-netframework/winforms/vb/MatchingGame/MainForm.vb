Imports MatchingGame.Logic
Public Class MainForm

    Private _game As Game

    Private Sub StartNewGame()

        _game = Game.Create()

        For i = 0 To tableLayoutPanel1.Controls.Count - 1
            Dim label = DirectCast(tableLayoutPanel1.Controls(i), Label)
            Dim column = tableLayoutPanel1.GetColumn(label)
            Dim row = tableLayoutPanel1.GetRow(label)

            label.Text = _game.GetCard(column, row).ToString()
            label.ForeColor = label.BackColor
        Next

        UpdateCards()

    End Sub

    Private Sub UpdateCards()
        For w = 0 To _game.Width - 1
            For h = 0 To _game.Height - 1
                Dim label = DirectCast(tableLayoutPanel1.GetControlFromPosition(w, h), Label)
                label.ForeColor = IIf(_game.IsOpen(w, h), Color.Black, label.BackColor)
            Next
        Next
    End Sub

    Private Sub label_Click(sender As Object, e As EventArgs) Handles label16.Click, label15.Click, label9.Click, label8.Click, label7.Click, label6.Click, label5.Click, label4.Click, label3.Click, label2.Click, label14.Click, label13.Click, label12.Click, label11.Click, label10.Click, label1.Click
        If closeCardTimer.Enabled Then Return

        If TypeOf sender Is Label Then

            Dim label = DirectCast(sender, Label)

            Dim column = tableLayoutPanel1.GetColumn(label)
            Dim row = tableLayoutPanel1.GetRow(label)

            If _game.IsOpen(column, row) Then Return

            _game.OpenCard(column, row)
            UpdateCards()

            If (_game.RemainingCardsInTurn > 0) Then Return

            CheckForWinner()

            If _game.CompleteTurn() Then Return

            closeCardTimer.Start()

        End If

    End Sub

    Private Sub CheckForWinner()

        If Not _game.IsComplete() Then Return

        Dim bestScore = GameSettings.Instance.BestScore
        Dim currentScore = _game.Turns
        GameSettings.Instance.UpdateScore(_game.Turns)

        Dim text As String

        If bestScore = 0 Then
            text = $"It took you {currentScore} turns to complete. Keep it up!"
        ElseIf bestScore < currentScore Then
            text = $"It took you {currentScore - bestScore} more turns than your previous best. Try harder!"
        Else
            text = $"You set a new best with only {currentScore} turns!"
        End If

        MessageBox.Show(text, "Congratulations!")
        StartNewGame()

    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        StartNewGame()
    End Sub

    Private Sub closeCardTimer_Tick(sender As Object, e As EventArgs) Handles closeCardTimer.Tick
        closeCardTimer.Stop()
        _game.CloseCards()
        UpdateCards()
    End Sub
End Class
