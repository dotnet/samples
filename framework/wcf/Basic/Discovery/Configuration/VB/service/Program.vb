'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved.
'----------------------------------------------------------------

Imports System
Imports System.ServiceModel


Namespace Microsoft.Samples.Discovery

	Friend Class Program
		Public Shared Sub Main()
			Dim baseAddress As New Uri("http://localhost:8000/" & Guid.NewGuid().ToString())

            Dim serviceHost As New ServiceHost(GetType(CalculatorService), baseAddress)
            Try
                ' ServiceDiscoveryBehavior is added through the configuration.
                ' See App.config. The service is discoverable over UDP multicast

                serviceHost.Open()

                Console.WriteLine("Calculator Service started {0}", baseAddress)
                Console.WriteLine()
                Console.WriteLine("Press <ENTER> to terminate the service")
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
