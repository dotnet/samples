Imports System.Collections

Module Program
    Public Sub Main()
        For Each member As EnvironmentVariableTarget In [Enum].GetValues(GetType(EnvironmentVariableTarget))
            If Environment.OSVersion.Platform <> PlatformID.Win32NT AndAlso member <> EnvironmentVariableTarget.Process Then
                Continue For
            End If

            Console.WriteLine($"Environment variables for {member}:")
            For Each item As DictionaryEntry In Environment.GetEnvironmentVariables(member)
                Console.WriteLine($"   {item.Key}: {item.Value}")
            Next
            Console.WriteLine()
        Next
    End Sub
End Module
