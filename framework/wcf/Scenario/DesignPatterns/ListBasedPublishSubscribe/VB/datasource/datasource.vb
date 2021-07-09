' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.ServiceModel

Namespace Microsoft.ServiceModel.Samples

    'The service contract is defined in generatedClient.vb, generated from the service by the svcutil tool.

    'Client implementation code.
    Class Client
        Implements ISampleContractCallback

        Public Shared Sub Main(ByVal args As String())

            Dim site As New InstanceContext(New Client())
            Dim client As New SampleContractClient(site)

            Console.WriteLine("Sending PublishPriceChange(Gold, 400.00D, -0.25D)")
            client.PublishPriceChange("Gold", 400, -0.25)

            Console.WriteLine("Sending PublishPriceChange(Silver, 7.00D, -0.20D)")
            client.PublishPriceChange("Silver", 7, -0.2)
            Console.WriteLine("Sending PublishPriceChange(Platinum, 850.00D, +0.50D)")

            client.PublishPriceChange("Platinum", 850, +0.5)
            Console.WriteLine("Sending PublishPriceChange(Gold, 401.00D, 1.00D)")

            client.PublishPriceChange("Gold", 401, 1)
            Console.WriteLine("Sending PublishPriceChange(Silver, 6.60D, -0.40D)")

            client.PublishPriceChange("Silver", 6.6, -0.4)
            Console.WriteLine()

            Console.WriteLine("Press ENTER to shut down data source")
            Console.ReadLine()

            'Closing the client gracefully closes the connection and cleans up resources
            client.Close()

        End Sub

        Public Sub PriceChange(ByVal item As String, ByVal price As Double, ByVal change As Double) Implements ISampleContractCallback.PriceChange

            Console.WriteLine("PriceChange(item {0}, price {1}, change {2})", item, price.ToString("C"), change)

        End Sub

    End Class

End Namespace

