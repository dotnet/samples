//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel.Discovery;

namespace Microsoft.Samples.Discovery
{    
    // Add the custom discovery service implementation through extensions
    class CustomDiscoveryExtension : DiscoveryServiceExtension
    {
        protected override DiscoveryService GetDiscoveryService()
        {
            return new CustomDiscoveryService(this.PublishedEndpoints);
        }
    }
}
