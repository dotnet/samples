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
            "https://docs.microsoft.com/windows",
            "https://docs.microsoft.com/office",
            "https://docs.microsoft.com/enterprise-mobility-security",
            "https://docs.microsoft.com/visualstudio",
            "https://docs.microsoft.com/microsoft-365",
            "https://docs.microsoft.com/sql",
            "https://docs.microsoft.com/dynamics365",
            "https://docs.microsoft.com/surface",
            "https://docs.microsoft.com/xamarin",
            "https://docs.microsoft.com/azure/devops",
            "https://docs.microsoft.com/system-center",
            "https://docs.microsoft.com/graph",
            "https://docs.microsoft.com/education",
            "https://docs.microsoft.com/gaming"
        }

    Private Sub OnStartButtonClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        _startButton.IsEnabled = False
        _resultsTextBox.Clear()

        Task.Run(Function() StartSumPageSizesAsync())
    End Sub

    Private Async Function StartSumPageSizesAsync() As Task
        Await SumPageSizesAsync()
        Await Dispatcher.BeginInvoke(
            Sub()
                _resultsTextBox.Text += $"{vbCrLf}Control returned to {NameOf(OnStartButtonClick)}."
                _startButton.IsEnabled = True
            End Sub)
    End Function

    Private Async Function SumPageSizesAsync() As Task
        Dim stopwatch As Stopwatch = Stopwatch.StartNew()

        Dim downloadTasksQuery As IEnumerable(Of Task(Of Integer)) =
            From url In _urlList
            Select ProcessUrlAsync(url, _client)

        Dim downloadTasks As Task(Of Integer)() = downloadTasksQuery.ToArray()

        Dim lengths As Integer() = Await Task.WhenAll(downloadTasks)
        Dim total As Integer = lengths.Sum()

        Await Dispatcher.BeginInvoke(
            Sub()
                stopwatch.[Stop]()
                _resultsTextBox.Text += $"{vbCrLf}Total bytes returned:  {total:#,#}"
                _resultsTextBox.Text += $"{vbCrLf}Elapsed time:          {stopwatch.Elapsed}{vbCrLf}"
            End Sub)
    End Function

    Private Async Function ProcessUrlAsync(ByVal url As String, ByVal client As HttpClient) As Task(Of Integer)
        Dim content As Byte() = Await client.GetByteArrayAsync(url)
        Await DisplayResultsAsync(url, content)

        Return content.Length
    End Function

    Private Function DisplayResultsAsync(ByVal url As String, ByVal content As Byte()) As Task
        Return Dispatcher.BeginInvoke(
            Sub()
                _resultsTextBox.Text += $"{url,-60} {content.Length,10:#,#}{vbCrLf}"
            End Sub).Task
    End Function

    Protected Overrides Sub OnClosed(e As EventArgs)
        _client.Dispose()
    End Sub
End Class
