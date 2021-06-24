//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Discovery; 

namespace Microsoft.Samples.Discovery
{       
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class DiscoveryRoutingService : DiscoveryService
    {
        // The endpoint over which the DiscoveryEndpoint will forward discovery requests
        DiscoveryEndpoint forwardingDiscoveryEndpoint;

        public DiscoveryRoutingService(DiscoveryEndpoint forwardingDiscoveryEndpoint)
        {
            this.forwardingDiscoveryEndpoint = forwardingDiscoveryEndpoint;
        }

        protected override IAsyncResult OnBeginFind(FindRequestContext findRequestContext, AsyncCallback callback, object state)
        {
            Console.WriteLine("Received a Probe request message. Forwarding the Probe message.");
            
            // FindAsyncResult will forward the Probe request
            return new FindAsyncResult(findRequestContext, this.forwardingDiscoveryEndpoint, callback, state);
        }

        protected override IAsyncResult OnBeginResolve(ResolveCriteria resolveCriteria, AsyncCallback callback, object state)
        {
            Console.WriteLine("Received a Resolve request message. Forwarding the Resolve message.");

            // ResolveAsyncResult will forward the Resolve request
            return new ResolveAsyncResult(resolveCriteria, this.forwardingDiscoveryEndpoint, callback, state);
        }

        protected override void OnEndFind(IAsyncResult result)
        {
            FindAsyncResult.End(result);
        }

        protected override EndpointDiscoveryMetadata OnEndResolve(IAsyncResult result)
        {
            return ResolveAsyncResult.End(result);
        }
    }
}
