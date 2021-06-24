'  Copyright (c) Microsoft Corporation. All rights reserved.

Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports System.ServiceModel.Channels

Namespace Microsoft.ServiceModel.Samples
	''' <summary>
	''' IChannelFactory implementation for Udp.
	''' 
	''' Supports IOutputChannel only, as Udp is fundamentally
	''' a datagram protocol.
	''' </summary>
	Friend Class UdpChannelFactory
		Inherits ChannelFactoryBase(Of IOutputChannel)

        Private bufferManager_local As BufferManager

        Private messageEncoderFactory_local As MessageEncoderFactory

        Private multicast_local As Boolean

        Private maxPacketSize_local As Long

        Friend Sub New(ByVal bindingElement As UdpTransportBindingElement, ByVal context As BindingContext)
            MyBase.New(context.Binding)
            Me.multicast_local = bindingElement.Multicast
            Me.bufferManager_local = BufferManager.CreateBufferManager(bindingElement.MaxBufferPoolSize, Integer.MaxValue)

            Dim messageEncoderBindingElements As Collection(Of MessageEncodingBindingElement) =
                context.BindingParameters.FindAll(Of MessageEncodingBindingElement)()

            If messageEncoderBindingElements.Count > 1 Then
                Throw New InvalidOperationException("More than one MessageEncodingBindingElement was found in the BindingParameters of the BindingContext")
            ElseIf messageEncoderBindingElements.Count = 1 Then
                Me.messageEncoderFactory_local = messageEncoderBindingElements(0).CreateMessageEncoderFactory()
            Else
                Me.messageEncoderFactory_local = UdpConstants.DefaultMessageEncoderFactory
            End If

            Me.maxPacketSize_local = bindingElement.MaxReceivedMessageSize
        End Sub


        Public ReadOnly Property MaxPacketSize() As Long
            Get
                Return Me.maxPacketSize_local
            End Get
        End Property

        Public ReadOnly Property BufferManager() As BufferManager
            Get
                Return Me.bufferManager_local
            End Get
        End Property

        Public ReadOnly Property MessageEncoderFactory() As MessageEncoderFactory
            Get
                Return Me.messageEncoderFactory_local
            End Get
        End Property

        Public ReadOnly Property Multicast() As Boolean
            Get
                Return Me.multicast_local
            End Get
        End Property

        Public Overloads Overrides Function GetProperty(Of T As Class)() As T
            Dim messageEncoderProperty As T = Me.MessageEncoderFactory.Encoder.GetProperty(Of T)()
            If messageEncoderProperty IsNot Nothing Then
                Return messageEncoderProperty
            End If

            If GetType(T) Is GetType(MessageVersion) Then
                Return CType(CObj(Me.MessageEncoderFactory.Encoder.MessageVersion), T)
            End If

            Return MyBase.GetProperty(Of T)()
        End Function

        Protected Overloads Overrides Sub OnOpen(ByVal timeout As TimeSpan)
        End Sub

        Protected Overloads Overrides Function OnBeginOpen(ByVal timeout As TimeSpan, ByVal callback As AsyncCallback,
                                                           ByVal state As Object) As IAsyncResult
            Return New CompletedAsyncResult(callback, state)
        End Function

        Protected Overloads Overrides Sub OnEndOpen(ByVal result As IAsyncResult)
            CompletedAsyncResult.End(result)
        End Sub

        ''' <summary>
        ''' Create a new Udp Channel. Supports IOutputChannel.
        ''' </summary>
        ''' <typeparam name="TChannel">The type of Channel to create (e.g. IOutputChannel)</typeparam>
        ''' <param name="remoteAddress">The address of the remote endpoint</param>
        ''' <returns></returns>
        Protected Overrides Function OnCreateChannel(ByVal remoteAddress As EndpointAddress,
                                                     ByVal via As Uri) As IOutputChannel
            Return New UdpOutputChannel(Me, remoteAddress, via, MessageEncoderFactory.Encoder)
        End Function

        Protected Overloads Overrides Sub OnClosed()
            MyBase.OnClosed()
            Me.bufferManager_local.Clear()
        End Sub
	End Class
End Namespace
