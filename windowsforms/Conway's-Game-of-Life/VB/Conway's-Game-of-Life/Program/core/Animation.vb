Imports System.Threading

Public Class Animation

    Private ReadOnly _gridControl As ExDGV
    Private ReadOnly _cellValues(99)() As Integer
    Public Cancelled As Boolean = False

    Public Sub New(dgv As ExDGV)
        _gridControl = dgv
        For r As Integer = 0 To 99
            ReDim _cellValues(r)(99)
        Next
    End Sub

    Private Sub clear()
        For r As Integer = 0 To 99
            For c As Integer = 0 To 99
                _cellValues(r)(c) = 0
            Next
        Next
    End Sub

    Private Sub repaint()
        If Cancelled Then Return
        For r As Integer = 0 To 99
            For c As Integer = 0 To 99
                _gridControl.Rows(r).Cells(c).Style.BackColor =
                    If(_cellValues(r)(c) > 0, Color.Black, Color.White)
            Next
        Next
    End Sub

    Public Sub setSeed(x As Integer)
        Cancelled = True
        clear()

        Select Case x
            Case 1 'diamond
                For r As Integer = 47 To 48
                    For c As Integer = 49 To 50
                        _cellValues(r)(c) = 1
                    Next c
                Next r
                For r As Integer = 49 To 50
                    For c As Integer = 49 To 50
                        _cellValues(r)(c) = -1
                    Next c
                Next r
                For r As Integer = 49 To 50
                    For c As Integer = 51 To 52
                        _cellValues(r)(c) = 1
                    Next c
                Next r
                For r As Integer = 51 To 52
                    For c As Integer = 49 To 50
                        _cellValues(r)(c) = 1
                    Next c
                Next r
                For r As Integer = 49 To 50
                    For c As Integer = 47 To 48
                        _cellValues(r)(c) = 1
                    Next c
                Next r
            Case 2 'square
                For r As Integer = 48 To 51
                    For c As Integer = 48 To 51
                        _cellValues(r)(c) = 1
                    Next c
                Next r
            Case 3 'cross
                For r As Integer = 47 To 48
                    For c As Integer = 47 To 48
                        _cellValues(r)(c) = 1
                    Next c
                Next r
                For r As Integer = 47 To 48
                    For c As Integer = 51 To 52
                        _cellValues(r)(c) = 1
                    Next c
                Next r
                For r As Integer = 49 To 50
                    For c As Integer = 49 To 50
                        _cellValues(r)(c) = 1
                    Next c
                Next r
                For r As Integer = 51 To 52
                    For c As Integer = 47 To 48
                        _cellValues(r)(c) = 1
                    Next c
                Next r
                For r As Integer = 51 To 52
                    For c As Integer = 51 To 52
                        _cellValues(r)(c) = 1
                    Next c
                Next r
        End Select
        Cancelled = False
        repaint()
        Cancelled = True

    End Sub

    Public Sub animate(index As Object)
        Cancelled = False
        Dim l As Integer = 0
        Dim t As Integer = 0
        Dim r As Integer = 0
        Dim b As Integer = 0
        Select Case DirectCast(index, Integer)
            Case 1, 3
                t = 47
                b = 52
                l = 47
                r = 52
            Case 2
                t = 48
                b = 51
                l = 48
                r = 51
        End Select
        Dim g As Integer = 2
        Do While Not Cancelled
            For y As Integer = t To b
                For x As Integer = l To r
                    If Cancelled Then
                        Return
                    End If
                    If _cellValues(y)(x) > 0 AndAlso _cellValues(y)(x) < g Then
                        grow(g, y, x)
                    End If
                Next x
            Next y
            'Pause for 50 milliseconds
            Thread.Sleep(50)

            For y As Integer = t To b
                For x As Integer = l To r
                    If Cancelled Then
                        Return
                    End If
                    If _cellValues(y)(x) > 0 AndAlso _cellValues(y)(x) < g Then
                        Dim count As Integer = surrounding(y, x)
                        If count < 2 Then
                            _cellValues(y)(x) = -1
                        ElseIf count > 3 Then
                            _cellValues(y)(x) = -1
                        End If
                    End If
                Next x
            Next y
            'Pause for 50 milliseconds
            Thread.Sleep(50)
            For y As Integer = t To b
                For x As Integer = l To r
                    If Cancelled Then
                        Return
                    End If
                    If _cellValues(y)(x) = -1 Then
                        Dim count As Integer = surrounding(y, x)
                        If count = 3 Then
                            _cellValues(y)(x) = 1
                        End If
                    End If
                Next x
            Next y
            'Pause for 50 milliseconds
            Thread.Sleep(50)
            t -= 1
            l -= 1
            b += 1
            r += 1
            g += 1
            If t < 0 OrElse l < 0 Then
                Cancelled = True
            End If
            repaint()
        Loop
    End Sub

    Private Sub grow(g As Integer, r As Integer, c As Integer)
        If r > 0 Then
            If _cellValues(r - 1)(c) = 0 Then
                _cellValues(r - 1)(c) = g
            End If
            If c > 0 Then
                If _cellValues(r - 1)(c - 1) = 0 Then
                    _cellValues(r - 1)(c - 1) = g
                End If
            End If
            If c < 99 Then
                If _cellValues(r - 1)(c + 1) = 0 Then
                    _cellValues(r - 1)(c + 1) = g
                End If
            End If
        End If
        If c > 0 Then
            If _cellValues(r)(c - 1) = 0 Then
                _cellValues(r)(c - 1) = g
            End If
        End If
        If c < 99 Then
            If _cellValues(r)(c + 1) = 0 Then
                _cellValues(r)(c + 1) = g
            End If
        End If
        If r < 99 Then
            If _cellValues(r + 1)(c) = 0 Then
                _cellValues(r + 1)(c) = g
            End If
            If c > 0 Then
                If _cellValues(r + 1)(c - 1) = 0 Then
                    _cellValues(r + 1)(c - 1) = g
                End If
            End If
            If c < 99 Then
                If _cellValues(r + 1)(c + 1) = 0 Then
                    _cellValues(r + 1)(c + 1) = g
                End If
            End If
        End If
    End Sub

    Private Function surrounding(r As Integer, c As Integer) As Integer
        Dim count As Integer = 0
        If r > 0 Then
            If _cellValues(r - 1)(c) > 0 Then
                count += 1
            End If
            If c > 0 Then
                If _cellValues(r - 1)(c - 1) > 0 Then
                    count += 1
                End If
            End If
            If c < 99 Then
                If _cellValues(r - 1)(c + 1) > 0 Then
                    count += 1
                End If
            End If
        End If
        If c > 0 Then
            If _cellValues(r)(c - 1) > 0 Then
                count += 1
            End If
        End If
        If c < 99 Then
            If _cellValues(r)(c + 1) > 0 Then
                count += 1
            End If
        End If
        If r < 99 Then
            If _cellValues(r + 1)(c) > 0 Then
                count += 1
            End If
            If c > 0 Then
                If _cellValues(r + 1)(c - 1) > 0 Then
                    count += 1
                End If
            End If
            If c < 99 Then
                If _cellValues(r + 1)(c + 1) > 0 Then
                    count += 1
                End If
            End If
        End If

        Return count
    End Function

End Class
