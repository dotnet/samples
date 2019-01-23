' <snippet6>
Imports System.IO
Imports System.Windows

Class MainWindow
    Private Async Sub ReadFileButton_Click(sender As Object, e As RoutedEventArgs)
        Try
            Using sr As StreamReader = New StreamReader("TestFile.txt")
                Dim line = Await sr.ReadToEndAsync()
                Console.WriteLine(line)
            End Using
        Catch ex As FileNotFoundException
            Console.WriteLine(ex.Message)
        End Try
    End Sub
End Class
' </snippet6>