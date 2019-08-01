Imports System.Security.Cryptography
Imports System.Text

Public Module Program
    Public Sub Main()
        Dim source As String = "Hello World!"
        Using sha256Hash As SHA256 = SHA256.Create()

            Dim hash As String = GetHash(sha256Hash, source)

            Console.WriteLine($"The SHA256 hash of {source} is: {hash}.")

            Console.WriteLine("Verifying the hash...")

            If VerifyHash(sha256Hash, source, hash) Then
                Console.WriteLine("The hashes are the same.")
            Else
                Console.WriteLine("The hashes are not same.")
            End If
        End Using
    End Sub 

    Private Function GetHash(ByVal hashAlgorithm As HashAlgorithm, ByVal input As String) As String

        ' Convert the input string to a byte array and compute the hash.
        Dim data As Byte() = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input))

        ' Create a new Stringbuilder to collect the bytes
        ' and create a string.
        Dim sBuilder As New StringBuilder()

        ' Loop through each byte of the hashed data 
        ' and format each one as a hexadecimal string.
        For i As Integer = 0 To data.Length - 1
            sBuilder.Append(data(i).ToString("x2"))
        Next

        ' Return the hexadecimal string.
        Return sBuilder.ToString()
    End Function

    ' Verify a hash against a string.
    Private Function VerifyHash(hashAlgorithm As HashAlgorithm, input As String, hash As String) As Boolean
        ' Hash the input.
        Dim hashOfInput As String = GetHash(hashAlgorithm, input)

        ' Create a StringComparer an compare the hashes.
        Dim comparer As StringComparer = StringComparer.OrdinalIgnoreCase

        Return comparer.Compare(hashOfInput, hash) = 0
    End Function
End Module
' The example displays the following output:
'    The SHA256 hash of Hello World! is: 7f83b1657ff1fc53b92dc18148a1d65dfc2d4b1fa3d677284addd200126d9069.
'    Verifying the hash...
'    The hashes are the same.
