Imports System.IO
Imports System.Text

Module ConsoleModule
    Sub Main()
        Dim args() As String = Environment.GetCommandLineArgs()

        ' Get command line arguments.
        If args.Length <> 3 OrElse String.IsNullOrWhitespace(args(1)) OrElse String.IsNullOrWhitespace(args(2)) Then
            Console.WriteLine("There must be a source and a destination file.") : ShowSyntax()
            Exit Sub
        End if

        Dim source As String = args(1)
        Dim destination As String = args(2)

        If Not File.Exists(source) Then
            Console.WriteLine($"The source file {vbCrLf}   '{source}'{vbCrLf}cannot be found.") : ShowSyntax()
            Exit Sub
        End if

        Try
            Using sr As New StreamReader(source)

                ' Check whether destination file exists and exit if it should not be overwritten.
                If File.Exists(destination) Then
                    Console.Write("The destination file {1}   '{0}'{1}exists. Overwrite it? (Y/N) ", source, vbCrLf)
                    Dim keyPressed As ConsoleKeyInfo = Console.ReadKey(True)
                    If Char.ToUpper(keyPressed.KeyChar) = "Y"c Or Char.ToUpper(keyPressed.KeyChar) = "N"c Then
                        Console.WriteLine(keyPressed.KeyChar)
                        If Char.ToUpper(keyPressed.KeyChar) = "N" Then Exit Sub
                    End If
                End If
                Using sw As New StreamWriter(destination, False, Encoding.UTF8)
                    ' Instantiate the encoder
                    Dim encoding As Encoding = encoding.GetEncoding("us-ascii", New CyrillicToLatinFallback(), New DecoderExceptionFallback())
                    ' This is an encoding operation, so we only need to get the encoder.
                    Dim encoder As Encoder = encoding.GetEncoder()
                    Dim decoder As Decoder = encoding.GetDecoder()

                    ' Define buffer to read characters
                    Dim buffer(99) As Char
                    Dim charsRead As Integer

                    Do
                        ' Read next 100 characters from input stream.
                        charsRead = sr.ReadBlock(buffer, 0, buffer.Length)

                        ' Encode characters.
                        Dim byteCount As Integer = encoder.GetByteCount(buffer, 0, charsRead, False)
                        Dim bytes(byteCount - 1) As Byte
                        Dim bytesWritten As Integer = encoder.GetBytes(buffer, 0, charsRead, bytes, 0, False)

                        ' Decode characters back to Unicode and write to a UTF-8-encoded file.
                        Dim charsToWrite(decoder.GetCharCount(bytes, 0, byteCount)) As Char
                        decoder.GetChars(bytes, 0, bytesWritten, charsToWrite, 0)
                        sw.Write(charsToWrite)
                    Loop While charsRead = buffer.Length
                End Using
            End Using
        Catch e As DirectoryNotFoundException
            Console.WriteLine($"Invalid directory: {e.Message}")
        Catch e As IOException
            Console.WriteLine($"I/O exception: {e.Message}")
        End Try
    End Sub

    Private Sub ShowSyntax()
        Console.WriteLine()
        Console.WriteLine("Syntax: CyrillicToRoman <source> <destination>")
        Console.WriteLine("   where <source>      = source filename")
        Console.WriteLine("         <destination> = destination filename")
        Console.WriteLine()
    End Sub
End Module
