//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel.Discovery;

namespace Microsoft.Samples.Discovery
{    
    class ResolveAsyncResult : AsyncResult
    {
        ResolveCriteria resolveCriteria;
        DiscoveryClient discoveryClient;

        EndpointDiscoveryMetadata endpointDiscoveryMetadata;

        public ResolveAsyncResult(ResolveCriteria resolveCriteria, DiscoveryEndpoint forwardingDiscoveryEndpoint, AsyncCallback callback, object state)
            : base(callback, state)
        {
            this.resolveCriteria = resolveCriteria;

            this.discoveryClient = new DiscoveryClient(forwardingDiscoveryEndpoint);
            this.discoveryClient.ResolveCompleted += new EventHandler<ResolveCompletedEventArgs>(ResolveCompleted);

            // Forwards the Resolve request message
            this.discoveryClient.ResolveAsync(resolveCriteria);
        }

        void ResolveCompleted(object sender, ResolveCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw e.Error;
            }

            try
            {
                this.endpointDiscoveryMetadata = e.Result.EndpointDiscoveryMetadata;
            }
            catch (Exception exception)
            {
                this.Complete(false, exception);
            }            
        }

        public static EndpointDiscoveryMetadata End(IAsyncResult result)
        {
            ResolveAsyncResult thisPtr = AsyncResult.End<ResolveAsyncResult>(result);
            thisPtr.discoveryClient.Close();

            return thisPtr.endpointDiscoveryMetadata;
        }
    }
}
