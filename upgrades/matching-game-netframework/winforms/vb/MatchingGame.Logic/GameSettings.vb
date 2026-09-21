Imports Microsoft.Win32

Public NotInheritable Class GameSettings

    Private Const KeyPath As String = "Software\\AdventureWorks\\MatchingGame"
    Public Shared ReadOnly Instance As GameSettings = Load()

    Private _bestScore As Integer

    Private Sub New()

    End Sub

    Public Property BestScore As Integer
        Get
            Return _bestScore
        End Get
        Set
            _bestScore = Value
            Save()
        End Set
    End Property

    Public Sub UpdateScore(newScore As Integer)
        If BestScore = 0 Or newScore < BestScore Then
            BestScore = newScore
        End If
    End Sub

    Private Shared Function Load() As GameSettings

        Dim result = New GameSettings()

        Using key = My.Computer.Registry.CurrentUser.OpenSubKey(KeyPath, False)
            If key IsNot Nothing Then
                Dim value = key.GetValue(NameOf(BestScore))
                If value IsNot Nothing Then
                    result.BestScore = CInt(value)
                End If
            End If
        End Using

        Return result

    End Function

    Private Sub Save()
        Using key = My.Computer.Registry.CurrentUser.OpenSubKey(KeyPath, True)
            key.SetValue(NameOf(BestScore), BestScore)
        End Using
    End Sub

End Class
