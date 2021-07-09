' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.ServiceModel

Namespace Microsoft.ServiceModel.Samples

    'The service contract is defined in generatedClient.vb, generated from the service by the svcutil tool.

    'Client implementation code.
    Class Client
        Implements ISampleContractCallback

        Public Shared Sub Main(ByVal args As String())

            Dim site As New InstanceContext(Nothing, New Client())
            Dim client As New SampleContractClient(site)

            'create a unique callback address so multiple clients can run on one machine
            Dim binding As WSDualHttpBinding = DirectCast(client.Endpoint.Binding, WSDualHttpBinding)
            Dim clientcallbackaddress As String = binding.ClientBaseAddress.AbsoluteUri
            clientcallbackaddress += Guid.NewGuid().ToString()
            binding.ClientBaseAddress = New Uri(clientcallbackaddress)

            'Subscribe.
            Console.WriteLine("Subscribing")
            client.Subscribe()

            Console.WriteLine()
            Console.WriteLine("Press ENTER to unsubscribe and shut down client")
            Console.ReadLine()

            Console.WriteLine("Unsubscribing")
            client.Unsubscribe()

            'Closing the client gracefully closes the connection and cleans up resources
            client.Close()

        End Sub

        Public Sub PriceChange(ByVal item As String, ByVal price As Double, ByVal change As Double) Implements ISampleContractCallback.PriceChange

            Console.WriteLine("PriceChange(item {0}, price {1}, change {2})", item, price.ToString("C"), change)

        End Sub

    End Class

End Namespace

