' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System

Imports System.ServiceModel
Imports System.ServiceModel.Activation

Namespace Microsoft.Samples.Federation

    Public Class HomeRealmSTSHostFactory
        Inherits ServiceHostFactoryBase

        Public Overloads Overrides Function CreateServiceHost(ByVal constructorString As String, ByVal baseAddresses() As Uri) As ServiceHostBase

            Return New HomeRealmSTSHost(baseAddresses)

        End Function

    End Class

    Class HomeRealmSTSHost
        Inherits ServiceHost

        Public Sub New(ByVal ParamArray addresses() As Uri)

            MyBase.New(GetType(HomeRealmSTS), addresses)
            ServiceConstants.LoadAppSettings()

        End Sub

    End Class

End Namespace

