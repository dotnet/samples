Imports System.Windows.Threading

Public Class PrimeNumber
    ' Current number to check
    Private _num As Long = 3
    Private _runCalculation As Boolean = False

    Private Sub StartStopButton_Click(sender As Object, e As RoutedEventArgs)
        _runCalculation = Not _runCalculation

        If _runCalculation Then
            StartStopButton.Content = "Stop"
            StartStopButton.Dispatcher.InvokeAsync(AddressOf CheckNextNumber, DispatcherPriority.SystemIdle)
        Else
            StartStopButton.Content = "Resume"
        End If

    End Sub

    Public Sub CheckNextNumber()
        ' Reset flag.
        _isPrime = True

        For i As Long = 3 To Math.Sqrt(_num)
            If (_num Mod i = 0) Then

                ' Set Not a prime flag to true.
                _isPrime = False
                Exit For
            End If
        Next

        ' If a prime number, update the UI text
        If _isPrime Then
            bigPrime.Text = _num.ToString()
        End If

        _num += 2

        ' Requeue this method on the dispatcher
        If (_runCalculation) Then
            StartStopButton.Dispatcher.InvokeAsync(AddressOf CheckNextNumber, DispatcherPriority.SystemIdle)
        End If
    End Sub

    Private _isPrime As Boolean
End Class
