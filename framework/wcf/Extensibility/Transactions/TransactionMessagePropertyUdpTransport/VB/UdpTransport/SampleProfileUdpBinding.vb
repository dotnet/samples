'  Copyright (c) Microsoft Corporation. All rights reserved.

Imports System.Configuration
Imports System.ServiceModel
Imports System.ServiceModel.Channels

Namespace Microsoft.ServiceModel.Samples
	''' <summary>
	''' Binding for Udp. This is our "sample profile" for Udp, which uses Text+Soap 1.2 
	''' and allows for variation in Reliability capabilities. If ReliableSessionEnabled is set
	''' then we will layer RM+CompositeDuplex on top of Udp. Otherwise we will just
	''' have Udp on our stack.  
	''' </summary>
	Public Class SampleProfileUdpBinding
		Inherits Binding

        Private reliableSessionEnabled_local As Boolean

        ' private BindingElements
        Private compositeDuplex As CompositeDuplexBindingElement
        Private session As ReliableSessionBindingElement
        Private transport As UdpTransportBindingElement
        Private encoding As MessageEncodingBindingElement

        Public Sub New()
            Initialize()
        End Sub

        Public Sub New(ByVal reliableSessionEnabled As Boolean)
            Me.New()
            Me.ReliableSessionEnabled = reliableSessionEnabled
        End Sub

        Public Sub New(ByVal configurationName As String)
            Me.New()
            ApplyConfiguration(configurationName)
        End Sub

        Public Property OrderedSession() As Boolean
            Get
                Return session.Ordered
            End Get
            Set(ByVal value As Boolean)
                session.Ordered = value
            End Set
        End Property

        Public Property ReliableSessionEnabled() As Boolean
            Get
                Return reliableSessionEnabled_local
            End Get
            Set(ByVal value As Boolean)
                reliableSessionEnabled_local = value
            End Set
        End Property

		Public Overrides ReadOnly Property Scheme() As String
			Get
				Return "soap.udp"
			End Get
		End Property

		Public Property SessionInactivityTimeout() As TimeSpan
			Get
				Return Me.session.InactivityTimeout
			End Get
			Set(ByVal value As TimeSpan)
				Me.session.InactivityTimeout = value
			End Set
		End Property

		Public ReadOnly Property SoapVersion() As EnvelopeVersion
			Get
				Return EnvelopeVersion.Soap12
			End Get
		End Property

		Public Property ClientBaseAddress() As Uri
			Get
				Return Me.compositeDuplex.ClientBaseAddress
			End Get
			Set(ByVal value As Uri)
				Me.compositeDuplex.ClientBaseAddress = value
			End Set
		End Property

		''' <summary>
		''' Create the set of binding elements that make up this binding. 
		''' NOTE: order of binding elements is important.
		''' </summary>
		''' <returns></returns>
		Public Overrides Function CreateBindingElements() As BindingElementCollection
			Dim bindingElements As New BindingElementCollection()

			If ReliableSessionEnabled Then
				bindingElements.Add(session)
				bindingElements.Add(compositeDuplex)
			End If

			bindingElements.Add(encoding)
			bindingElements.Add(transport)

			Return bindingElements.Clone()
		End Function

		Private Sub ApplyConfiguration(ByVal configurationName As String)
            Dim section As SampleProfileUdpBindingCollectionElement = CType(ConfigurationManager.GetSection(UdpConstants.UdpBindingSectionName), 
                                                                            SampleProfileUdpBindingCollectionElement)
			Dim element As SampleProfileUdpBindingConfigurationElement = section.Bindings(configurationName)
			If element Is Nothing Then
                Throw New ConfigurationErrorsException(String.Format(System.Globalization.CultureInfo.CurrentCulture,
                                                                     "There is no binding named {0} at {1}.",
                                                                     configurationName,
                                                                     section.BindingName))
			Else
				element.ApplyConfiguration(Me)
			End If
		End Sub

		Private Sub Initialize()
			transport = New UdpTransportBindingElement()
			session = New ReliableSessionBindingElement()
			compositeDuplex = New CompositeDuplexBindingElement()
			encoding = New TextMessageEncodingBindingElement()
		End Sub

		'initialize a SampleProfileUdpBinding from the info collected in a ReliableSessionBindingElement if one is present.
        Private Sub InitializeFrom(ByVal udpTransportBindingElement As UdpTransportBindingElement,
                                   ByVal textMessageEncodingBindingElement As TextMessageEncodingBindingElement,
                                   ByVal reliableSessionBindingElement As ReliableSessionBindingElement,
                                   ByVal compositeDuplexBindingElement As CompositeDuplexBindingElement)
            Me.transport.Multicast = udpTransportBindingElement.Multicast
            Me.transport.MaxBufferPoolSize = udpTransportBindingElement.MaxBufferPoolSize
            Me.transport.MaxReceivedMessageSize = udpTransportBindingElement.MaxReceivedMessageSize

            CType(Me.encoding, TextMessageEncodingBindingElement).WriteEncoding = textMessageEncodingBindingElement.WriteEncoding
            textMessageEncodingBindingElement.ReaderQuotas.CopyTo((CType(Me.encoding, TextMessageEncodingBindingElement)).ReaderQuotas)

            Me.ReliableSessionEnabled = reliableSessionBindingElement IsNot Nothing

            If reliableSessionBindingElement IsNot Nothing Then
                Me.SessionInactivityTimeout = reliableSessionBindingElement.InactivityTimeout
                Me.OrderedSession = reliableSessionBindingElement.Ordered
            End If

            If compositeDuplexBindingElement IsNot Nothing Then
                Me.ClientBaseAddress = compositeDuplexBindingElement.ClientBaseAddress
            End If
        End Sub

		'try to create a SampleProfileUdpBinding from the collection of BindingElement
		'returns true if it is possible, with the resulting binding.
        Public Shared Function TryCreate(ByVal elements As BindingElementCollection,
                                         <System.Runtime.InteropServices.Out()> ByRef binding As Binding) As Boolean
            binding = Nothing
            If elements.Count > 4 Then
                Return False
            End If

            Dim reliableSessionBindingElement As ReliableSessionBindingElement = Nothing
            Dim compositeDuplexBindingElement As CompositeDuplexBindingElement = Nothing
            Dim textMessageEncodingBindingElement As TextMessageEncodingBindingElement = Nothing
            Dim udpTransportBindingElement As UdpTransportBindingElement = Nothing

            For Each element As BindingElement In elements
                If TypeOf element Is CompositeDuplexBindingElement Then
                    compositeDuplexBindingElement = TryCast(element, CompositeDuplexBindingElement)
                ElseIf TypeOf element Is TransportBindingElement Then
                    udpTransportBindingElement = TryCast(element, UdpTransportBindingElement)
                ElseIf TypeOf element Is TextMessageEncodingBindingElement Then
                    textMessageEncodingBindingElement = TryCast(element, TextMessageEncodingBindingElement)
                ElseIf TypeOf element Is ReliableSessionBindingElement Then
                    reliableSessionBindingElement = TryCast(element, ReliableSessionBindingElement)
                Else
                    Return False
                End If
            Next element

            If udpTransportBindingElement Is Nothing Then
                Return False
            End If
            If textMessageEncodingBindingElement Is Nothing Then
                Return False
            End If

            If ((reliableSessionBindingElement IsNot Nothing) AndAlso
                (compositeDuplexBindingElement Is Nothing)) OrElse
            ((reliableSessionBindingElement Is Nothing) AndAlso
             (compositeDuplexBindingElement IsNot Nothing)) Then
                Return False
            End If

            Dim sampleProfileUdpBinding As New SampleProfileUdpBinding()
            sampleProfileUdpBinding.InitializeFrom(udpTransportBindingElement, textMessageEncodingBindingElement,
                                                   reliableSessionBindingElement, compositeDuplexBindingElement)
            If Not sampleProfileUdpBinding.IsBindingElementsMatch(udpTransportBindingElement,
                                                                  textMessageEncodingBindingElement,
                                                                  reliableSessionBindingElement,
                                                                  compositeDuplexBindingElement) Then
                Return False
            End If

            binding = sampleProfileUdpBinding
            Return True
        End Function

        Private Function IsBindingElementsMatch(ByVal udpTransportBindingElement As UdpTransportBindingElement,
                                                ByVal textMessageEncodingBindingElement As TextMessageEncodingBindingElement,
                                                ByVal reliableSessionBindingElement As ReliableSessionBindingElement,
                                                ByVal compositeDuplexBindingElement As CompositeDuplexBindingElement) As Boolean
            If Not IsTransportMatch(Me.transport, udpTransportBindingElement) Then
                Return False
            End If

            If Not IsEncodingMatch(Me.encoding, textMessageEncodingBindingElement) Then
                Return False
            End If

            If Me.ReliableSessionEnabled Then
                If Not IsSessionMatch(Me.session, reliableSessionBindingElement) Then
                    Return False
                End If
                If compositeDuplexBindingElement IsNot Nothing Then
                    If Not IsCompositeDuplexMatch(Me.compositeDuplex, compositeDuplexBindingElement) Then
                        Return False
                    End If
                Else
                    Return False
                End If
            ElseIf reliableSessionBindingElement IsNot Nothing Then
                Return False
            End If

            Return True
        End Function

		Private Function IsTransportMatch(ByVal a As BindingElement, ByVal b As BindingElement) As Boolean
			If b Is Nothing Then
				Return False
			End If

			Dim transportA As UdpTransportBindingElement = TryCast(a, UdpTransportBindingElement)
			Dim transportB As UdpTransportBindingElement = TryCast(b, UdpTransportBindingElement)

			If transportB Is Nothing Then
				Return False
			End If
			If transportA.MaxBufferPoolSize <> transportB.MaxBufferPoolSize Then
				Return False
			End If
			If transportA.MaxReceivedMessageSize <> transportB.MaxReceivedMessageSize Then
				Return False
			End If
			If transportA.Multicast <> transportB.Multicast Then
				Return False
			End If

			Return True
		End Function

		Private Function IsEncodingMatch(ByVal a As BindingElement, ByVal b As BindingElement) As Boolean
			If b Is Nothing Then
				Return False
			End If

			Dim messageEncodingBindingElement As MessageEncodingBindingElement = TryCast(b, MessageEncodingBindingElement)
			If messageEncodingBindingElement Is Nothing Then
				Return False
			End If

			Dim textA As TextMessageEncodingBindingElement = TryCast(a, TextMessageEncodingBindingElement)
			Dim textB As TextMessageEncodingBindingElement = TryCast(b, TextMessageEncodingBindingElement)
			If textB Is Nothing Then
				Return False
			End If
			If textA.MaxReadPoolSize <> textB.MaxReadPoolSize Then
				Return False
			End If
			If textA.MaxWritePoolSize <> textB.MaxWritePoolSize Then
				Return False
			End If

			' compare XmlDictionaryReaderQuotas
			If textA.ReaderQuotas.MaxStringContentLength <> textB.ReaderQuotas.MaxStringContentLength Then
				Return False
			End If
			If textA.ReaderQuotas.MaxArrayLength <> textB.ReaderQuotas.MaxArrayLength Then
				Return False
			End If
			If textA.ReaderQuotas.MaxBytesPerRead <> textB.ReaderQuotas.MaxBytesPerRead Then
				Return False
			End If
			If textA.ReaderQuotas.MaxDepth <> textB.ReaderQuotas.MaxDepth Then
				Return False
			End If
			If textA.ReaderQuotas.MaxNameTableCharCount <> textB.ReaderQuotas.MaxNameTableCharCount Then
				Return False
			End If

			If textA.WriteEncoding.EncodingName <> textB.WriteEncoding.EncodingName Then
				Return False
			End If
			If Not IsMessageVersionMatch(textA.MessageVersion, textB.MessageVersion) Then
				Return False
			End If

			Return True
		End Function

		Private Function IsMessageVersionMatch(ByVal a As MessageVersion, ByVal b As MessageVersion) As Boolean
			If b Is Nothing Then
				Throw New ArgumentNullException("b")
			End If
			If a.Addressing Is Nothing Then
				Throw New InvalidOperationException("MessageVersion.Addressing cannot be null")
			End If

            If a.Envelope IsNot b.Envelope Then
                Return False
            End If
			'if (a.Addressing.Namespace != b.Addressing.Namespace)
			'{
			'    return false;
			'}

			Return True
		End Function

		Private Function IsSessionMatch(ByVal a As BindingElement, ByVal b As BindingElement) As Boolean
			If b Is Nothing Then
				Return False
			End If

			Dim sessionA As ReliableSessionBindingElement = TryCast(a, ReliableSessionBindingElement)
			Dim sessionB As ReliableSessionBindingElement = TryCast(b, ReliableSessionBindingElement)

			If sessionB Is Nothing Then
				Return False
			End If
			If sessionA.AcknowledgementInterval <> sessionB.AcknowledgementInterval Then
				Return False
			End If
			If sessionA.FlowControlEnabled <> sessionB.FlowControlEnabled Then
				Return False
			End If
			If sessionA.InactivityTimeout <> sessionB.InactivityTimeout Then
				Return False
			End If
			If sessionA.MaxPendingChannels <> sessionB.MaxPendingChannels Then
				Return False
			End If
			If sessionA.MaxRetryCount <> sessionB.MaxRetryCount Then
				Return False
			End If
			If sessionA.MaxTransferWindowSize <> sessionB.MaxTransferWindowSize Then
				Return False
			End If
			If sessionA.Ordered <> sessionB.Ordered Then
				Return False
			End If

			Return True
		End Function

		Private Function IsCompositeDuplexMatch(ByVal a As BindingElement, ByVal b As BindingElement) As Boolean
			If b Is Nothing Then
				Return False
			End If

			Dim duplexA As CompositeDuplexBindingElement = TryCast(a, CompositeDuplexBindingElement)
			Dim duplexB As CompositeDuplexBindingElement = TryCast(b, CompositeDuplexBindingElement)
			If duplexB Is Nothing Then
				Return False
			End If

			Return (duplexB.ClientBaseAddress Is duplexA.ClientBaseAddress)
		End Function
	End Class
End Namespace
