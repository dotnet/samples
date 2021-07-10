//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel.Configuration;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.Udp
{

    /// <summary>
    /// Binding Section for Udp. Implements configuration for SampleProfileUdpBinding.
    /// </summary>
    public class SampleProfileUdpBindingCollectionElement : StandardBindingCollectionElement<SampleProfileUdpBinding, SampleProfileUdpBindingConfigurationElement>
    {
    }
}
