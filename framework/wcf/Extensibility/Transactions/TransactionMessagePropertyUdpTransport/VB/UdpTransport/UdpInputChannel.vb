'  Copyright (c) Microsoft Corporation. All rights reserved.

Imports System.ServiceModel
Imports System.ServiceModel.Channels

Namespace Microsoft.ServiceModel.Samples
	Friend Class UdpInputChannel
		Inherits ChannelBase
		Implements IInputChannel
		Private encoder As MessageEncoder
		Private messageQueue As InputQueue(Of Message)

		Friend Sub New(ByVal listener As UdpChannelListener)
			MyBase.New(listener)
			Me.encoder = listener.MessageEncoderFactory.Encoder
			Me.messageQueue = New InputQueue(Of Message)()
		End Sub

		Public ReadOnly Property LocalAddress() As EndpointAddress Implements IInputChannel.LocalAddress
			Get
				Return Nothing
			End Get
		End Property

		'Hands the message off to other components higher up the
		'channel stack that have previously called BeginReceive() 
		'and are waiting for messages to arrive on this channel.
		Friend Sub Dispatch(ByVal message As Message)
			Me.messageQueue.EnqueueAndDispatch(message)
		End Sub

       

		'Closes the channel ungracefully during error conditions.
		Protected Overrides Sub OnAbort()
			Me.messageQueue.Close()
		End Sub

		'Closes the channel gracefully during normal conditions.
		Protected Overrides Sub OnClose(ByVal timeout As TimeSpan)
			Me.messageQueue.Close()
		End Sub

        Protected Overrides Function OnBeginClose(ByVal timeout As TimeSpan, ByVal callback As AsyncCallback,
                                                  ByVal state As Object) As IAsyncResult
            Me.OnClose(timeout)
            Return New CompletedAsyncResult(callback, state)
        End Function

		Protected Overrides Sub OnEndClose(ByVal result As IAsyncResult)
			CompletedAsyncResult.End(result)
		End Sub

		Protected Overrides Sub OnOpen(ByVal timeout As TimeSpan)
		End Sub

        Protected Overrides Function OnBeginOpen(ByVal timeout As TimeSpan, ByVal callback As AsyncCallback,
                                                 ByVal state As Object) As IAsyncResult
            Return New CompletedAsyncResult(callback, state)
        End Function

		Protected Overrides Sub OnEndOpen(ByVal result As IAsyncResult)
			CompletedAsyncResult.End(result)
		End Sub

		Public Function Receive() As Message Implements IInputChannel.Receive
			Return Me.Receive(Me.DefaultReceiveTimeout)
		End Function

		Public Function Receive(ByVal timeout As TimeSpan) As Message Implements IInputChannel.Receive
            Dim message As Message = Nothing
			If Me.TryReceive(timeout, message) Then
				Return message
			Else
				Throw CreateReceiveTimedOutException(Me, timeout)
			End If
		End Function

        Public Function BeginReceive(ByVal callback As AsyncCallback,
                                     ByVal state As Object) As IAsyncResult Implements IInputChannel.BeginReceive
            Return Me.BeginReceive(Me.DefaultReceiveTimeout, callback, state)
        End Function

        Public Function BeginReceive(ByVal timeout As TimeSpan, ByVal callback As AsyncCallback,
                                     ByVal state As Object) As IAsyncResult Implements IInputChannel.BeginReceive
            Return Me.BeginTryReceive(timeout, callback, state)
        End Function

		Public Function EndReceive(ByVal result As IAsyncResult) As Message Implements IInputChannel.EndReceive
			Return Me.messageQueue.EndDequeue(result)
		End Function

        Public Function TryReceive(ByVal timeout As TimeSpan,
                                   <System.Runtime.InteropServices.Out()> ByRef message As Message) As Boolean Implements IInputChannel.TryReceive
            UdpChannelHelpers.ValidateTimeout(timeout)
            Return Me.messageQueue.Dequeue(timeout, message)
        End Function

        Public Function BeginTryReceive(ByVal timeout As TimeSpan,
                                        ByVal callback As AsyncCallback,
                                        ByVal state As Object) As IAsyncResult Implements IInputChannel.BeginTryReceive
            UdpChannelHelpers.ValidateTimeout(timeout)
            Return Me.messageQueue.BeginDequeue(timeout, callback, state)
        End Function

        Public Function EndTryReceive(ByVal result As IAsyncResult,
                                      <System.Runtime.InteropServices.Out()> ByRef message As Message) As Boolean Implements IInputChannel.EndTryReceive
            Return Me.messageQueue.EndDequeue(result, message)
        End Function

		Public Function WaitForMessage(ByVal timeout As TimeSpan) As Boolean Implements IInputChannel.WaitForMessage
			UdpChannelHelpers.ValidateTimeout(timeout)
			Return Me.messageQueue.WaitForItem(timeout)
		End Function

        Public Function BeginWaitForMessage(ByVal timeout As TimeSpan,
                                            ByVal callback As AsyncCallback,
                                            ByVal state As Object) As IAsyncResult Implements IInputChannel.BeginWaitForMessage
            UdpChannelHelpers.ValidateTimeout(timeout)
            Return Me.messageQueue.BeginWaitForItem(timeout, callback, state)
        End Function

		Public Function EndWaitForMessage(ByVal result As IAsyncResult) As Boolean Implements IInputChannel.EndWaitForMessage
			Return Me.messageQueue.EndWaitForItem(result)
		End Function

        Private Shared Function CreateReceiveTimedOutException(ByVal channel As IInputChannel,
                                                               ByVal timeout As TimeSpan) As TimeoutException
            If channel.LocalAddress IsNot Nothing Then
                Return New TimeoutException(String.Format("Receive on local address {0} timed out after {1}. The time allotted to this operation may have been a portion of a longer timeout.",
                                                          channel.LocalAddress.Uri.AbsoluteUri,
                                                          timeout))
            Else
                Return New TimeoutException(String.Format("Receive timed out after {0}. The time allotted to this operation may have been a portion of a longer timeout.",
                                                          timeout))
            End If
        End Function
	End Class
End Namespace
