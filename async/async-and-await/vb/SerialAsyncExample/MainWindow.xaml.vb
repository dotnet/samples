Imports System.Net.Http

Public Class MainWindow
    Private ReadOnly _client As HttpClient = New HttpClient With {
        .MaxResponseContentBufferSize = 1_000_000
    }

    Private ReadOnly _urlList As IEnumerable(Of String) =
        New String() _
        {
            "https://docs.microsoft.com",
            "https://docs.microsoft.com/azure",
            "https://docs.microsoft.com/powershell",
            "https://docs.microsoft.com/dotnet",
            "https://docs.microsoft.com/aspnet/core",
            "https://docs.microsoft.com/windows"
        }

    Private Async Sub OnStartButtonClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        _startButton.IsEnabled = False
        _resultsTextBox.Clear()

        Await SumPageSizesAsync()

        _resultsTextBox.Text += $"{vbCrLf}Control returned to {NameOf(OnStartButtonClick)}."
        _startButton.IsEnabled = False
    End Sub

    Private Async Function SumPageSizesAsync() As Task
        Dim stopwatch As Stopwatch = Stopwatch.StartNew()

        Dim total As Integer = 0

        For Each url As String In _urlList
            Dim contentLength As Integer = Await ProcessUrlAsync(url, _client)
            total += contentLength
        Next

        stopwatch.[Stop]()
        _resultsTextBox.Text += $"{vbCrLf}Total bytes returned:  {total:#,#}"
        _resultsTextBox.Text += $"{vbCrLf}Elapsed time:          {stopwatch.Elapsed}{vbCrLf}"
    End Function

    Private Async Function ProcessUrlAsync(ByVal url As String, ByVal client As HttpClient) As Task(Of Integer)
        Dim content As Byte() = Await client.GetByteArrayAsync(url)
        DisplayResults(url, content)

        Return content.Length
    End Function

    Private Sub DisplayResults(ByVal url As String, ByVal content As Byte())
        _resultsTextBox.Text += $"{url,-60} {content.Length,10:#,#}{vbCrLf}"
    End Sub

    Protected Overrides Sub OnClosed(e As EventArgs)
        _client.Dispose()
    End Sub
End Class
