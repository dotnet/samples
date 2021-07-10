'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved.
'----------------------------------------------------------------

Imports Microsoft.VisualBasic
Imports System
Imports System.ServiceModel
Imports System.ServiceModel.Discovery

Namespace Microsoft.Samples.Discovery

	Friend Class Program
		Public Shared Sub Main()
			Dim baseAddress As New Uri("http://localhost:8000/" & Guid.NewGuid().ToString())

			' Create a ServiceHost for the CalculatorService type.
            Dim serviceHost As New ServiceHost(GetType(CalculatorService), baseAddress)
            serviceHost.AddServiceEndpoint(GetType(ICalculatorService), New WSHttpBinding(), String.Empty)

            Dim serviceDiscoveryBehavior As New ServiceDiscoveryBehavior()

            ' Announce the availability of the service over UDP multicast
            serviceDiscoveryBehavior.AnnouncementEndpoints.Add(New UdpAnnouncementEndpoint())

            Try
                ' Make the service discoverable over UDP multicast
                serviceHost.Description.Behaviors.Add(serviceDiscoveryBehavior)
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
