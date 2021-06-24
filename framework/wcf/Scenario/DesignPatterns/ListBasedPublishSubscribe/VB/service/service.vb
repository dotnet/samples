' Copyright (c) Microsoft Corporation.  All Rights Reserved.

' This WCF sample implements the List-based Publish-Subscribe Design Pattern.

Imports System
Imports System.ServiceModel
Imports System.Diagnostics

Namespace Microsoft.ServiceModel.Samples

    ' Create a service contract and define the service operations.
    ' NOTE: The service operations must be declared explicitly.
    <ServiceContract([Namespace]:="http://Microsoft.ServiceModel.Samples", SessionMode:=SessionMode.Required, CallbackContract:=GetType(ISampleClientContract))> _
    Public Interface ISampleContract

        <OperationContract(IsOneWay:=False, IsInitiating:=True)> _
        Sub Subscribe()
        <OperationContract(IsOneWay:=False, IsTerminating:=True)> _
        Sub Unsubscribe()
        <OperationContract(IsOneWay:=True)> _
        Sub PublishPriceChange(ByVal item As String, ByVal price As Double, ByVal change As Double)

    End Interface

    Public Interface ISampleClientContract

        <OperationContract(IsOneWay:=True)> _
        Sub PriceChange(ByVal item As String, ByVal price As Double, ByVal change As Double)

    End Interface

    Public Class PriceChangeEventArgs
        Inherits EventArgs

        Public Item As String
        Public Price As Double
        Public Change As Double

    End Class

    ' The Service implementation implements your service contract.
    <ServiceBehavior(InstanceContextMode:=InstanceContextMode.PerSession)> _
    Public Class SampleService
        Implements ISampleContract

        Public Shared Event PriceChangeEvent As PriceChangeEventHandler
        Public Delegate Sub PriceChangeEventHandler(ByVal sender As Object, ByVal e As PriceChangeEventArgs)

        Private callback As ISampleClientContract = Nothing

        'Clients call this service operation to subscribe.
        'A price change event handler is registered for this client instance.
        Public Sub Subscribe() Implements ISampleContract.Subscribe

            callback = OperationContext.Current.GetCallbackChannel(Of ISampleClientContract)()
            AddHandler PriceChangeEvent, AddressOf PriceChangeHandler

        End Sub

        'Clients call this service operation to unsubscribe.
        'The previous price change event handler is deregistered.
        Public Sub Unsubscribe() Implements ISampleContract.Unsubscribe

            RemoveHandler PriceChangeEvent, AddressOf PriceChangeHandler

        End Sub

        'Information source clients call this service operation to report a price change.
        'A price change event is raised. The price change event handlers for each subscriber will execute.
        Public Sub PublishPriceChange(ByVal item As String, ByVal price As Double, ByVal change As Double) Implements ISampleContract.PublishPriceChange

            Dim e As New PriceChangeEventArgs()
            e.Item = item
            e.Price = price
            e.Change = change
            RaiseEvent PriceChangeEvent(Me, e)

        End Sub

        'This event handler runs when a PriceChange event is raised.
        'The client's PriceChange service operation is invoked to provide notification about the price change.
        Public Sub PriceChangeHandler(ByVal sender As Object, ByVal e As PriceChangeEventArgs)

            callback.PriceChange(e.Item, e.Price, e.Change)

        End Sub

    End Class

End Namespace

