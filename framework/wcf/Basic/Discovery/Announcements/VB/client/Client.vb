'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved.
'----------------------------------------------------------------

Imports Microsoft.VisualBasic
Imports System
Imports System.ServiceModel
Imports System.ServiceModel.Discovery
Imports System.Xml

Namespace Microsoft.Samples.Discovery

	Friend Class Client
        Public Shared Sub Main()

            ' Create an AnnouncementService instance
            Dim announcementService As New AnnouncementService()

            ' Subscribe the announcement events
            AddHandler announcementService.OnlineAnnouncementReceived, AddressOf OnOnlineEvent
            AddHandler announcementService.OfflineAnnouncementReceived, AddressOf OnOfflineEvent

            ' Create ServiceHost for the AnnouncementService
            Dim announcementServiceHost As New ServiceHost(announcementService)

            Try
                ' Listen for the announcements sent over UDP multicast
                announcementServiceHost.AddServiceEndpoint(New UdpAnnouncementEndpoint())
                announcementServiceHost.Open()

                Console.WriteLine("Listening for service announcements.")
                Console.WriteLine()
                Console.WriteLine("Press <ENTER> to terminate.")
                Console.ReadLine()
                announcementServiceHost.Close()
            Catch ex As CommunicationException
                Console.WriteLine(ex.Message)
            Catch ex As TimeoutException
                Console.WriteLine(ex.Message)
            End Try
            
            If announcementServiceHost.State <> CommunicationState.Closed Then
                Console.WriteLine("Aborting the service...")
                announcementServiceHost.Abort()
            End If

        End Sub

		Private Shared Sub OnOnlineEvent(ByVal sender As Object, ByVal e As AnnouncementEventArgs)
			Console.WriteLine()
			Console.WriteLine("Received an online announcement from {0}:", e.EndpointDiscoveryMetadata.Address)
			PrintEndpointDiscoveryMetadata(e.EndpointDiscoveryMetadata)
		End Sub

		Private Shared Sub OnOfflineEvent(ByVal sender As Object, ByVal e As AnnouncementEventArgs)
			Console.WriteLine()
			Console.WriteLine("Received an offline announcement from {0}:", e.EndpointDiscoveryMetadata.Address)
			PrintEndpointDiscoveryMetadata(e.EndpointDiscoveryMetadata)
		End Sub

		Private Shared Sub PrintEndpointDiscoveryMetadata(ByVal endpointDiscoveryMetadata As EndpointDiscoveryMetadata)
            For Each contractTypeName As XmlQualifiedName In endpointDiscoveryMetadata.ContractTypeNames
                Console.WriteLine(Constants.vbTab & "ContractTypeName: {0}", contractTypeName)
            Next contractTypeName
			For Each scope As Uri In endpointDiscoveryMetadata.Scopes
				Console.WriteLine(Constants.vbTab & "Scope: {0}", scope)
			Next scope
			For Each listenUri As Uri In endpointDiscoveryMetadata.ListenUris
				Console.WriteLine(Constants.vbTab & "ListenUri: {0}", listenUri)
			Next listenUri
		End Sub
	End Class
End Namespace

