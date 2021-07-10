'  Copyright (c) Microsoft Corporation. All rights reserved.

Imports System.Configuration
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Configuration

Namespace Microsoft.ServiceModel.Samples
	''' <summary>
	''' Configuration section for Udp. 
	''' </summary>
	Public Class UdpTransportElement
		Inherits BindingElementExtensionElement
		Public Sub New()
		End Sub

        <ConfigurationProperty(UdpConfigurationStrings.MaxBufferPoolSize,
            DefaultValue:=UdpDefaults.MaxBufferPoolSize), LongValidator(MinValue:=0)> _
        Public Property MaxBufferPoolSize() As Long
            Get
                Return CLng(Fix(MyBase.Item(UdpConfigurationStrings.MaxBufferPoolSize)))
            End Get
            Set(ByVal value As Long)
                MyBase.Item(UdpConfigurationStrings.MaxBufferPoolSize) = value
            End Set
        End Property

        <ConfigurationProperty(UdpConfigurationStrings.MaxReceivedMessageSize, DefaultValue:=
            UdpDefaults.MaxReceivedMessageSize), IntegerValidator(MinValue:=1)> _
        Public Property MaxReceivedMessageSize() As Integer
            Get
                Return CInt(Fix(MyBase.Item(UdpConfigurationStrings.MaxReceivedMessageSize)))
            End Get
            Set(ByVal value As Integer)
                MyBase.Item(UdpConfigurationStrings.MaxReceivedMessageSize) = value
            End Set
        End Property

		<ConfigurationProperty(UdpConfigurationStrings.Multicast, DefaultValue := UdpDefaults.Multicast)> _
		Public Property Multicast() As Boolean
			Get
				Return CBool(MyBase.Item(UdpConfigurationStrings.Multicast))
			End Get
			Set(ByVal value As Boolean)
				MyBase.Item(UdpConfigurationStrings.Multicast) = value
			End Set
		End Property

		Public Overrides ReadOnly Property BindingElementType() As Type
			Get
				Return GetType(UdpTransportBindingElement)
			End Get
		End Property

		Protected Overrides Function CreateBindingElement() As BindingElement
			Dim bindingElement As New UdpTransportBindingElement()
			Me.ApplyConfiguration(bindingElement)
			Return bindingElement
		End Function

		Public Overrides Sub ApplyConfiguration(ByVal bindingElement As BindingElement)
			MyBase.ApplyConfiguration(bindingElement)

			Dim udpBindingElement As UdpTransportBindingElement = CType(bindingElement, UdpTransportBindingElement)
            With udpBindingElement
                .MaxBufferPoolSize = Me.MaxBufferPoolSize
                .MaxReceivedMessageSize = Me.MaxReceivedMessageSize
                .Multicast = Me.Multicast
            End With
        End Sub

		Public Overrides Sub CopyFrom(ByVal [from] As ServiceModelExtensionElement)
			MyBase.CopyFrom(From)

			Dim source As UdpTransportElement = CType(From, UdpTransportElement)
			Me.MaxBufferPoolSize = source.MaxBufferPoolSize
			Me.MaxReceivedMessageSize = source.MaxReceivedMessageSize
			Me.Multicast = source.Multicast
		End Sub

		Protected Overrides Sub InitializeFrom(ByVal bindingElement As BindingElement)
			MyBase.InitializeFrom(bindingElement)

			Dim udpBindingElement As UdpTransportBindingElement = CType(bindingElement, UdpTransportBindingElement)
			Me.MaxBufferPoolSize = udpBindingElement.MaxBufferPoolSize
			Me.MaxReceivedMessageSize = CInt(Fix(udpBindingElement.MaxReceivedMessageSize))
			Me.Multicast = udpBindingElement.Multicast
		End Sub

		Protected Overrides ReadOnly Property Properties() As ConfigurationPropertyCollection
			Get

                Dim properties_local As ConfigurationPropertyCollection = MyBase.Properties
                With properties_local
                    .Add(New ConfigurationProperty(UdpConfigurationStrings.MaxBufferPoolSize, GetType(Long),
                                                   UdpDefaults.MaxBufferPoolSize,
                                                   Nothing, New LongValidator(0, Int64.MaxValue),
                                                   ConfigurationPropertyOptions.None))

                    .Add(New ConfigurationProperty(UdpConfigurationStrings.MaxReceivedMessageSize, GetType(Integer),
                                                   UdpDefaults.MaxReceivedMessageSize,
                                                   Nothing, New IntegerValidator(1, Int32.MaxValue),
                                                   ConfigurationPropertyOptions.None))

                    .Add(New ConfigurationProperty(UdpConfigurationStrings.Multicast, GetType(Boolean),
                                                   UdpDefaults.Multicast, Nothing, Nothing,
                                                   ConfigurationPropertyOptions.None))
                End With
                Return properties_local
            End Get
		End Property
	End Class
End Namespace
