'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation. All rights reserved.
'----------------------------------------------------------------

 Imports System
 Imports System.Collections.Specialized
 Imports System.Collections.Generic
Namespace Microsoft.Samples.UriTemplateTables

    Public Class Program
        Public Shared Sub Main()
            Dim prefix As New Uri("http://localhost/")

            'A UriTemplateTable is an associative set of UriTemplates. It lets you match
            'candidate URI's against the templates in the set, and retrieve the data associated
            'with the matching templates.

            'To start, we need some example templates and a table to put them in:
            Dim weatherByCity As New UriTemplate("weather/{state}/{city}")
            Dim weatherByState As New UriTemplate("weather/{state}")
            Dim traffic As New UriTemplate("traffic/*")
            Dim wildcard As New UriTemplate("*")

            Dim table As New UriTemplateTable(prefix)

            'Now we associate each template with some data. Here we're just using strings;
            'you can associate anything you want with the template.
            table.KeyValuePairs.Add(New KeyValuePair(Of UriTemplate, Object)(weatherByCity, "weatherByCity"))
            table.KeyValuePairs.Add(New KeyValuePair(Of UriTemplate, Object)(weatherByState, "weatherByState"))
            table.KeyValuePairs.Add(New KeyValuePair(Of UriTemplate, Object)(traffic, "traffic"))

            'Once the table is populated, we can use the MatchSingle function to
            'retrieve some match results:

            Dim results As UriTemplateMatch = Nothing
            Dim weatherInSeattle As New Uri("http://localhost/weather/Washington/Seattle")

            'The Match function will select the best match for the incoming URI 
            'based on the templates in the table. There are two variants of Match --
            'Match() can potentially return multiple results, which MatchSingle() will
            'either return exactly one result or throw an exception.
            results = table.MatchSingle(weatherInSeattle)

            'We get back the associated data in the Data member of
            'the UriTemplateMatchResults that are returned.
            If results IsNot Nothing Then
                'Output will be "weatherByCity"
                Console.WriteLine(results.Data)

                'The UriTemplateMatch contains useful data obtained during the
                'matching process.

                Console.WriteLine("State: {0}", results.BoundVariables("state"))
                Console.WriteLine("City: {0}", results.BoundVariables("city"))
            End If

            Console.ReadLine()
        End Sub
    End Class

End Namespace
