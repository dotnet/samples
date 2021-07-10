' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.ServiceModel
Imports System.ServiceModel.Channels

Namespace Microsoft.ServiceModel.Samples

    'Utility class that acts as the header that client will add to list of incoming headers
    Public Class CustomHeader

        Public Shared ReadOnly HeaderName As String = "MessageHeaderGUID"
        Public Shared ReadOnly HeaderNamespace As String = "http://Microsoft.ServiceModel.Samples/GUID"

    End Class

    Class MessageHeaderClient

        Public Shared Sub Main(ByVal args As String())

            'Create two clients to the remote service.
            Dim client1 As New MessageHeaderReaderClient()
            Dim client2 As New MessageHeaderReaderClient()

            'Create an OperationContextScope with client1 so we can add headers.
            Using New OperationContextScope(client1.InnerChannel)

                'Create a new GUID that we will send as header.
                Dim gid As String = Guid.NewGuid().ToString()

                'Create a MessageHeader for the guid we just created.
                Dim ch As MessageHeader = MessageHeader.CreateHeader(CustomHeader.HeaderName, CustomHeader.HeaderNamespace, gid)

                'Add the header to the OutgoingMessageHeader collection.
                OperationContext.Current.OutgoingMessageHeaders.Add(ch)

                'Now call RetreieveHeader on both the proxies. Since the OperationContextScope is tied to 
                'client1's InnerChannel, the header should only be added to calls made on that client.
                'Calls made on client2 should not be sending the header across even though the call
                'is made in the same OperationContextScope.
                Console.WriteLine("Using client1 to send message")
                Console.WriteLine("Did server retrieve the header? : Actual: {0}, Expected: True", client1.RetrieveHeader(gid))

                Console.WriteLine()
                Console.WriteLine("Using client2 to send message")
                Console.WriteLine("Did server retrieve the header? : Actual: {0}, Expected: False", client2.RetrieveHeader(gid))

            End Using

            'Close the proxies.
            client1.Close()
            client2.Close()

            Console.WriteLine()
            Console.WriteLine("Press <ENTER> to terminate client.")
            Console.ReadLine()

        End Sub

    End Class

End Namespace

