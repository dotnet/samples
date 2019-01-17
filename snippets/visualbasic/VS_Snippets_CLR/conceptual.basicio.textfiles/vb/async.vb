Imports System
Imports System.IO

Class WriteTextAsync

    Public Shared Async Sub Main()

        ' Set a variable to the Documents path.
        Dim docPath As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

        ' Write the text asynchronously to a new file named "WriteTextAsync.txt".
        Using outputFile As New StreamWriter(Path.Combine(docPath, Convert.ToString("WriteTextAsync.txt")))
            Await outputFile.WriteAsync("This is a sentence.")
        End Using
    End Sub

End Class
