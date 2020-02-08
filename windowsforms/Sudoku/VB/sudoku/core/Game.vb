Public Class Game

    Public Event ShowClues(grid()() As Integer)
    Public Event ShowSolution(grid()() As Integer)

    Private ReadOnly _hRow(8) As List(Of Integer)
    Private ReadOnly _vRow(8) As List(Of Integer)
    Private ReadOnly _threeSquare(8) As List(Of Integer)
    Private ReadOnly _grid(8)() As Integer

    Private _r As Random

    Public Sub NewGame(rn As Random)
        _r = rn
        CreateNewGame()
    End Sub

    Private Sub InitializeLists()
        For x As Integer = 0 To 8
            _hRow(x) = New List(Of Integer)(New Integer() {1, 2, 3, 4, 5, 6, 7, 8, 9})
            _vRow(x) = New List(Of Integer)(New Integer() {1, 2, 3, 4, 5, 6, 7, 8, 9})
            _threeSquare(x) = New List(Of Integer)(New Integer() {1, 2, 3, 4, 5, 6, 7, 8, 9})
            Dim row(8) As Integer
            _grid(x) = row
        Next
    End Sub

    Private Sub CreateNewGame()
        Do
            InitializeLists()
            For y As Integer = 0 To 8
                For x As Integer = 0 To 8
                    _grid(y)(x) = Nothing
                    Dim si As Integer = (y \ 3) * 3 + (x \ 3)
                    Dim useful() As Integer = _hRow(y).Intersect(_vRow(x)).Intersect(_threeSquare(si)).ToArray
                    If useful.Any Then
                        Dim randomNumber As Integer = useful(_r.Next(0, useful.Count))
                        _hRow(y).Remove(randomNumber)
                        _vRow(x).Remove(randomNumber)
                        _threeSquare(si).Remove(randomNumber)
                        _grid(y)(x) = randomNumber
                        If y = 8 AndAlso x = 8 Then Exit Do
                    End If
                Next
            Next
        Loop

        RaiseEvent ShowClues(_grid)

    End Sub

    Public Sub ShowGridSolution()

        RaiseEvent ShowSolution(_grid)

    End Sub

End Class
