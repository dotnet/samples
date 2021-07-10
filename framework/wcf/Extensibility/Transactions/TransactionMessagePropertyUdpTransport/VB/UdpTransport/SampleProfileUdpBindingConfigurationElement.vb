'  Copyright (c) Microsoft Corporation. All rights reserved.

Imports System.Configuration
Imports System.Globalization
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Configuration

Namespace Microsoft.ServiceModel.Samples
	Public Class SampleProfileUdpBindingConfigurationElement
		Inherits StandardBindingElement
		Public Sub New(ByVal configurationName As String)
			MyBase.New(configurationName)
		End Sub

		Public Sub New()
			Me.New(Nothing)
		End Sub

		Protected Overrides ReadOnly Property BindingElementType() As Type
			Get
				Return GetType(SampleProfileUdpBinding)
			End Get
		End Property

		<ConfigurationProperty(UdpConfigurationStrings.OrderedSession, DefaultValue := UdpDefaults.OrderedSession)> _
		Public Property OrderedSession() As Boolean
			Get
				Return CBool(MyBase.Item(UdpConfigurationStrings.OrderedSession))
			End Get
			Set(ByVal value As Boolean)
				MyBase.Item(UdpConfigurationStrings.OrderedSession) = value
			End Set
		End Property

        <ConfigurationProperty(UdpConfigurationStrings.ReliableSessionEnabled,
            DefaultValue:=UdpDefaults.ReliableSessionEnabled)> _
        Public Property ReliableSessionEnabled() As Boolean
            Get
                Return CBool(MyBase.Item(UdpConfigurationStrings.ReliableSessionEnabled))
            End Get
            Set(ByVal value As Boolean)
                MyBase.Item(UdpConfigurationStrings.ReliableSessionEnabled) = value
            End Set
        End Property

        <ConfigurationProperty(UdpConfigurationStrings.SessionInactivityTimeout,
            DefaultValue:=UdpDefaults.SessionInactivityTimeoutString),
        TimeSpanValidator(MinValueString:="00:00:00")> _
        Public Property SessionInactivityTimeout() As TimeSpan
            Get
                Return CType(MyBase.Item(UdpConfigurationStrings.SessionInactivityTimeout), TimeSpan)
            End Get
            Set(ByVal value As TimeSpan)
                MyBase.Item(UdpConfigurationStrings.SessionInactivityTimeout) = value
            End Set
        End Property

		<ConfigurationProperty(UdpConfigurationStrings.ClientBaseAddress, DefaultValue := Nothing)> _
		Public Property ClientBaseAddress() As Uri
			Get
				Return CType(MyBase.Item(UdpConfigurationStrings.ClientBaseAddress), Uri)
			End Get
			Set(ByVal value As Uri)
				MyBase.Item(UdpConfigurationStrings.ClientBaseAddress) = value
			End Set
		End Property

		Protected Overrides ReadOnly Property Properties() As ConfigurationPropertyCollection
			Get

                Dim properties_local As ConfigurationPropertyCollection = MyBase.Properties
                With properties_local
                    .Add(New ConfigurationProperty(UdpConfigurationStrings.OrderedSession, GetType(Boolean),
                                                   UdpDefaults.OrderedSession, Nothing, Nothing,
                                                   ConfigurationPropertyOptions.None))
                    .Add(New ConfigurationProperty(UdpConfigurationStrings.ReliableSessionEnabled,
                                                   GetType(Boolean), UdpDefaults.ReliableSessionEnabled,
                                                   Nothing, Nothing, ConfigurationPropertyOptions.None))
                    .Add(New ConfigurationProperty(UdpConfigurationStrings.SessionInactivityTimeout,
                                                   GetType(TimeSpan), TimeSpan.Parse(UdpDefaults.SessionInactivityTimeoutString),
                                                   Nothing, New TimeSpanValidator(TimeSpan.Parse("00:00:00"),
                                                                                  TimeSpan.Parse("10675199.02:48:05.4775807"), False),
                                                                              ConfigurationPropertyOptions.None))
                    .Add(New ConfigurationProperty(UdpConfigurationStrings.ClientBaseAddress, GetType(Uri), Nothing,
                                                   Nothing, Nothing,
                                                   ConfigurationPropertyOptions.None))
                End With
                Return properties_local
            End Get
		End Property

		Protected Overrides Sub InitializeFrom(ByVal binding As Binding)
			MyBase.InitializeFrom(binding)
			Dim udpBinding As SampleProfileUdpBinding = CType(binding, SampleProfileUdpBinding)

			Me.OrderedSession = udpBinding.OrderedSession
			Me.ReliableSessionEnabled = udpBinding.ReliableSessionEnabled
			Me.SessionInactivityTimeout = udpBinding.SessionInactivityTimeout
			If udpBinding.ClientBaseAddress IsNot Nothing Then
				Me.ClientBaseAddress = udpBinding.ClientBaseAddress
			End If
		End Sub

		Protected Overrides Sub OnApplyConfiguration(ByVal binding As Binding)
			If binding Is Nothing Then
				Throw New ArgumentNullException("binding")
			End If

			If binding.GetType() IsNot GetType(SampleProfileUdpBinding) Then
                Throw New ArgumentException(String.Format(CultureInfo.CurrentCulture,
                                                          "Invalid type for binding. Expected type: {0}. Type passed in: {1}.",
                                                          GetType(SampleProfileUdpBinding).AssemblyQualifiedName,
                                                          binding.GetType().AssemblyQualifiedName))
			End If
			Dim udpBinding As SampleProfileUdpBinding = CType(binding, SampleProfileUdpBinding)
            With udpBinding
                .OrderedSession = Me.OrderedSession
                .ReliableSessionEnabled = Me.ReliableSessionEnabled
                .SessionInactivityTimeout = Me.SessionInactivityTimeout
            End With
            If Me.ClientBaseAddress IsNot Nothing Then
                udpBinding.ClientBaseAddress = ClientBaseAddress
            End If
        End Sub
	End Class
End Namespace
