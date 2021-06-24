'  Copyright (c) Microsoft Corporation. All rights reserved.

Imports System.Security.Permissions
Imports System.ServiceModel.Description

Namespace Microsoft.ServiceModel.Samples
	Public Class TransactionFlowBehavior
		Implements IEndpointBehavior
        Public Sub AddBindingParameters(ByVal endpoint As ServiceEndpoint,
                                        ByVal bindingParameters As System.ServiceModel.Channels.BindingParameterCollection) Implements IEndpointBehavior.AddBindingParameters
        End Sub

        Public Sub ApplyClientBehavior(ByVal endpoint As ServiceEndpoint,
                                       ByVal clientRuntime As System.ServiceModel.Dispatcher.ClientRuntime) Implements IEndpointBehavior.ApplyClientBehavior
            Dim inspector As New TransactionFlowInspector()
            clientRuntime.MessageInspectors.Add(inspector)
        End Sub

        Public Sub ApplyDispatchBehavior(ByVal endpoint As ServiceEndpoint,
                                         ByVal endpointDispatcher As System.ServiceModel.Dispatcher.EndpointDispatcher) Implements IEndpointBehavior.ApplyDispatchBehavior
        End Sub

		Public Sub Validate(ByVal endpoint As ServiceEndpoint) Implements IEndpointBehavior.Validate
		End Sub
	End Class
End Namespace
