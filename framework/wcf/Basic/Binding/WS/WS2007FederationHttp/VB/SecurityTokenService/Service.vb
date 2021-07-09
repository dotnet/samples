' Copyright (c) Microsoft Corporation. All rights reserved.

Imports System
Imports System.Security.Permissions
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Dispatcher
Imports Microsoft.VisualBasic

Namespace Microsoft.Samples.WS2007FederationHttpBinding
    Class Service
        Shared Sub Main(ByVal args As String())
            ' Create ServiceHost. 
            Dim sh As New ServiceHost(GetType(SecurityTokenService))
            sh.Open()

            Try
                For Each cd As ChannelDispatcher In sh.ChannelDispatchers
                    For Each ed As EndpointDispatcher In cd.Endpoints
                        Console.WriteLine("STS listening at {0}", ed.EndpointAddress.Uri)
                    Next
                Next

                Console.WriteLine("" & Chr(10) & "Press enter to exit" & Chr(10) & "")
                Console.ReadLine()
            Finally
                sh.Close()
            End Try
        End Sub
    End Class
End Namespace
