'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved.
'----------------------------------------------------------------

 Imports System
 Imports System.Collections.Specialized
Namespace Microsoft.Samples.UriTemplates

    Public Class Program
        Public Shared Sub Main()
            Dim prefix As New Uri("http://localhost/")

            'A UriTemplate is a "URI with holes". It describes a set of URI's that
            'are structurally similar. This UriTemplate might be used for organizing
            'weather reports:
            Dim template As New UriTemplate("weather/{state}/{city}")

            'You can convert a UriTemplate into a Uri by filling
            'the holes in the template with parameters.

            'BindByPosition moves left-to-right across the template
            Dim positionalUri As Uri = template.BindByPosition(prefix, "Washington", "Redmond")

            Console.WriteLine("Calling BindByPosition...")
            Console.WriteLine(positionalUri)
            Console.WriteLine()

            'BindByName takes a NameValueCollection of parameters.
            'Each parameter gets substituted into the UriTemplate "hole"
            'that has the same name as the parameter.
            Dim parameters As New NameValueCollection()
            parameters.Add("state", "Washington")
            parameters.Add("city", "Redmond")

            Dim namedUri As Uri = template.BindByName(prefix, parameters)

            Console.WriteLine("Calling BindByName...")
            Console.WriteLine(namedUri)
            Console.WriteLine()


            'The inverse operation of Bind is Match(), which extrudes a URI
            'through the template to produce a set of name/value pairs.
            Dim fullUri As New Uri("http://localhost/weather/Washington/Redmond")
            Dim results As UriTemplateMatch = template.Match(prefix, fullUri)

            Console.WriteLine([String].Format("Matching {0} to {1}", template.ToString(), fullUri.ToString()))

            If results IsNot Nothing Then
                For Each variableName As String In results.BoundVariables.Keys
                    Console.WriteLine([String].Format("   {0}: {1}", variableName, results.BoundVariables(variableName)))
                Next
            End If

            Console.ReadLine()
        End Sub
    End Class
End Namespace
