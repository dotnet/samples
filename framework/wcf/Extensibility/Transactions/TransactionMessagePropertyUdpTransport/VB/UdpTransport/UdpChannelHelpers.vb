'  Copyright (c) Microsoft Corporation. All rights reserved.

Imports System.Globalization
Imports System.Net
Imports System.Net.Sockets
Imports System.ServiceModel
Imports System.ServiceModel.Channels

Namespace Microsoft.ServiceModel.Samples
	''' <summary>
	''' Collection of constants used by the Udp Channel classes
	''' </summary>
	Friend NotInheritable Class UdpConstants
        Friend Const EventLogSourceName = "Microsoft.ServiceModel.Samples"
        Friend Const Scheme = "soap.udp"
        Friend Const UdpBindingSectionName = "system.serviceModel/bindings/sampleProfileUdpBinding"
        Friend Const UdpTransportSectionName = "udpTransport"
		Friend Const WSAETIMEDOUT As Integer = 10060

		Private Shared messageEncoderFactory As MessageEncoderFactory
		Private Sub New()
		End Sub
		Shared Sub New()
			messageEncoderFactory = New TextMessageEncodingBindingElement().CreateMessageEncoderFactory()
		End Sub

		' ensure our advertised MessageVersion matches the version we're
        ' using to serialize/deserialize data to/from the wire.
		Friend Shared ReadOnly Property MessageVersion() As MessageVersion
			Get
				Return messageEncoderFactory.MessageVersion
			End Get
		End Property

        ' we can use the same encoder for all our Udp Channels as it's free-threaded.
		Friend Shared ReadOnly Property DefaultMessageEncoderFactory() As MessageEncoderFactory
			Get
				Return messageEncoderFactory
			End Get
		End Property
	End Class

	Friend NotInheritable Class UdpConfigurationStrings
        Public Const MaxBufferPoolSize = "maxBufferPoolSize"
        Public Const MaxReceivedMessageSize = "maxMessageSize"
        Public Const Multicast = "multicast"
        Public Const OrderedSession = "orderedSession"
        Public Const ReliableSessionEnabled = "reliableSessionEnabled"
        Public Const SessionInactivityTimeout = "sessionInactivityTimeout"
        Public Const ClientBaseAddress = "clientBaseAddress"
	End Class

	Friend NotInheritable Class UdpPolicyStrings
        Public Const UdpNamespace = "http://sample.schemas.microsoft.com/policy/udp"
        Public Const Prefix = "udp"
        Public Const MulticastAssertion = "Multicast"
        Public Const TransportAssertion = "soap.udp"
	End Class

	Friend NotInheritable Class UdpChannelHelpers
		''' <summary>
		''' The Channel layer normalizes exceptions thrown by the underlying networking implementations
		''' into subclasses of CommunicationException, so that Channels can be used polymorphically from
		''' an exception handling perspective.
		''' </summary>
		Private Sub New()
		End Sub
		Friend Shared Function ConvertTransferException(ByVal socketException As SocketException) As CommunicationException
            Return New CommunicationException(String.Format(CultureInfo.CurrentCulture,
                                                            "A Udp error ({0}: {1}) occurred while transmitting data.",
                                                            socketException.ErrorCode, socketException.Message),
                                                        socketException)
		End Function

		Friend Shared Function IsInMulticastRange(ByVal address As IPAddress) As Boolean
			If address.AddressFamily = AddressFamily.InterNetwork Then
				' 224.0.0.0 through 239.255.255.255
				Dim addressBytes() As Byte = address.GetAddressBytes()
				Return ((addressBytes(0) And &HE0) = &HE0)
				'(address.Address & MulticastIPAddress.IPv4MulticastMask) == MulticastIPAddress.IPv4MulticastMask);
			Else
				Return address.IsIPv6Multicast
			End If
		End Function

		Friend Shared Sub ValidateTimeout(ByVal timeout As TimeSpan)
			If timeout < TimeSpan.Zero Then
                Throw New ArgumentOutOfRangeException("timeout", timeout,
                                                      "Timeout must be greater than or equal to TimeSpan.Zero. To disable timeout, specify TimeSpan.MaxValue.")
			End If
		End Sub
	End Class

	Friend NotInheritable Class UdpDefaults
		Friend Const MaxBufferPoolSize As Long = 64 * 1024
        Friend Const MaxReceivedMessageSize = 5 * 1024 * 1024 '64 * 1024;
        Friend Const Multicast = False
        Friend Const OrderedSession = True
        Friend Const ReliableSessionEnabled = True
        Friend Const SessionInactivityTimeoutString = "00:10:00"
	End Class

	Friend NotInheritable Class AddressingVersionConstants
        Friend Const WSAddressing10NameSpace = "http://www.w3.org/2005/08/addressing"
        Friend Const WSAddressingAugust2004NameSpace = "http://schemas.xmlsoap.org/ws/2004/08/addressing"
	End Class
End Namespace
