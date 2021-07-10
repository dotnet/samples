'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved. 
'----------------------------------------------------------------

Imports System
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json

Namespace Microsoft.Samples.JsonSerialization

    Class Sample
        Public Shared Sub Main()
            Dim p As New Person()
            p.name = "John"
            p.age = 42

            Dim stream1 As New MemoryStream()

            'Serialize the Person object to a memory stream using DataContractJsonSerializer.
            Dim ser As New DataContractJsonSerializer(GetType(Person))
            ser.WriteObject(stream1, p)

            'Show the JSON output
            stream1.Position = 0
            Dim sr As New StreamReader(stream1)
            Console.Write("JSON form of Person object: ")
            Console.WriteLine(sr.ReadToEnd())

            'Deserialize the JSON back into a new Person object
            stream1.Position = 0
            Dim p2 As Person = DirectCast(ser.ReadObject(stream1), Person)

            'Show the results
            Console.Write("Deserialized back, got name=")
            Console.Write(p2.name)
            Console.Write(", age=")
            Console.WriteLine(p2.age)

            Console.WriteLine("Press <ENTER> to terminate the program.")
            Console.ReadLine()
        End Sub
    End Class

    <DataContract()> _
    Friend Class Person
        <DataMember()> _
        Friend name As String

        <DataMember()> _
        Friend age As Integer
    End Class

End Namespace
