' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.ServiceModel
Imports System.ServiceModel.Configuration
Imports System.ServiceModel.Description
Imports System.Xml

Namespace Microsoft.Samples.ServiceModel

    Class EndpointValidateElement
        Inherits BehaviorExtensionElement

        Protected Overloads Overrides Function CreateBehavior() As Object

            Return New EndpointValidateBehavior()

        End Function

        Public Overloads Overrides ReadOnly Property BehaviorType() As Type

            Get

                Return GetType(EndpointValidateBehavior)

            End Get

        End Property

    End Class

End Namespace
