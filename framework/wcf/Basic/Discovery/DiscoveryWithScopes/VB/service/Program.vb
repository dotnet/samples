'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved.
'----------------------------------------------------------------

Imports Microsoft.VisualBasic
Imports System
Imports System.ServiceModel
Imports System.ServiceModel.Description
Imports System.ServiceModel.Discovery

Namespace Microsoft.Samples.Discovery

    Friend Class Program
        Public Shared Sub Main()
            Dim baseAddress As New Uri("http://localhost:8000/" & Guid.NewGuid().ToString())

            ' Create a ServiceHost for the CalculatorService type.
            Dim serviceHost As New ServiceHost(GetType(CalculatorService), baseAddress)

            ' Add an endpoint to the service
            Dim discoverableCalculatorEndpoint As ServiceEndpoint = serviceHost.AddServiceEndpoint(GetType(ICalculatorService), New WSHttpBinding(), "/DiscoverableEndpoint")
            Dim discoverableEndpointBehavior As New EndpointDiscoveryBehavior()
            discoverableEndpointBehavior.Scopes.Add(New Uri("ldap:///ou=engineering,o=exampleorg,c=us"))
            discoverableCalculatorEndpoint.Behaviors.Add(discoverableEndpointBehavior)

            ' Add an endpoint to the service and disable discoverability of that endpoint
            Dim nonDiscoverableCalculatorEndpoint As ServiceEndpoint = serviceHost.AddServiceEndpoint(GetType(ICalculatorService), New WSHttpBinding(), "/NonDiscoverableEndpoint")
            Dim nonDiscoverableEndpointBehavior As New EndpointDiscoveryBehavior()
            nonDiscoverableEndpointBehavior.Scopes.Add(New Uri("ldap:///ou=engineering,o=exampleorg,c=us"))
            nonDiscoverableEndpointBehavior.Enabled = False
            nonDiscoverableCalculatorEndpoint.Behaviors.Add(nonDiscoverableEndpointBehavior)

            Try
                ' Make the service discoverable over UDP multicast
                serviceHost.Description.Behaviors.Add(New ServiceDiscoveryBehavior())
                serviceHost.AddServiceEndpoint(New UdpDiscoveryEndpoint())

                serviceHost.Open()

                Console.WriteLine("Calculator Service started at {0}", baseAddress)
                Console.WriteLine()
                Console.WriteLine("Press <ENTER> to terminate the service.")
                Console.WriteLine()
                Console.ReadLine()
                serviceHost.Close()
            Catch ex As CommunicationException
                Console.WriteLine(ex.Message)
            Catch ex As TimeoutException
                Console.WriteLine(ex.Message)
            End Try

            If serviceHost.State <> CommunicationState.Closed Then
                Console.WriteLine("Aborting the service...")
                serviceHost.Abort()
            End If
        End Sub
    End Class
End Namespace
