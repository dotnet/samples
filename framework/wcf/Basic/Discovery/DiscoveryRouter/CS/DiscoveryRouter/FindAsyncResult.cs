//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.ServiceModel.Discovery;
using System.Threading;  

namespace Microsoft.Samples.Discovery
{
    class FindAsyncResult : AsyncResult
    {
        FindRequestContext findRequestContext;
        DiscoveryClient discoveryClient;

        int pendingEndpointCount;

        public FindAsyncResult(FindRequestContext findRequestContext, DiscoveryEndpoint forwardingDiscoveryEndpoint, AsyncCallback callback, object state)
            : base(callback, state)
        {
            // Store the context. Responses will be added to this context
            this.findRequestContext = findRequestContext;

            // Attach delegates which will handle the find responses
            this.discoveryClient = new DiscoveryClient(forwardingDiscoveryEndpoint);
            this.discoveryClient.FindProgressChanged += new EventHandler<FindProgressChangedEventArgs>(FindProgressChanged);
            this.discoveryClient.FindCompleted += new EventHandler<FindCompletedEventArgs>(FindCompleted);

            // Forward the Probe request message
            this.discoveryClient.FindAsync(findRequestContext.Criteria);
        }        

        void FindProgressChanged(object sender, FindProgressChangedEventArgs e)
        {
            // Add EndpointDiscoveryMetadata to FindRequestContext.
            // If Request-Response message exchange pattern is used, the responses will be collected and sent in one message to the original sender (client)
            // If duplex message exchange pattern is used, each of the responses will be sent to the original sender (client) as they are received
            this.findRequestContext.AddMatchingEndpoint(e.EndpointDiscoveryMetadata);

            if (Interlocked.Increment(ref pendingEndpointCount) == 0)
            {
                this.Complete(false);
            }
        }

        void FindCompleted(object sender, FindCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw e.Error;
            }

            try
            {
                // Ensures that all FindProgressedChanged events are raised before completing the
                // AsyncResult. If there are pending FindProgressedChanged events to be raised, then
                // the AsyncResult will be completed in the FindProgressedChanged method after all
                // events are raised.
                if (Interlocked.Add(ref pendingEndpointCount, -e.Result.Endpoints.Count) == 0)
                {
                    this.Complete(false);
                }
            }
            catch (Exception exception)
            {
                this.Complete(false, exception);
            }
        }

        public static void End(IAsyncResult result)
        {
            FindAsyncResult thisPtr = AsyncResult.End<FindAsyncResult>(result);
            thisPtr.discoveryClient.Close();
        }
    }
}
