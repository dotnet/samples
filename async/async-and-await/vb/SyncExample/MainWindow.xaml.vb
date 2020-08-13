Imports System.IO
Imports System.Net

Public Class MainWindow
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

    Private Sub OnStartButtonClick(ByVal sender As Object, ByVal e As RoutedEventArgs)
        _resultsTextBox.Clear()

        SumPageSizes()

        _resultsTextBox.Text += $"{vbCrLf}Control returned to {NameOf(OnStartButtonClick)}."
    End Sub

    Private Sub SumPageSizes()
        Dim stopwatch As Stopwatch = Stopwatch.StartNew()

        Dim total As Integer = _urlList.[Select](Function(url) ProcessUrl(url)).Sum()

        stopwatch.[Stop]()
        _resultsTextBox.Text += $"{vbCrLf}Total bytes returned:  {total:#,#}"
        _resultsTextBox.Text += $"{vbCrLf}Elapsed time:          {stopwatch.Elapsed}{vbCrLf}"
    End Sub

    Private Function ProcessUrl(ByVal url As String) As Integer
        Using memoryStream = New MemoryStream()
            Dim webReq = CType(WebRequest.Create(url), HttpWebRequest)

            Using response As WebResponse = webReq.GetResponse()
                Using responseStream As Stream = response.GetResponseStream()
                    responseStream.CopyTo(memoryStream)
                End Using
            End Using

            Dim content As Byte() = memoryStream.ToArray()
            DisplayResults(url, content)
        End Using

        Return Content.Length
    End Function

    Private Sub DisplayResults(ByVal url As String, ByVal content As Byte())
        _resultsTextBox.Text += $"{url,-60} {content.Length,10:#,#}{vbCrLf}"
    End Sub
End Class
