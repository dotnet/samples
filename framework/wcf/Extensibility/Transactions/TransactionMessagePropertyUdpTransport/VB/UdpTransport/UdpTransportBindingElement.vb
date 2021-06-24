'  Copyright (c) Microsoft Corporation. All rights reserved.

Imports System.Globalization
Imports System.IO
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Description
Imports System.Xml
Imports WsdlNS = System.Web.Services.Description

Namespace Microsoft.ServiceModel.Samples
	''' <summary>
	''' Udp Binding Element.  
	''' Used to configure and construct Udp ChannelFactories and ChannelListeners.
	''' </summary>
	Public Class UdpTransportBindingElement ' for policy export -  to signal that we're a transport
		Inherits TransportBindingElement
		Implements IPolicyExportExtension, IWsdlExportExtension

        Private multicast_local As Boolean
        Private Shared xmlDocument As XmlDocument

        Public Sub New()
            Me.multicast_local = UdpDefaults.Multicast
        End Sub

        Protected Sub New(ByVal other As UdpTransportBindingElement)
            MyBase.New(other)
            Me.multicast_local = other.multicast
        End Sub

        Public Property Multicast() As Boolean
            Get
                Return Me.multicast_local
            End Get

            Set(ByVal value As Boolean)
                Me.multicast_local = value
            End Set
        End Property

		Public Overrides Function BuildChannelFactory(Of TChannel)(ByVal context As BindingContext) As IChannelFactory(Of TChannel)
			If context Is Nothing Then
				Throw New ArgumentNullException("context")
			End If

			Return CType(CObj(New UdpChannelFactory(Me, context)), IChannelFactory(Of TChannel))
		End Function

        Public Overrides Function BuildChannelListener(Of TChannel As {System.ServiceModel.Channels.IChannel,
                                                           Class})(ByVal context As BindingContext) As IChannelListener(Of TChannel)
            If context Is Nothing Then
                Throw New ArgumentNullException("context")
            End If

            If Not Me.CanBuildChannelListener(Of TChannel)(context) Then
                Throw New ArgumentException(String.Format(CultureInfo.CurrentCulture, "Unsupported channel type: {0}.",
                                                          GetType(TChannel).Name))
            End If

            Return CType(CObj(New UdpChannelListener(Me, context)), IChannelListener(Of TChannel))
        End Function

		''' <summary>
		''' Used by higher layers to determine what types of channel factories this
		''' binding element supports. Which in this case is just IOutputChannel.
		''' </summary>
		Public Overrides Function CanBuildChannelFactory(Of TChannel)(ByVal context As BindingContext) As Boolean
			Return (GetType(TChannel) Is GetType(IOutputChannel))
		End Function

		''' <summary>
		''' Used by higher layers to determine what types of channel listeners this
		''' binding element supports. Which in this case is just IInputChannel.
		''' </summary>
        Public Overrides Function CanBuildChannelListener(Of TChannel As {System.ServiceModel.Channels.IChannel,
                                                              Class})(ByVal context As BindingContext) As Boolean
            Return (GetType(TChannel) Is GetType(IInputChannel))
        End Function

		Public Overrides ReadOnly Property Scheme() As String
			Get
				Return UdpConstants.Scheme
			End Get
		End Property

		' We expose in policy the fact that we're UDP, and whether we're multicast or not.
		' Import is done through UdpBindingElementImporter.
        Private Sub ExportPolicy(ByVal exporter As MetadataExporter,
                                 ByVal context As PolicyConversionContext) Implements IPolicyExportExtension.ExportPolicy
            If exporter Is Nothing Then
                Throw New ArgumentNullException("exporter")
            End If

            If context Is Nothing Then
                Throw New ArgumentNullException("context")
            End If

            Dim bindingAssertions As ICollection(Of XmlElement) = context.GetBindingAssertions()
            Dim xmlDocument As New XmlDocument()
            bindingAssertions.Add(xmlDocument.CreateElement(UdpPolicyStrings.Prefix, UdpPolicyStrings.TransportAssertion,
                                                            UdpPolicyStrings.UdpNamespace))

            If Multicast Then
                bindingAssertions.Add(xmlDocument.CreateElement(UdpPolicyStrings.Prefix,
                                                                UdpPolicyStrings.MulticastAssertion,
                                                                UdpPolicyStrings.UdpNamespace))
            End If

            Dim createdNew = False
            Dim encodingBindingElement As MessageEncodingBindingElement = context.BindingElements.Find(Of MessageEncodingBindingElement)()
            If encodingBindingElement Is Nothing Then
                createdNew = True
                encodingBindingElement = New TextMessageEncodingBindingElement()
            End If

            If createdNew AndAlso TypeOf encodingBindingElement Is IPolicyExportExtension Then
                CType(encodingBindingElement, IPolicyExportExtension).ExportPolicy(exporter, context)
            End If

            AddWSAddressingAssertion(context, encodingBindingElement.MessageVersion.Addressing)
        End Sub

		Public Overrides Function Clone() As BindingElement
			Return New UdpTransportBindingElement(Me)
		End Function

        Public Overrides Function GetProperty(Of T As Class)(ByVal context As BindingContext) As T
            If context Is Nothing Then
                Throw New ArgumentNullException("context")
            End If

            Return context.GetInnerProperty(Of T)()
        End Function

        Public Sub ExportContract(ByVal exporter As WsdlExporter,
                                  ByVal context As WsdlContractConversionContext) Implements IWsdlExportExtension.ExportContract
        End Sub

        Public Sub ExportEndpoint(ByVal exporter As WsdlExporter,
                                  ByVal context As WsdlEndpointConversionContext) Implements IWsdlExportExtension.ExportEndpoint
            Dim bindingElements As BindingElementCollection = context.Endpoint.Binding.CreateBindingElements()
            Dim encodingBindingElement As MessageEncodingBindingElement = bindingElements.Find(Of MessageEncodingBindingElement)()

            If encodingBindingElement Is Nothing Then
                encodingBindingElement = New TextMessageEncodingBindingElement()
            End If

            ' Set SoapBinding Transport URI
            If UdpPolicyStrings.UdpNamespace IsNot Nothing Then
                Dim soapBinding As WsdlNS.SoapBinding = GetSoapBinding(context, exporter)

                If soapBinding IsNot Nothing Then
                    soapBinding.Transport = UdpPolicyStrings.UdpNamespace
                End If
            End If

            If context.WsdlPort IsNot Nothing Then
                AddAddressToWsdlPort(context.WsdlPort, context.Endpoint.Address,
                                     encodingBindingElement.MessageVersion.Addressing)
            End If
        End Sub

        Private Shared Sub AddAddressToWsdlPort(ByVal wsdlPort As WsdlNS.Port,
                                                ByVal endpointAddress As EndpointAddress,
                                                ByVal addressing As AddressingVersion)
            If addressing Is AddressingVersion.None Then
                Return
            End If

            Dim memoryStream As New MemoryStream()
            Dim xmlWriter As XmlWriter = xmlWriter.Create(memoryStream)
            xmlWriter.WriteStartElement("temp")

            If addressing Is AddressingVersion.WSAddressing10 Then
                xmlWriter.WriteAttributeString("xmlns", "wsa10", Nothing, AddressingVersionConstants.WSAddressing10NameSpace)
            ElseIf addressing Is AddressingVersion.WSAddressingAugust2004 Then
                xmlWriter.WriteAttributeString("xmlns", "wsa", Nothing, AddressingVersionConstants.WSAddressingAugust2004NameSpace)
            Else
                Throw New InvalidOperationException("This addressing version is not supported:" & vbLf & addressing.ToString())
            End If

            endpointAddress.WriteTo(addressing, xmlWriter)
            xmlWriter.WriteEndElement()

            xmlWriter.Flush()
            memoryStream.Seek(0, SeekOrigin.Begin)

            Dim xmlReader As XmlReader = xmlReader.Create(memoryStream)
            xmlReader.MoveToContent()

            Dim endpointReference As XmlElement = CType(XmlDoc.ReadNode(xmlReader).ChildNodes(0), XmlElement)

            wsdlPort.Extensions.Add(endpointReference)
        End Sub

        Private Shared Sub AddWSAddressingAssertion(ByVal context As PolicyConversionContext,
                                                    ByVal addressing As AddressingVersion)
            Dim addressingAssertion As XmlElement = Nothing

            If addressing Is AddressingVersion.WSAddressing10 Then
                addressingAssertion = XmlDoc.CreateElement("wsaw", "UsingAddressing",
                                                           "http://www.w3.org/2006/05/addressing/wsdl")
            ElseIf addressing Is AddressingVersion.WSAddressingAugust2004 Then
                addressingAssertion = XmlDoc.CreateElement("wsap", "UsingAddressing",
                                                           AddressingVersionConstants.WSAddressingAugust2004NameSpace & "/policy")
            ElseIf addressing Is AddressingVersion.None Then
                ' do nothing
                addressingAssertion = Nothing
            Else
                Throw New InvalidOperationException("This addressing version is not supported:" & vbLf & addressing.ToString())
            End If

            If addressingAssertion IsNot Nothing Then
                context.GetBindingAssertions().Add(addressingAssertion)
            End If
        End Sub

        Private Shared Function GetSoapBinding(ByVal endpointContext As WsdlEndpointConversionContext,
                                               ByVal exporter As WsdlExporter) As WsdlNS.SoapBinding
            Dim envelopeVersion As EnvelopeVersion = Nothing
            Dim existingSoapBinding As WsdlNS.SoapBinding = Nothing
            Dim versions As Object = Nothing
            Dim SoapVersionStateKey As New Object()

            'get the soap version state
            If exporter.State.TryGetValue(SoapVersionStateKey, versions) Then
                If versions IsNot Nothing AndAlso (CType(versions, 
                                                   Dictionary(Of WsdlNS.Binding, 
                                                              EnvelopeVersion))).ContainsKey(endpointContext.WsdlBinding) Then
                    envelopeVersion = (CType(versions, 
                                       Dictionary(Of WsdlNS.Binding, 
                                                  EnvelopeVersion)))(endpointContext.WsdlBinding)
                End If
            End If

            If envelopeVersion Is envelopeVersion.None Then
                Return Nothing
            End If

            'get existing soap binding
            For Each o As Object In endpointContext.WsdlBinding.Extensions
                If TypeOf o Is WsdlNS.SoapBinding Then
                    existingSoapBinding = CType(o, WsdlNS.SoapBinding)
                End If
            Next o

            Return existingSoapBinding
        End Function

		'reflects the structure of the wsdl
		Private Shared ReadOnly Property XmlDoc() As XmlDocument
			Get
				If xmlDocument Is Nothing Then
                    Dim nameTable As New NameTable()
                    With nameTable
                        .Add("Policy")
                        .Add("All")
                        .Add("ExactlyOne")
                        .Add("PolicyURIs")
                        .Add("Id")
                        .Add("UsingAddressing")
                        .Add("UsingAddressing")
                    End With
                    xmlDocument = New XmlDocument(nameTable)
                End If
				Return xmlDocument
			End Get
		End Property
	End Class
End Namespace
