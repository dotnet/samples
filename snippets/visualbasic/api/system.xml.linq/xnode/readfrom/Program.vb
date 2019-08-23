Imports System.Collections.Generic
Imports System.Linq
Imports System.Xml
Imports System.Xml.Linq

Module Program
    Iterator Function StreamRootChildDoc(ByVal uri As String) As IEnumerable(Of XElement)

        Using reader As XmlReader = XmlReader.Create(uri)
            reader.MoveToContent()

            ' Parse the file and return each of the nodes.
            While Not reader.EOF

                If reader.NodeType = XmlNodeType.Element AndAlso reader.Name = "Child" Then
                    Dim el As XElement = TryCast(XElement.ReadFrom(reader), XElement)
                    If el IsNot Nothing Then Yield el
                Else
                    reader.Read()
                End If
            End While
        End Using
    End Function

    Sub Main(args As String())

        Dim grandChildData As IEnumerable(Of String) =
            From el In StreamRootChildDoc("Source.xml")
            Where CInt(el.Attribute("Key")) > 1
            Select CStr(el.Element("GrandChild"))

        For Each str As String In grandChildData
            Console.WriteLine(str)
        Next

    End Sub

End Module