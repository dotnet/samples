'  Copyright (c) Microsoft Corporation. All rights reserved.

Imports System.ServiceModel.Configuration

Namespace Microsoft.ServiceModel.Samples
	''' <summary>
	''' Binding Section for Udp. Implements configuration for SampleProfileUdpBinding.
	''' </summary>
	Public Class SampleProfileUdpBindingCollectionElement
		Inherits StandardBindingCollectionElement(Of SampleProfileUdpBinding, SampleProfileUdpBindingConfigurationElement)
	End Class
End Namespace
