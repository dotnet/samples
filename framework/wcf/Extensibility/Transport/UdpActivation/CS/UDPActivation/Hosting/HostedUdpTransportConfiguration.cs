
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.


using System;
using System.ServiceModel.Activation;

namespace Microsoft.ServiceModel.Samples.Hosting
{
    class HostedUdpTransportConfiguration : HostedTransportConfiguration
    {
        public HostedUdpTransportConfiguration()
        {
        }

        public override Uri[] GetBaseAddresses(string virtualPath)
        {
            return HostedUdpTransportConfigurationImpl.Value.GetBaseAddresses(virtualPath);
        }
    }
}

