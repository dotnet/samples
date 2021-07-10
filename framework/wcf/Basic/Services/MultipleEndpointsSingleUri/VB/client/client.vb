' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.ServiceModel
Imports System.ServiceModel.Description

Namespace Microsoft.ServiceModel.Samples
    'The service contract is defined in generatedClient.vb, generated from the service by the svcutil tool.

    'Client implementation code.
    Class Client

        Public Shared Sub Main()

            ' Note that the ListenUri must be communicated out-of-band.
            ' That is, the metadata exposed by the service does not publish
            ' the ListenUri, and thus the svcutil-generated config doesn't 
            ' know about it.  

            ' On the client, use ClientViaBehavior to specify 
            ' the Uri where the server is listening.
            Dim via As New Uri("http://localhost/ServiceModelSamples/service.svc")

            ' Create a client to talk to the Calculator contract
            Dim calcClient As New CalculatorClient()
            calcClient.ChannelFactory.Endpoint.Behaviors.Add(New ClientViaBehavior(via))

            Dim value1 As Double = 100
            Dim value2 As Double = 15.99
            Dim result As Double = calcClient.Add(value1, value2)
            Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result)
            calcClient.Close()

            ' Create a client to talk to the Echo contract that is located
            ' at the same EndpointAddress and ListenUri as the calculator contract
            Dim echoClient As New EchoClient("WSHttpBinding_IEcho")
            echoClient.ChannelFactory.Endpoint.Behaviors.Add(New ClientViaBehavior(via))
            Console.WriteLine(echoClient.Echo("Hello!"))
            echoClient.Close()

            ' Create a client to talk to the Echo contract that is located
            ' at a different EndpointAddress, but the same ListenUri
            Dim echoClient1 As New EchoClient("WSHttpBinding_IEcho1")
            echoClient1.ChannelFactory.Endpoint.Behaviors.Add(New ClientViaBehavior(via))
            Console.WriteLine(echoClient1.Echo("Hello!"))
            echoClient.Close()

            Console.WriteLine()
            Console.WriteLine("Press <ENTER> to terminate client application.")
            Console.ReadLine()

        End Sub

    End Class

End Namespace

