'  Copyright (c) Microsoft Corporation. All rights reserved.

Imports System.ServiceModel
Imports System.ServiceModel.Channels

Namespace Microsoft.ServiceModel.Samples
	Public Class TransactionFlowProperty
        Public Const PropertyName = "TransactionFlowProperty"
		Private propToken() As Byte

		Private Sub New()
		End Sub

		Public Shared Function [Get](ByVal message As Message) As Byte()
			If message Is Nothing Then
				Return Nothing
			End If

			If message.Properties.ContainsKey(PropertyName) Then
				Dim tfp As TransactionFlowProperty = CType(message.Properties(PropertyName), TransactionFlowProperty)
				Return tfp.propToken
			End If

			Return Nothing
		End Function

		Public Shared Sub [Set](ByVal propToken() As Byte, ByVal message As Message)
			If message.Properties.ContainsKey(PropertyName) Then
				Throw New CommunicationException("A transaction flow property is already set on the message.")
			End If

			Dim [property] As New TransactionFlowProperty()
			[property].propToken = propToken
			message.Properties.Add(PropertyName, [property])
		End Sub
	End Class
End Namespace
