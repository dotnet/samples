'  Copyright (c) Microsoft Corporation. All rights reserved.

Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Description
Imports System.Xml
Imports WsdlNS = System.Web.Services.Description

Namespace Microsoft.ServiceModel.Samples
	''' <summary>
	''' Policy import/export for Udp
	''' </summary>
	Public Class UdpBindingElementImporter
		Implements IPolicyImportExtension, IWsdlImportExtension
		Public Sub New()
		End Sub

        Private Sub ImportPolicy(ByVal importer As MetadataImporter,
                                 ByVal context As PolicyConversionContext) Implements IPolicyImportExtension.ImportPolicy
            If importer Is Nothing Then
                Throw New ArgumentNullException("importer")
            End If
            If context Is Nothing Then
                Throw New ArgumentNullException("context")
            End If

            Dim udpBindingElement As UdpTransportBindingElement = Nothing
            Dim multicast = False
            Dim policyAssertions As PolicyAssertionCollection = context.GetBindingAssertions()
            If policyAssertions.Remove(UdpPolicyStrings.TransportAssertion, UdpPolicyStrings.UdpNamespace) IsNot Nothing Then
                udpBindingElement = New UdpTransportBindingElement()
            End If
            If policyAssertions.Remove(UdpPolicyStrings.MulticastAssertion, UdpPolicyStrings.UdpNamespace) IsNot Nothing Then
                multicast = True
            End If
            If udpBindingElement IsNot Nothing Then
                udpBindingElement.Multicast = multicast
                context.BindingElements.Add(udpBindingElement)
            End If
        End Sub

        Public Sub BeforeImport(ByVal wsdlDocuments As WsdlNS.ServiceDescriptionCollection,
                                ByVal xmlSchemas As System.Xml.Schema.XmlSchemaSet,
                                ByVal policy As ICollection(Of XmlElement)) Implements IWsdlImportExtension.BeforeImport
        End Sub

        Public Sub ImportContract(ByVal importer As WsdlImporter,
                                  ByVal context As WsdlContractConversionContext) Implements IWsdlImportExtension.ImportContract
        End Sub

        Public Sub ImportEndpoint(ByVal importer As WsdlImporter,
                                  ByVal context As WsdlEndpointConversionContext) Implements IWsdlImportExtension.ImportEndpoint
            If context Is Nothing Then
                Throw New ArgumentNullException("context")
            End If

            If context.Endpoint.Binding Is Nothing Then
                Throw New ArgumentNullException("context.Endpoint.Binding")
            End If

            Dim bindingElements As BindingElementCollection = context.Endpoint.Binding.CreateBindingElements()
            Dim transportBindingElement As TransportBindingElement = bindingElements.Find(Of TransportBindingElement)()
            If TypeOf transportBindingElement Is UdpTransportBindingElement Then
                ImportAddress(context)
            End If

            If TypeOf context.Endpoint.Binding Is CustomBinding Then
                Dim binding As Binding = Nothing

                If TypeOf transportBindingElement Is UdpTransportBindingElement Then
                    'if TryCreate is true, the CustomBinding will be replace by a SampleProfileUdpBinding in the
                    'generated config file for better typed generation.
                    If SampleProfileUdpBinding.TryCreate(bindingElements, binding) Then
                        binding.Name = context.Endpoint.Binding.Name
                        binding.Namespace = context.Endpoint.Binding.Namespace
                        context.Endpoint.Binding = binding
                    End If
                End If
            End If
        End Sub

		'this imports the address of the endpoint.
		Private Sub ImportAddress(ByVal context As WsdlEndpointConversionContext)
			Dim address As EndpointAddress = Nothing

			If context.WsdlPort IsNot Nothing Then
                Dim addressing10Element As XmlElement = context.WsdlPort.Extensions.Find("EndpointReference",
                                                                                         AddressingVersionConstants.WSAddressing10NameSpace)

                Dim addressing200408Element As XmlElement = context.WsdlPort.Extensions.Find("EndpointReference",
                                                                                             AddressingVersionConstants.WSAddressingAugust2004NameSpace)

                Dim soapAddressBinding As WsdlNS.SoapAddressBinding = CType(context.WsdlPort.Extensions.Find(GetType(WsdlNS.SoapAddressBinding)), 
                                                                            WsdlNS.SoapAddressBinding)

				If addressing10Element IsNot Nothing Then
                    address = EndpointAddress.ReadFrom(AddressingVersion.WSAddressing10,
                                                       New XmlNodeReader(addressing10Element))
				End If
				If addressing200408Element IsNot Nothing Then
                    address = EndpointAddress.ReadFrom(AddressingVersion.WSAddressingAugust2004,
                                                       New XmlNodeReader(addressing200408Element))
				ElseIf soapAddressBinding IsNot Nothing Then
					' checking for soapAddressBinding checks for both Soap 1.1 and Soap 1.2
					address = New EndpointAddress(soapAddressBinding.Location)
				End If
			End If

			If address IsNot Nothing Then
				context.Endpoint.Address = address
			End If
		End Sub
	End Class
End Namespace
