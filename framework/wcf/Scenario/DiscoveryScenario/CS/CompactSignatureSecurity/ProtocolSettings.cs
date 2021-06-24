//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel.Discovery;

namespace Microsoft.Samples.Discovery
{

    class ProtocolSettings
    {
        public ProtocolSettings(DiscoveryVersion discoveryVersion)
        {
            Utility.IfNullThrowNullArgumentException(discoveryVersion, "discoveryVersion");
            this.DiscoveryNamespace = discoveryVersion.Namespace;
            this.DiscoveryPrefix = ProtocolStrings.DiscoveryPrefix;

            if (discoveryVersion.Namespace == ProtocolStrings.DiscoveryNamespaceApril2005)
            {
                this.AddressingNamespace = ProtocolStrings.WsaNamespaceAugust2004;
                this.SchemeUri = ProtocolStrings.SchemeUriAugust2004;
                this.SupportsInclusivePrefixes = false;
            }
            else
            {
                this.AddressingNamespace = ProtocolStrings.WsaNamespace10;
                this.SchemeUri = ProtocolStrings.SchemeUri11;
                this.SupportsInclusivePrefixes = true;
            }
        }

        public bool SupportsInclusivePrefixes { get; private set; }

        public string DiscoveryNamespace { get; private set; }

        public string AddressingNamespace { get; private set; }

        public string DiscoveryPrefix { get; private set; }

        public string SchemeUri { get; private set; }
    }
}
