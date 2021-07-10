'  Copyright (c) Microsoft Corporation. All rights reserved.

Imports System.ServiceModel
Imports System.ServiceModel.Dispatcher
Imports System.Transactions

Namespace Microsoft.ServiceModel.Samples
	Friend Class TransactionFlowInspector
		Implements IClientMessageInspector
        Public Sub AfterReceiveReply(ByRef reply As System.ServiceModel.Channels.Message,
                                     ByVal correlationState As Object) Implements IClientMessageInspector.AfterReceiveReply
        End Sub

        Public Function BeforeSendRequest(ByRef request As System.ServiceModel.Channels.Message,
                                          ByVal channel As System.ServiceModel.IClientChannel) As Object Implements IClientMessageInspector.BeforeSendRequest
            ' obtain the tx propagation token
            Dim propToken() As Byte = Nothing
            If Transaction.Current IsNot Nothing AndAlso IsTxFlowRequiredForThisOperation(request.Headers.Action) Then
                Try
                    propToken = TransactionInterop.GetTransmitterPropagationToken(Transaction.Current)
                Catch e As TransactionException
                    Throw New CommunicationException("TransactionInterop.GetTransmitterPropagationToken failed.", e)
                End Try
            End If

            ' set the propToken on the message in a TransactionFlowProperty
            TransactionFlowProperty.Set(propToken, request)

            Return Nothing
        End Function

		Private Shared Function IsTxFlowRequiredForThisOperation(ByVal action As String) As Boolean
			' In general, this should contain logic to identify which operations (actions) require transaction flow.
			' Here we just flow transactions for all actions.
			Return True
		End Function
	End Class
End Namespace
