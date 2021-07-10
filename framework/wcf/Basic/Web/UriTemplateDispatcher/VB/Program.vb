'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved.
'----------------------------------------------------------------

Imports System
Imports System.Collections.Generic
Namespace Microsoft.Samples.UriTemplateDispatcher

    Public Class Program
        Private Delegate Sub Handler(ByVal results As UriTemplateMatch)

        Public Shared Sub Main()
            Dim prefix As New Uri("http://localhost/")

            'One interesting use for UriTemplateTable is as a dispatching engine.
            'This is accomplished by using the UriTemplateTable to associate
            'a delegate handler with each UriTemplate.

            'To start, we need a UriTemplateTable.
            Dim table As New UriTemplateTable(prefix)

            'Now, we add templates to the table and associate them
            'with handler functions, written as anonymous delegates:

            [AddHandler](table, New UriTemplate("weather/{state}/{city}"), AddressOf TemplateMatchHandler1)
            [AddHandler](table, New UriTemplate("weather/{state}"), AddressOf TemplateMatchHandler2)
            [AddHandler](table, New UriTemplate("traffic/*"), AddressOf TemplateMatchHandler3)

			'MakeReadOnly() freezes the table and prevents further additions.
            'Passing false to this method will disallow duplicate URI's,
            'guaranteeing that at most a single match will be returned. 
            'Calling this method explictly is optional, as the collection
            'will be made read-only during the first call to Match() or MatchSingle()

		table.MakeReadOnly(False)

            Dim request As Uri = Nothing

            'Match WeatherByCity
            request = New Uri("http://localhost/weather/Washington/Seattle")
            Console.WriteLine(request)
            InvokeDelegate(table.MatchSingle(request))

            'Match WeatherByState
            request = New Uri("http://localhost/weather/Washington")
            Console.WriteLine(request)
            InvokeDelegate(table.MatchSingle(request))

            'Wildcard match Traffic
            request = New Uri("http://localhost/Traffic/Washington/Seattle/SR520")
            Console.WriteLine(request)
            InvokeDelegate(table.MatchSingle(request))

            Console.ReadLine()

        End Sub

        Private Shared Sub TemplateMatchHandler1(ByVal r As UriTemplateMatch)
            Console.WriteLine("Matched the WeatherByCity template")
            Console.WriteLine("State: {0}", r.BoundVariables("state"))
            Console.WriteLine("City: {0}", r.BoundVariables("city"))
        End Sub
        Private Shared Sub TemplateMatchHandler2(ByVal r As UriTemplateMatch)
            Console.WriteLine("Matched the WeatherByState template")
            Console.WriteLine("State: {0}", r.BoundVariables("state"))
        End Sub
        Private Shared Sub TemplateMatchHandler3(ByVal r As UriTemplateMatch)
            Console.WriteLine("Matched the traffic/* template")
            Console.WriteLine("Wildcard segments:")

            For Each s As String In r.WildcardPathSegments
                Console.WriteLine(" " + s)
            Next
        End Sub

        Private Shared Sub [AddHandler](ByVal table As UriTemplateTable, ByVal template As UriTemplate, ByVal handler As Handler)
            table.KeyValuePairs.Add(New KeyValuePair(Of UriTemplate, Object)(template, handler))
        End Sub


        Private Shared Sub InvokeDelegate(ByVal results As UriTemplateMatch)
            If results Is Nothing Then
                Console.WriteLine("No Match")
            Else
                Dim handler As Handler = CType(results.Data, Handler)
                handler(results)
            End If

            Console.WriteLine("")
        End Sub
    End Class
End Namespace
