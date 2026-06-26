Imports MatchingGame.Logic
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Threading

Class MainWindow

    Private _game As Game
    Private ReadOnly _closeCardTimer As DispatcherTimer
    Private Shared ReadOnly HiddenBrush As Brush = New SolidColorBrush(Colors.CornflowerBlue)
    Private Shared ReadOnly VisibleBrush As Brush = New SolidColorBrush(Colors.Black)

    Public Sub New()
        InitializeComponent()
        _closeCardTimer = New DispatcherTimer()
        _closeCardTimer.Interval = TimeSpan.FromMilliseconds(750)
        AddHandler _closeCardTimer.Tick, AddressOf CloseCardTimer_Tick
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        For i = 0 To 15
            Dim button As New Button()
            button.Style = CType(Resources("CardButtonStyle"), Style)
            AddHandler button.Click, AddressOf Card_Click
            cardGrid.Children.Add(button)
        Next

        StartNewGame()
    End Sub

    Private Sub StartNewGame()
        _game = Game.Create()

        For i = 0 To cardGrid.Children.Count - 1
            Dim button = CType(cardGrid.Children(i), Button)
            Dim col = i Mod _game.Width
            Dim row = i \ _game.Width
            button.Content = _game.GetCard(col, row).ToString()
            button.Foreground = HiddenBrush
        Next

        UpdateCards()
    End Sub

    Private Sub UpdateCards()
        For i = 0 To cardGrid.Children.Count - 1
            Dim button = CType(cardGrid.Children(i), Button)
            Dim col = i Mod _game.Width
            Dim row = i \ _game.Width
            button.Foreground = If(_game.IsOpen(col, row), VisibleBrush, HiddenBrush)
        Next
    End Sub

    Private Sub Card_Click(sender As Object, e As RoutedEventArgs)
        If _closeCardTimer.IsEnabled Then Return

        Dim button = TryCast(sender, Button)
        If button IsNot Nothing Then
            Dim index = cardGrid.Children.IndexOf(button)
            Dim col = index Mod _game.Width
            Dim row = index \ _game.Width

            If _game.IsOpen(col, row) Then Return

            _game.OpenCard(col, row)
            UpdateCards()

            If _game.RemainingCardsInTurn > 0 Then Return

            CheckForWinner()

            If _game.CompleteTurn() Then Return

            _closeCardTimer.Start()
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

    Private Sub CloseCardTimer_Tick(sender As Object, e As EventArgs)
        _closeCardTimer.Stop()
        _game.CloseCards()
        UpdateCards()
    End Sub

End Class
