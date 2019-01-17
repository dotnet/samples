' <Snippet1>
Imports System.Collections.Generic
Imports System.IO

Module Module1

    Sub Main()
        Try
            Dim dirPath As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

            Dim dirs As List(Of String) = New List(Of String)(Directory.EnumerateDirectories(dirPath))

            For Each folder In dirs
                Console.WriteLine($"{dir.Substring(dir.LastIndexOf("\\") + 1)}")
            Next
            Console.WriteLine($"{dirs.Count} directories found.")
        Catch uAEx As UnauthorizedAccessException
            Console.WriteLine(uAEx.Message)
        Catch pathEx As PathTooLongException
            Console.WriteLine(pathEx.Message)
        End Try

    End Sub
End Module
' </Snippet1>
