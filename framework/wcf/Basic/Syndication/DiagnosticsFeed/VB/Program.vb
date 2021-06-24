'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved. 
'----------------------------------------------------------------

Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq
Imports System.Xml
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.ServiceModel.Syndication
Imports System.ServiceModel.Web
Imports System.Text
Imports System.Collections.ObjectModel

Namespace Microsoft.Samples.DiagnosticsFeed
    <ServiceContract([Namespace]:="")> _
    Interface IDiagnosticsService
        'The [WebGet] attribute controls how WCF dispatches
        'HTTP requests to service operations based on URI suffix
        '(the part of the request URI after the endpoint address)
        'using the HTTP GET method. The UriTemplate specifies a relative
        'path of 'feed', and specifies that the format will be
        'supplied via a query string. 
        <WebGet(UriTemplate:="feed?format={format}")> _
        <ServiceKnownType(GetType(Atom10FeedFormatter))> _
        <ServiceKnownType(GetType(Rss20FeedFormatter))> _
        Function GetProcesses(ByVal format As String) As SyndicationFeedFormatter
    End Interface

    Public Class ProcessService
        Implements IDiagnosticsService
        Public Function GetProcesses(ByVal format As String) As SyndicationFeedFormatter Implements IDiagnosticsService.GetProcesses
            Dim processes As IEnumerable(Of Process) = New List(Of Process)(Process.GetProcesses())

            'SyndicationFeed also has convenience constructors
            'that take in common elements like Title and Content.
            Dim f As New SyndicationFeed()

            f.LastUpdatedTime = DateTimeOffset.Now

            'Create a title for the feed.
            f.Title = SyndicationContent.CreatePlaintextContent("Currently running processes")
            f.Links.Add(SyndicationLink.CreateSelfLink(OperationContext.Current.IncomingMessageHeaders.To))

            f.Items = From p In processes _
                      Select New SyndicationItem() With _
                                                        { _
                                                          .LastUpdatedTime = DateTimeOffset.Now, _
                                                          .Title = SyndicationContent.CreatePlaintextContent(p.ProcessName), _
                                                          .Summary = SyndicationContent.CreateHtmlContent(String.Format("<b>{0}</b>", p.MainWindowTitle)), _
                                                          .Content = SyndicationContent.CreateXmlContent(New ProcessData(p)) _
                                                         }

            ' Choose formatter according to query string passed
            If format = "rss" Then
                Return New Rss20FeedFormatter(f)
            Else
                Return New Atom10FeedFormatter(f)
            End If
        End Function
    End Class

    Class Program
        Shared Sub Main(ByVal args As String())
            Dim host As New WebServiceHost(GetType(ProcessService), New Uri("http://localhost:8000/diagnostics"))
            host.Open()

            Console.WriteLine("Service host open")

            'The syndication feeds exposed by the service are available
            'at the following URI (for Atom use feed?format=atom)

            ' http://localhost:8000/diagnostics/feed?format=rss

            'These feeds can be viewed directly in an RSS-aware client
            'such as IE7.

            ' Change value of feed query string to 'atom' to use Atom format
            Dim reader As XmlReader = XmlReader.Create("http://localhost:8000/diagnostics/feed?format=rss", _
                                                       New XmlReaderSettings() With _
                                                       { _
                                                         .MaxCharactersInDocument = 1024 * 64 _
                                                       })

            Dim feed As SyndicationFeed = SyndicationFeed.Load(XmlReader.Create("http://localhost:8000/diagnostics/feed?format=rss"))

            For Each i As SyndicationItem In feed.Items
                Dim content As XmlSyndicationContent = TryCast(i.Content, XmlSyndicationContent)
                Dim pd As ProcessData = content.ReadContent(Of ProcessData)()

                Console.WriteLine(i.Title.Text)
                Console.WriteLine(pd.ToString())
            Next

            'Press any key to end the program
            Console.ReadLine()
        End Sub
    End Class
End Namespace
