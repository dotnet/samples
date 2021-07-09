' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Description

Namespace Microsoft.Samples.ServiceModel

    Public Class EndpointValidateBehavior
        Implements IServiceBehavior

        Private secureElementFound As Boolean

        Public Sub AddBindingParameters(ByVal serviceDescription As ServiceDescription, ByVal serviceHostBase As ServiceHostBase, ByVal endpoints As System.Collections.ObjectModel.Collection(Of ServiceEndpoint), ByVal bindingParameters As System.ServiceModel.Channels.BindingParameterCollection) Implements IServiceBehavior.AddBindingParameters

        End Sub

        Public Sub ApplyDispatchBehavior(ByVal serviceDescription As ServiceDescription, ByVal serviceHostBase As ServiceHostBase) Implements IServiceBehavior.ApplyDispatchBehavior

        End Sub

        ' The validation process will scan each endpoint to see if it's bindings have binding elements
        ' that are secure. These elements consist of: Transport, Asymmetric, Symmetric,
        ' HttpsTransport, WindowsStream and SSLStream.
        Public Sub Validate(ByVal serviceDescription As ServiceDescription, ByVal serviceHostBase As ServiceHostBase) Implements IServiceBehavior.Validate

            ' Loop through each endpoint individually gathering their binding elements.
            For Each endpoint As ServiceEndpoint In serviceDescription.Endpoints

                secureElementFound = False

                ' Retrieve the endpoint's binding element collection.
                Dim bindingElements As BindingElementCollection = endpoint.Binding.CreateBindingElements()

                ' Look to see if the binding elements collection contains any secure binding
                ' elements. Transport, Asymmetric and Symmetric binding elements are all
                ' derived from SecurityBindingElement.
                If (bindingElements.Find(Of SecurityBindingElement)() IsNot Nothing) OrElse (bindingElements.Find(Of HttpsTransportBindingElement)() IsNot Nothing) OrElse (bindingElements.Find(Of WindowsStreamSecurityBindingElement)() IsNot Nothing) OrElse (bindingElements.Find(Of SslStreamSecurityBindingElement)() IsNot Nothing) Then

                    secureElementFound = True

                End If

                ' Send a message to the system event viewer whhen an endpoint is deemed insecure.
                If Not secureElementFound Then
                    Throw New Exception(System.DateTime.Now.ToString() + ": The endpoint """ + endpoint.Name + """ has no secure bindings.")
                End If

            Next

        End Sub

    End Class

End Namespace
