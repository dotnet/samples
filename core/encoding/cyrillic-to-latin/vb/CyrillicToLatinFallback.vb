Imports System.IO
Imports System.Text

Public Class CyrillicToLatinFallback : Inherits EncoderFallback

    Private table As Dictionary(Of Char, String)

    Public Sub New()
        table = New Dictionary(Of Char, String)
        ' Define mappings.
        ' Uppercase modern Cyrillic characters.
        table.Add(ChrW(&H0410), "A")
        table.Add(ChrW(&H0411), "B")
        table.Add(ChrW(&H0412), "V")
        table.Add(ChrW(&H0413), "G")
        table.Add(ChrW(&H0414), "D")
        table.Add(ChrW(&H0415), "E")
        table.Add(ChrW(&H0416), "Zh")
        table.Add(ChrW(&H0417), "Z")
        table.Add(ChrW(&H0418), "I")
        table.Add(ChrW(&H0419), "I")
        table.Add(ChrW(&H041A), "K")
        table.Add(ChrW(&H041B), "L")
        table.Add(ChrW(&H041C), "M")
        table.Add(ChrW(&H041D), "N")
        table.Add(ChrW(&H041E), "O")
        table.Add(ChrW(&H041F), "P")
        table.Add(ChrW(&H0420), "R")
        table.Add(ChrW(&H0421), "S")
        table.Add(ChrW(&H0422), "T")
        table.Add(ChrW(&H0423), "U")
        table.Add(ChrW(&H0424), "F")
        table.Add(ChrW(&H0425), "Kh")
        table.Add(ChrW(&H0426), "Ts")
        table.Add(ChrW(&H0427), "Ch")
        table.Add(ChrW(&H0428), "Sh")
        table.Add(ChrW(&H0429), "Shch")
        table.Add(ChrW(&H042A), "'")    ' Hard sign
        table.Add(ChrW(&H042B), "Ye")
        table.Add(ChrW(&H042C), "'")    ' Soft sign
        table.Add(ChrW(&H042D), "E")
        table.Add(ChrW(&H042E), "Iu")
        table.Add(ChrW(&H042F), "Ia")
        ' Lowercase modern Cyrillic characters.
        table.Add(ChrW(&H0430), "a")
        table.Add(ChrW(&H0431), "b")
        table.Add(ChrW(&H0432), "v")
        table.Add(ChrW(&H0433), "g")
        table.Add(ChrW(&H0434), "d")
        table.Add(ChrW(&H0435), "e")
        table.Add(ChrW(&H0436), "zh")
        table.Add(ChrW(&H0437), "z")
        table.Add(ChrW(&H0438), "i")
        table.Add(ChrW(&H0439), "i")
        table.Add(ChrW(&H043A), "k")
        table.Add(ChrW(&H043B), "l")
        table.Add(ChrW(&H043C), "m")
        table.Add(ChrW(&H043D), "n")
        table.Add(ChrW(&H043E), "o")
        table.Add(ChrW(&H043F), "p")
        table.Add(ChrW(&H0440), "r")
        table.Add(ChrW(&H0441), "s")
        table.Add(ChrW(&H0442), "t")
        table.Add(ChrW(&H0443), "u")
        table.Add(ChrW(&H0444), "f")
        table.Add(ChrW(&H0445), "kh")
        table.Add(ChrW(&H0446), "ts")
        table.Add(ChrW(&H0447), "ch")
        table.Add(ChrW(&H0448), "sh")
        table.Add(ChrW(&H0449), "shch")
        table.Add(ChrW(&H044A), "'")   ' Hard sign
        table.Add(ChrW(&H044B), "yi")
        table.Add(ChrW(&H044C), "'")   ' Soft sign
        table.Add(ChrW(&H044D), "e")
        table.Add(ChrW(&H044E), "iu")
        table.Add(ChrW(&H044F), "ia")
    End Sub

    Public Overrides Function CreateFallbackBuffer() As System.Text.EncoderFallbackBuffer
        Return New CyrillicToLatinFallbackBuffer(table)
    End Function

    Public Overrides ReadOnly Property MaxCharCount As Integer
        Get
            Return 4                  ' Maximum is "Shch" and "shch"
        End Get
    End Property
End Class

Public Class CyrillicToLatinFallbackBuffer : Inherits EncoderFallbackBuffer
    Private table As Dictionary(Of Char, String)
    Private bufferIndex As Integer
    Private buffer As String
    Private leftToReturn As Integer

    Friend Sub New(ByVal table As Dictionary(Of Char, String))
        Me.table = table
        Me.bufferIndex = -1
        Me.leftToReturn = -1
    End Sub

    Public Overloads Overrides Function Fallback(ByVal charUnknownHigh As Char, ByVal charUnknownLow As Char, ByVal index As Integer) As Boolean
        ' There's no need to handle surrogates.
        Return False
    End Function

    Public Overloads Overrides Function Fallback(ByVal charUnknown As Char, ByVal index As Integer) As Boolean
        If charUnknown >= ChrW(&H0410) And charUnknown <= ChrW(&H044F) Then
            buffer = table.Item(charUnknown)
            leftToReturn = buffer.Length - 1
            bufferIndex = -1
            Return True
        End If
        Return False
    End Function

    Public Overrides Function GetNextChar() As Char
        Dim charToReturn As Char
        If leftToReturn >= 0 Then
            leftToReturn -= 1
            bufferIndex += 1
            charToReturn = buffer(bufferIndex)
        Else
            charToReturn = ChrW(0)
        End If
        Return charToReturn
    End Function

    Public Overrides Function MovePrevious() As Boolean
        If bufferIndex > 0 Then
            bufferIndex -= 1
            leftToReturn += 1
            Return True
        End If
        Return False
    End Function

    Public Overrides ReadOnly Property Remaining As Integer
        Get
            Return leftToReturn
        End Get
    End Property
End Class
