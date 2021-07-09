//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ServiceModel.Discovery;

namespace Microsoft.Samples.Discovery
{
    class CustomDiscoveryService : DiscoveryService
    {
        ReadOnlyCollection<EndpointDiscoveryMetadata> publishedEndpoints;
        Uri customScopeMatchBy = new Uri("net.tcp://Microsoft.Samples.Discovery/ORExactMatch");

        public CustomDiscoveryService(ReadOnlyCollection<EndpointDiscoveryMetadata> publishedEndpoints)
        {
            this.publishedEndpoints = publishedEndpoints;
        }

        protected override IAsyncResult OnBeginFind(FindRequestContext findRequestContext, AsyncCallback callback, object state)
        {
            // Implements an OR matching that adds an endpoint if any of its scopes match any of the scopes in FindCriteria
            if (findRequestContext.Criteria.ScopeMatchBy.Equals(customScopeMatchBy))
            {
                foreach (EndpointDiscoveryMetadata endpointDiscoveryMetadata in this.publishedEndpoints)
                {
                    bool endpointAdded = false;
                    
                    foreach (Uri findCriteriaScope in findRequestContext.Criteria.Scopes)
                    {
                        if (endpointAdded)
                        {
                            break;
                        }

                        foreach (Uri serviceEndpointScope in endpointDiscoveryMetadata.Scopes)
                        {                            
                            if (serviceEndpointScope.Equals(findCriteriaScope))
                            {
                                findRequestContext.AddMatchingEndpoint(endpointDiscoveryMetadata);
                                endpointAdded = true;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (EndpointDiscoveryMetadata endpointDiscoveryMetadata in this.publishedEndpoints)
                {
                    if (findRequestContext.Criteria.IsMatch(endpointDiscoveryMetadata))
                    {
                        findRequestContext.AddMatchingEndpoint(endpointDiscoveryMetadata);
                    }
                }
            }

            return new FindAsyncResult(callback, state);
        }

        protected override void OnEndFind(IAsyncResult result)
        {
            FindAsyncResult.End(result);
        }

        protected override IAsyncResult OnBeginResolve(ResolveCriteria resolveCriteria, AsyncCallback callback, object state)
        {
            EndpointDiscoveryMetadata matchingEndpoint = null;

            foreach (EndpointDiscoveryMetadata endpointDiscoveryMetadata in this.publishedEndpoints)
            {
                if (resolveCriteria.Address == endpointDiscoveryMetadata.Address)
                {
                    matchingEndpoint = endpointDiscoveryMetadata;
                    break;
                }
            }

            return new ResolveAsyncResult(matchingEndpoint, callback, state);
        }

        protected override EndpointDiscoveryMetadata OnEndResolve(IAsyncResult result)
        {
            return ResolveAsyncResult.End(result);
        }    

        sealed class FindAsyncResult : AsyncResult
        {
            public FindAsyncResult(AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.Complete(true);
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<FindAsyncResult>(result);
            }
        }

        sealed class ResolveAsyncResult : AsyncResult
        {
            EndpointDiscoveryMetadata matchingEndpoint;

            public ResolveAsyncResult(EndpointDiscoveryMetadata matchingEndpoint, AsyncCallback callback, object state)
                : base(callback, state)
            {
                this.matchingEndpoint = matchingEndpoint;
                this.Complete(true);
            }

            public static EndpointDiscoveryMetadata End(IAsyncResult result)
            {
                ResolveAsyncResult thisPtr = AsyncResult.End<ResolveAsyncResult>(result);
                return thisPtr.matchingEndpoint;
            }
        }

    }
}
