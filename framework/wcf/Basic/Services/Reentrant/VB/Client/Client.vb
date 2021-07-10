' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.ServiceModel
Imports Microsoft.ServiceModel.Samples
Imports Microsoft.VisualBasic

Namespace Microsoft.ServiceModel.Samples

    '
    '      The callback implementation calls back to the service and so this also must
    '      be marked Reentrant.
    '     
    <CallbackBehavior(ConcurrencyMode:=ConcurrencyMode.Reentrant)> _
    Public Class PingPongCallback
        Implements IPingPongCallback

#Region "IPingPongCallback Members"

        Public Sub Pong(ByVal ticks As Integer) Implements IPingPongCallback.Pong

            Console.WriteLine("Pong: Ticks = " & ticks.ToString())
            If ticks <> 0 Then

                'Retrieve the Callback  Channel (in this case the Channel that was used to send the
                'original message) and make an outgoing call until ticks reaches 0.
                Dim channel As IPingPong = OperationContext.Current.GetCallbackChannel(Of IPingPong)()
                channel.Ping((ticks - 1))

            End If

        End Sub

#End Region

    End Class

    Public Class Client

        Public Shared Sub Main(ByVal args() As String)

            'Default is to PingPong between client and server twice
            Dim ticks As Integer = 10
            If (args.Length <> 0) Then
                ticks = Convert.ToInt32(args(0))
            End If

            'Create a PingPong client
            Dim pingPongClient As New PingPongClient(New InstanceContext(New PingPongCallback()))
            pingPongClient.Open()
            pingPongClient.Ping(ticks)
            pingPongClient.Close()

            Console.WriteLine()
            Console.WriteLine("Press <ENTER> to terminate client.")
            Console.ReadLine()

        End Sub

    End Class

End Namespace

