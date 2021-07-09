' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.ServiceModel

Namespace Microsoft.ServiceModel.Samples
    '
    '      PingPong Service which demonstrated Client and Service calling each other
    '      with a Tick count till the tick count reaches 0.
    '     
    <ServiceContract(CallbackContract:=GetType(IPingPongCallback))> _
    Public Interface IPingPong

        <OperationContract()> _
        Sub Ping(ByVal ticks As Integer)

    End Interface

    Public Interface IPingPongCallback

        <OperationContract()> _
        Sub Pong(ByVal ticks As Integer)

    End Interface

    <ServiceBehavior(ConcurrencyMode:=ConcurrencyMode.Reentrant, InstanceContextMode:=InstanceContextMode.PerSession)> _
    Class PingPong
        Implements IPingPong

        Public Shared Sub Main(ByVal args As String())

            Using serviceHost As New ServiceHost(GetType(PingPong))

                ' Open the ServiceHost to create listeners and start listening for messages.
                serviceHost.Open()

                ' The service can now be accessed.
                Console.WriteLine("The service is ready.")
                Console.WriteLine("Press <ENTER> to terminate service.")
                Console.WriteLine()
                Console.ReadLine()

                ' Close the ServiceHost to shutdown the service.
                serviceHost.Close()

            End Using

        End Sub

#Region "IPingPong Members"

        Public Sub Ping(ByVal ticks As Integer) Implements IPingPong.Ping

            Console.WriteLine("Ping: Ticks = " & ticks.ToString())
            'Keep pinging back and forth till Ticks reaches 0.
            If ticks <> 0 Then
                OperationContext.Current.GetCallbackChannel(Of IPingPongCallback)().Pong((ticks - 1))
            End If

        End Sub

#End Region

    End Class

End Namespace

