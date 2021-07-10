' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.ServiceModel
Imports System.ServiceModel.Channels

Namespace Microsoft.ServiceModel.Samples

    ' Define a service contract.
    <ServiceContract([Namespace]:="http://Microsoft.ServiceModel.Samples", SessionMode:=SessionMode.Allowed)> _
    Public Interface IMessageHeaderReader

        <OperationContract()> _
        Function RetrieveHeader(ByVal guid As String) As Boolean

    End Interface

    'Utility class that acts as the header that client will add to list of incoming headers
    Public Class CustomHeader

        Public Shared ReadOnly HeaderName As String = "MessageHeaderGUID"
        Public Shared ReadOnly HeaderNamespace As String = "http://Microsoft.ServiceModel.Samples/GUID"

    End Class

    'Use a singleton so that its the same Instance that will determine whether the header is present or not
    'for all incoming channels
    <ServiceBehavior(InstanceContextMode:=InstanceContextMode.[Single], ConcurrencyMode:=ConcurrencyMode.[Single])> _
    Public Class MessageHeaderReader
        Implements IMessageHeaderReader

#Region "IMessageHeaderService Members"

        'This method will try to retrieve a header whose value matches the GUID passed by the client.
        'The return value will notify the client whether the service was able to retrieve the header or not.
        Public Function RetrieveHeader(ByVal guid As String) As Boolean Implements IMessageHeaderReader.RetrieveHeader

            Dim messageHeaderCollection As MessageHeaders = OperationContext.Current.IncomingMessageHeaders
            Dim guidHeader As String = Nothing

            Console.WriteLine("Trying to check if IncomingMessageHeader collection contains header with value {0}", guid)
            If messageHeaderCollection.FindHeader(CustomHeader.HeaderName, CustomHeader.HeaderNamespace) <> -1 Then

                guidHeader = messageHeaderCollection.GetHeader(Of String)(CustomHeader.HeaderName, CustomHeader.HeaderNamespace)

            Else

                Console.WriteLine("No header was found")

            End If

            If guidHeader IsNot Nothing Then

                Console.WriteLine("Found header with value {0}. Does it match with GUID sent as parameter: {1}", guidHeader, guidHeader.Equals(guid))

            End If

            Console.WriteLine()
            'Return true if header is present and equals the guid sent by client as argument
            Return (guidHeader IsNot Nothing AndAlso guidHeader.Equals(guid))

        End Function

#End Region

    End Class

    Public Class SampleServiceHost

        Public Shared Sub Main(ByVal args As String())

            ' Create a ServiceHost for the MessageHeaderReader service type.
            Using serviceHost As New ServiceHost(GetType(MessageHeaderReader))

                ' Open the ServiceHost to create listeners and start listening for messages.
                serviceHost.Open()

                ' The service can now be accessed.
                Console.WriteLine("The service is ready.")
                Console.WriteLine("Press <ENTER> to terminate service.")
                Console.WriteLine()
                Console.ReadLine()

            End Using

        End Sub

    End Class

End Namespace
