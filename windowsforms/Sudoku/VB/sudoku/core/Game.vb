Public Class Game

    Public Event ShowClues(grid()() As Integer)
    Public Event ShowSolution(grid()() As Integer)

    Private ReadOnly _HRow(8) As List(Of Integer)
    Private ReadOnly _VRow(8) As List(Of Integer)
    Private ReadOnly _ThreeSquare(8) As List(Of Integer)
    Private ReadOnly _Grid(8)() As Integer

    Private _R As Random

    Public Sub NewGame(rn As Random)
        _R = rn
        createNewGame()
    End Sub

    Private Sub initializeLists()
        For x As Integer = 0 To 8
            _HRow(x) = New List(Of Integer)(New Integer() {1, 2, 3, 4, 5, 6, 7, 8, 9})
            _VRow(x) = New List(Of Integer)(New Integer() {1, 2, 3, 4, 5, 6, 7, 8, 9})
            _ThreeSquare(x) = New List(Of Integer)(New Integer() {1, 2, 3, 4, 5, 6, 7, 8, 9})
            Dim row(8) As Integer
            _Grid(x) = row
        Next
    End Sub

    Private Sub createNewGame()
        Do
            initializeLists()
            For y As Integer = 0 To 8
                For x As Integer = 0 To 8
                    _Grid(y)(x) = Nothing
                    Dim si As Integer = (y \ 3) * 3 + (x \ 3)
                    Dim useful() As Integer = _HRow(y).Intersect(_VRow(x)).Intersect(_ThreeSquare(si)).ToArray
                    If useful.Any Then
                        Dim randomNumber As Integer = useful(_R.Next(0, useful.Count))
                        _HRow(y).Remove(randomNumber)
                        _VRow(x).Remove(randomNumber)
                        _ThreeSquare(si).Remove(randomNumber)
                        _Grid(y)(x) = randomNumber
                        If y = 8 AndAlso x = 8 Then Exit Do
                    End If
                Next
            Next
        Loop

        RaiseEvent ShowClues(_Grid)

    End Sub

    Public Sub showGridSolution()

        RaiseEvent ShowSolution(_Grid)

    End Sub

End Class
